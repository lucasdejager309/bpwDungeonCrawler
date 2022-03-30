using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGen : Singleton<DungeonGen>
{
    public List<Room> roomList {get; private set; } = new List<Room>();

    public GameObject doorPrefab;

    public Tilemap floorTileMap;
    public Tilemap solidTileMap;
    public Tilemap dungeonFeatureMap;

    public Dictionary<Vector2Int, Tile> floorTileDictionary = new Dictionary<Vector2Int, Tile>();
    public Dictionary<Vector2Int, Tile> solidTileDictionary = new Dictionary<Vector2Int, Tile>();
    public Dictionary<Vector2Int, Tile> dungeonFeatureDictionary = new Dictionary<Vector2Int, Tile>();

    public Vector2Int SpawnPos;

    public Vector2Int playerSpawnPos;

    void Awake() {
        Instance = this;
    }

    public void GenerateDungeon(DungeonAppearance appearance, DungeonSettings settings) {

        EventManager.InvokeEvent("RELOAD_DUNGEON");
        WipeDungeon();
        AllocateRooms(settings, appearance);
        AllocateCorridors(settings, appearance);
        AllocateWalls(appearance);

        if (appearance.dungeonFeatures.Length != 0) {
            AllocateDungeonFeatures(settings, appearance);
        }

        FindRoomEntrances();

        EntityManager.Instance.ClearEntityDict();
        CreateLevelEnds(appearance);

        SpawnTiles();

        EventManager.InvokeEvent("DUNGEON_GENERATED");
    }

    public void WipeDungeon() {
        //this is bad code
        
        floorTileMap.ClearAllTiles();
        floorTileDictionary.Clear();

        solidTileMap.ClearAllTiles();
        solidTileDictionary.Clear();

        dungeonFeatureMap.ClearAllTiles();
        dungeonFeatureDictionary.Clear();

        roomList.Clear();
    }

    public InspectInfo GetInfo(Vector2Int pos) {
        if (solidTileDictionary.ContainsKey(pos)) {
            return new InspectInfo(solidTileDictionary[pos].sprite, solidTileDictionary[pos].name);
        } else if (dungeonFeatureDictionary.ContainsKey(pos)) {
            return new InspectInfo(dungeonFeatureDictionary[pos].sprite, dungeonFeatureDictionary[pos].name);
        } else if (floorTileDictionary.ContainsKey(pos)) {
           return new InspectInfo(floorTileDictionary[pos].sprite, floorTileDictionary[pos].name);
        }
        return null;
    }

    void AllocateRooms(DungeonSettings settings, DungeonAppearance appearance) {
        for (int i = 0; i <= settings.amountRooms; i++){
            Room room = new Room() {
                size = new Vector2Int((int)settings.roomSizeRange.GetRandom(), (int)settings.roomSizeRange.GetRandom()),
                sequencialRoom = true
            };

            if (i == 0) {
                room.spawnRoom = true;
                room.position = new Vector2Int(0,0);

                roomList.Add(room);
                AddRoomToDungeon(room, appearance);
            } else {
                room.position = roomList[i - 1].position + new Vector2Int(Random.Range(-settings.newRoomRange, settings.newRoomRange), Random.Range(-settings.newRoomRange, settings.newRoomRange));
                if (i == settings.amountRooms-1) {
                    room.endRoom = true;
                }

                if (CheckRoomPos(room, floorTileDictionary, settings)) {
                    roomList.Add(room);
                    AddRoomToDungeon(room, appearance);
                } else {
                    i--;
                }
            }
        }

        //random rooms
        for (int i = 0; i < settings.amountRandomRooms; i++) {
            Room room = new Room() {
                size = new Vector2Int((int)Random.Range(settings.roomSizeRange.min, settings.roomSizeRange.max), (int)Random.Range(settings.roomSizeRange.min, settings.roomSizeRange.max)),
                sequencialRoom = false
            };
            Room linkedRoom = roomList[Random.Range(0, roomList.Count)];
            room.linkedToRoom = linkedRoom;
            room.position = linkedRoom.position + new Vector2Int(Random.Range(-settings.newRoomRange, settings.newRoomRange), Random.Range(-settings.newRoomRange, settings.newRoomRange));
            if (CheckRoomPos(room, floorTileDictionary, settings))  {
                roomList.Add(room);
                AddRoomToDungeon(room, appearance);
            } else { i--;}
        }
    }

    void AllocateCorridors(DungeonSettings settings, DungeonAppearance appearance) {
        for (int i = 0; i < roomList.Count; i++) {
            Room startRoom = roomList[i];
            Room otherRoom;
            
            if (startRoom.linkedToRoom != null) {
                otherRoom = startRoom.linkedToRoom;
            } else {
                otherRoom = roomList[i+1];
            }

            Vector2Int startPos = GetCorridorStartInRoom(startRoom, settings);
            Vector2Int endPos = GetCorridorStartInRoom(otherRoom, settings);

            Vector2Int corridorDir = new Vector2Int((int)Mathf.Sign(endPos.x - startPos.x), (int)Mathf.Sign(endPos.y - startPos.y));

            for (int x = startPos.x; x != endPos.x; x += corridorDir.x) {
                for (int c = 0; c < settings.corridorWidth; c++) {
                    Vector2Int pos = new Vector2Int(x, startPos.y+c);

                    Tile tileToAdd = GenTile.PickRandomGenTile(appearance.floorTiles).GetTile();
                    AddTileToDictionary(pos, tileToAdd, floorTileDictionary, false);
                }
            }

            //what does this do?? (i commented it out and it still works so idk)
            if (corridorDir.y < 0) {
                startPos.y += Mathf.RoundToInt(settings.corridorWidth/2);
            }

            for (int y = startPos.y; y != endPos.y; y += corridorDir.y) {
                for (int c = 0; c < settings.corridorWidth; c++) {
                    Vector2Int pos = new Vector2Int(endPos.x+c, y);
                    
                    AddTileToDictionary(pos, GenTile.PickRandomGenTile(appearance.floorTiles).GetTile(), floorTileDictionary, false);
                }
            }
        }
    }

    public Vector2Int GetCorridorStartInRoom(Room room, DungeonSettings settings) {

        Vector2Int pos = new Vector2Int();

        Wall chosenWall = (Wall)Mathf.RoundToInt(Random.Range(0, 4));

        switch(chosenWall) {
            case Wall.bottom:
                pos.y = room.position.y+settings.corridorWidth;
                break;
            case Wall.top:
                pos.y = room.position.y+room.size.y-settings.corridorWidth-1;
                break;
            case Wall.left:
                pos.x = room.position.x+settings.corridorWidth;
                break;
            case Wall.right:
                pos.x = room.position.x+room.size.x-settings.corridorWidth-1;
                break;
        }

        if (chosenWall == Wall.bottom || chosenWall == Wall.top) {
            pos.x = Random.Range(room.position.x, room.position.x+room.size.x-1);
        } else {
            pos.y = Random.Range(room.position.y, room.position.y+room.size.y-1);
        }

        return pos;
    }

    void AllocateWalls(DungeonAppearance appearance) {
        foreach(KeyValuePair<Vector2Int, Tile> kv in floorTileDictionary) {
            Vector2Int position = kv.Key;
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    Vector2Int gridPos = position + new Vector2Int(x, y);
                    if (!floorTileDictionary.ContainsKey(gridPos)) {
                        AddTileToDictionary(gridPos, GenTile.PickRandomGenTile(appearance.solidTiles).GetTile(), solidTileDictionary, false);
                    } 
                }
            }
        }
    }

    void FindRoomEntrances() {
        foreach (Room room in roomList) {
            //check bottom wall
            for (int i = room.position.x; i < room.position.x+room.size.x; i++) {
                Vector2Int possiblePos = new Vector2Int(i, room.position.y-1);
                if (!solidTileDictionary.ContainsKey(possiblePos)) {
                    room.entrances.Add(possiblePos);
                }
            }
            //check top wall
            for (int i = room.position.x; i < room.position.x+room.size.x; i++) {
                Vector2Int possiblePos = new Vector2Int(i, room.position.y+room.size.y);
                if (!solidTileDictionary.ContainsKey(possiblePos)) {
                    room.entrances.Add(possiblePos);
                }
            }
            //check right wall
            for (int i = room.position.y; i < room.position.y+room.size.y; i++) {
                Vector2Int possiblePos = new Vector2Int(room.position.x+room.size.x, i);
                if (!solidTileDictionary.ContainsKey(possiblePos)) {
                    room.entrances.Add(possiblePos);
                }
            }
            //check left wall
            for (int i = room.position.y; i < room.position.y+room.size.y; i++) {
                Vector2Int possiblePos = new Vector2Int(room.position.x-1, i);
                if (!solidTileDictionary.ContainsKey(possiblePos)) {
                    room.entrances.Add(possiblePos);
                }
            }
        }
    }

    void AllocateDungeonFeatures(DungeonSettings settings, DungeonAppearance appearance) {
        foreach(Room room in roomList) {
            int amountOfFeatures = (int)Random.Range(settings.dungeonFeaturesAmountRange.min, settings.dungeonFeaturesAmountRange.max);
            for (int i = 0; i < amountOfFeatures; i++) {
                DungeonFeature feature = DungeonFeature.PickRandomFeature(appearance.dungeonFeatures);
                Vector2Int pos = new Vector2Int();
                
                bool posFound = false;
                while(!posFound) {
                    pos = room.GetRandomPosInRoom(feature.wallAdjacent);
                    if (!solidTileDictionary.ContainsKey(pos) && !dungeonFeatureDictionary.ContainsKey(pos)) {
                        posFound = true;
                    }
                }

                if (!feature.solid) {
                    AddTileToDictionary(pos, feature.GetTile(), dungeonFeatureDictionary, false);
                } else {
                    AddTileToDictionary(pos, feature.GetTile(), solidTileDictionary, false);
                }
            }
        }
    }

    bool AddTileToDictionary(Vector2Int pos, Tile tiletoAdd, Dictionary<Vector2Int, Tile> dictionary , bool overwrite) {

        if (overwrite || !dictionary.ContainsKey(pos))
        {
            dictionary.Add(pos, tiletoAdd);
            return true;
        }
        return false;
        
    }

    void CreateLevelEnds(DungeonAppearance appearance) {
        foreach(Room room in roomList) {
            if (room.endRoom) {
                Vector2Int pos = new Vector2Int();
                
                bool done = false;
                while (!done) {
                    pos = room.GetRandomPosInRoom(false);
                    if (!dungeonFeatureDictionary.ContainsKey(pos)) {
                        done = true;
                    }
                }
                EntityManager.Instance.SpawnEntity(pos, appearance.endOfLevel);

            }

            if (room.spawnRoom) {
                Vector2Int pos = new Vector2Int();
                
                bool done = false;
                while (!done) {
                    pos = room.GetRandomPosInRoom(false);
                    if (!dungeonFeatureDictionary.ContainsKey(pos)) {
                        done = true;
                    }
                }
                EntityManager.Instance.SpawnEntity(pos, appearance.startOfLevel);
                playerSpawnPos = pos;
            }
        }
    }

    public void SpawnTiles() {
        //this is bad code
        
        foreach(KeyValuePair<Vector2Int, Tile> entry in floorTileDictionary) {
                Vector3Int location = new Vector3Int(entry.Key.x, entry.Key.y, 0);
                floorTileMap.SetTile(location, entry.Value);
        }

        foreach(KeyValuePair<Vector2Int, Tile> entry in solidTileDictionary) {
                Vector3Int location = new Vector3Int(entry.Key.x, entry.Key.y, 0);
                solidTileMap.SetTile(location, entry.Value);
        }

        foreach(KeyValuePair<Vector2Int, Tile> entry in dungeonFeatureDictionary) {
                Vector3Int location = new Vector3Int(entry.Key.x, entry.Key.y, 0);
                dungeonFeatureMap.SetTile(location, entry.Value);
        }
    }

    void SpawnDoors() {
        foreach(Room room in roomList) {
            foreach(Vector2Int entrance in room.entrances) {
                Instantiate(doorPrefab, new Vector3(entrance.x, entrance.y, -3), Quaternion.identity);
                EntityManager.Instance.SpawnEntity(entrance, doorPrefab);
            }
        }
    }

    private bool CheckRoomPos(Room room, Dictionary<Vector2Int, Tile> tileDictionary, DungeonSettings settings) {
        for (int i = 0; i < roomList.Count; i++) {
            for (int xx = room.position.x-settings.minRoomSpacing; xx < room.position.x + room.size.x+settings.minRoomSpacing; xx++) {
                 for (int yy = room.position.y-settings.minRoomSpacing; yy < room.position.y + room.size.y+settings.minRoomSpacing; yy++) {
                     Vector2Int pos = new Vector2Int(xx, yy);
                    
                     if (tileDictionary.ContainsKey(pos)) {
                         return false;
                     }

                     if (Mathf.Abs(pos.x) > settings.maxRange | Mathf.Abs(pos.y) > settings.maxRange) {
                         return false;
                     }
                 }
            }
        }

        return true;
    }

    private void AddRoomToDungeon(Room room, DungeonAppearance appearance) {
        for (int x = room.position.x; x < room.position.x+room.size.x; x++) {
            for (int y = room.position.y; y < room.position.y+room.size.y; y++) {
                Vector2Int pos = new Vector2Int(x, y);
                AddTileToDictionary(pos, GenTile.PickRandomGenTile(appearance.floorTiles).GetTile(), floorTileDictionary, false);
            }
        }
    }

    public Room GetRandomRoom(bool includeSpawnRoom = false) {
        Room room = new Room();
        if (!includeSpawnRoom) {
            bool roomFound = false;
            while (!roomFound) {
                room = roomList[Random.Range(0, roomList.Count)];
                if (!room.spawnRoom) {
                    roomFound = true;
                }
            }
        } else {
            room = roomList[Random.Range(0, roomList.Count)];
        }
        
        return room;
    }

}

