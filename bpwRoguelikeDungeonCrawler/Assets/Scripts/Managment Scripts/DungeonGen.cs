using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGen : Singleton<DungeonGen>
{
    public int maxRange;

    public int amountRooms = 15;
    public int amountRandomRooms = 10;
    public int minRoomSize = 5;
    public int maxRoomSize = 10;

    public int minNewRoomRange = -20;
    public int maxNewRoomRange = 20;
    public int minRoomSpacing = 3;

    public int corridorWidth = 2;

    public List<Room> roomList {get; private set; } = new List<Room>();

    public Tile startTile;
    public Tile endTile;
    public Tile tempTestTile;

    public TileLayer floorTilelayer;
    public TileLayer wallTilelayer;
    public TileLayer dungeonFeaturelayer;

    public int minDungeonFeaturesPerRoom;
    public int maxDungeonFeaturesPerRoom;
    public DungeonFeature[] dungeonFeatures;

    public Vector2Int SpawnPos;

    void Awake() {
        Instance = this;
        EventManager.AddListener("RELOAD_DUNGEON", GenerateDungeon);
    }

    public void GenerateDungeon() {
        WipeDungeon();
        AllocateRooms();
        AllocateCorridors();
        AllocateWalls();
        FindRoomEntrances();

        CreateLevelEnds();

        if (dungeonFeatures.Length != 0) {
            AllocateDungeonFeatures();
        }

        SpawnTiles();

        EventManager.InvokeEvent("DUNGEON_GENERATED");
    }

    public void WipeDungeon() {
        //this is bad code
        
        floorTilelayer.tilemap.ClearAllTiles();
        floorTilelayer.tileDictionary.Clear();

        wallTilelayer.tilemap.ClearAllTiles();
        wallTilelayer.tileDictionary.Clear();

        dungeonFeaturelayer.tilemap.ClearAllTiles();
        dungeonFeaturelayer.tileDictionary.Clear();

        roomList.Clear();
    }

    void AllocateRooms() {
        for (int i = 0; i <= amountRooms; i++){
            Room room = new Room() {
                size = new Vector2Int(Random.Range(minRoomSize, maxRoomSize), Random.Range(minRoomSize, maxRoomSize)),
                sequencialRoom = true
            };

            if (i == 0) {
                room.spawnRoom = true;
                room.position = new Vector2Int(0,0);

                roomList.Add(room);
                AddRoomToDungeon(room);
            } else {
                room.position = roomList[i - 1].position + new Vector2Int(Random.Range(minNewRoomRange, maxNewRoomRange), Random.Range(minNewRoomRange, maxNewRoomRange));
                if (i == amountRooms-1) {
                    room.endRoom = true;
                }

                if (CheckRoomPos(room, floorTilelayer)) {
                    roomList.Add(room);
                    AddRoomToDungeon(room);
                } else {
                    i--;
                }
            }
        }

        //random rooms
        for (int i = 0; i < amountRandomRooms; i++) {
            Room room = new Room() {
                size = new Vector2Int(Random.Range(minRoomSize, maxRoomSize), Random.Range(minRoomSize, maxRoomSize)),
                sequencialRoom = false
            };
            Room linkedRoom = roomList[Random.Range(0, roomList.Count)];
            room.linkedToRoom = linkedRoom;
            room.position = linkedRoom.position + new Vector2Int(Random.Range(minNewRoomRange, maxNewRoomRange), Random.Range(minNewRoomRange, maxNewRoomRange));
            if (CheckRoomPos(room, floorTilelayer))  {
                roomList.Add(room);
                AddRoomToDungeon(room);
            } else { i--;}
        }
    }

    void AllocateCorridors() {
        for (int i = 0; i < roomList.Count; i++) {
            Room startRoom = roomList[i];
            Room otherRoom;
            
            if (startRoom.linkedToRoom != null) {
                otherRoom = startRoom.linkedToRoom;
            } else {
                otherRoom = roomList[i+1];
            }

            Vector2Int startPos = GetCorridorStartInRoom(startRoom);
            Vector2Int endPos = GetCorridorStartInRoom(otherRoom);

            Vector2Int corridorDir = new Vector2Int((int)Mathf.Sign(endPos.x - startPos.x), (int)Mathf.Sign(endPos.y - startPos.y));

            for (int x = startPos.x; x != endPos.x; x += corridorDir.x) {
                for (int c = 0; c < corridorWidth; c++) {
                    Vector2Int pos = new Vector2Int(x, startPos.y+c);

                    AddTileToDictionary(pos, GenTile.PickRandomTile(floorTilelayer.tiles).tile, floorTilelayer, false);
                }
            }

            //what does this do?? (i commented it out and it still works so idk)
            if (corridorDir.y < 0) {
                startPos.y += Mathf.RoundToInt(corridorWidth/2);
            }

            for (int y = startPos.y; y != endPos.y; y += corridorDir.y) {
                for (int c = 0; c < corridorWidth; c++) {
                    Vector2Int pos = new Vector2Int(endPos.x+c, y);
                    
                    AddTileToDictionary(pos, GenTile.PickRandomTile(floorTilelayer.tiles).tile, floorTilelayer, false);
                }
            }
        }
    }

    public Vector2Int GetCorridorStartInRoom(Room room) {
        // return new Vector2Int(Random.Range(room.position.x+corridorWidth, room.position.x + room.size.x - corridorWidth), 
        // Random.Range(room.position.y + corridorWidth, room.position.y + room.size.y - corridorWidth));


        Vector2Int pos = new Vector2Int();

        Wall chosenWall = (Wall)Mathf.RoundToInt(Random.Range(0, 4));

        switch(chosenWall) {
            case Wall.bottom:
                pos.y = room.position.y+corridorWidth;
                break;
            case Wall.top:
                pos.y = room.position.y+room.size.y-corridorWidth-1;
                break;
            case Wall.left:
                pos.x = room.position.x+corridorWidth;
                break;
            case Wall.right:
                pos.x = room.position.x+room.size.x-corridorWidth-1;
                break;
        }

        if (chosenWall == Wall.bottom || chosenWall == Wall.top) {
            pos.x = Random.Range(room.position.x, room.position.x+room.size.x-1);
        } else {
            pos.y = Random.Range(room.position.y, room.position.y+room.size.y-1);
        }

        return pos;
    }

    void AllocateWalls() {
        foreach(KeyValuePair<Vector2Int, Tile> kv in floorTilelayer.tileDictionary) {
            Vector2Int position = kv.Key;
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    Vector2Int gridPos = position + new Vector2Int(x, y);
                    if (!floorTilelayer.tileDictionary.ContainsKey(gridPos)) {
                        AddTileToDictionary(gridPos, GenTile.PickRandomTile(wallTilelayer.tiles).tile, wallTilelayer, false);
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
                if (!wallTilelayer.tileDictionary.ContainsKey(possiblePos)) {
                    room.entrances.Add(possiblePos);
                }
            }
            //check top wall
            for (int i = room.position.x; i < room.position.x+room.size.x; i++) {
                Vector2Int possiblePos = new Vector2Int(i, room.position.y+room.size.y);
                if (!wallTilelayer.tileDictionary.ContainsKey(possiblePos)) {
                    room.entrances.Add(possiblePos);
                }
            }
            //check right wall
            for (int i = room.position.y; i < room.position.y+room.size.y; i++) {
                Vector2Int possiblePos = new Vector2Int(room.position.x+room.size.x, i);
                if (!wallTilelayer.tileDictionary.ContainsKey(possiblePos)) {
                    room.entrances.Add(possiblePos);
                }
            }
            //check left wall
            for (int i = room.position.y; i < room.position.y+room.size.y; i++) {
                Vector2Int possiblePos = new Vector2Int(room.position.x-1, i);
                if (!wallTilelayer.tileDictionary.ContainsKey(possiblePos)) {
                    room.entrances.Add(possiblePos);
                }
            }
        }
    }

    void AllocateDungeonFeatures() {
        foreach(Room room in roomList) {
            int amountOfFeatures = Random.Range(minDungeonFeaturesPerRoom, maxDungeonFeaturesPerRoom);
            for (int i = 0; i < amountOfFeatures; i++) {
                DungeonFeature feature = DungeonFeature.PickRandomFeature(dungeonFeatures);
                Vector2Int pos = room.GetRandomPosInRoom(feature.wallAdjacent);
                
                AddTileToDictionary(pos, feature.tile, dungeonFeaturelayer, false);
                
            }
        }
    }

    bool AddTileToDictionary(Vector2Int pos, Tile tiletoAdd, TileLayer tileLayer, bool overwrite) {
        //     foreach (GenTile genTile in tileLayer.tiles) {
        //         if (genTile.tile == tiletoAdd) {
        //             if (overwrite || !tileLayer.tileDictionary.ContainsKey(pos)) {
        //                 tileLayer.tileDictionary.Add(pos, genTile.tile);
        //                 return true;
        //             }
        //         }
        // }

        // return false;

        if (overwrite || !tileLayer.tileDictionary.ContainsKey(pos))
        {
            tileLayer.tileDictionary.Add(pos, tiletoAdd);
            return true;
        }
        return false;
        
    }

    void CreateLevelEnds() {
        foreach(Room room in roomList) {
            if (room.endRoom) {
                Vector2Int pos = room.GetRandomPosInRoom(false);

                AddTileToDictionary(pos, endTile, dungeonFeaturelayer, true);
            }

            if (room.spawnRoom) {
                Vector2Int pos = room.GetRandomPosInRoom(false);

                AddTileToDictionary(pos, startTile, dungeonFeaturelayer, true);
                SpawnPos = pos;
            }
        }
    }

    void SpawnTiles() {
        //this is bad code
        
        foreach(KeyValuePair<Vector2Int, Tile> entry in floorTilelayer.tileDictionary) {
                Vector3Int location = new Vector3Int(entry.Key.x, entry.Key.y, 0);
                floorTilelayer.tilemap.SetTile(location, entry.Value);
        }

        foreach(KeyValuePair<Vector2Int, Tile> entry in wallTilelayer.tileDictionary) {
                Vector3Int location = new Vector3Int(entry.Key.x, entry.Key.y, 0);
                wallTilelayer.tilemap.SetTile(location, entry.Value);
        }

        foreach(KeyValuePair<Vector2Int, Tile> entry in dungeonFeaturelayer.tileDictionary) {
                Vector3Int location = new Vector3Int(entry.Key.x, entry.Key.y, 0);
                dungeonFeaturelayer.tilemap.SetTile(location, entry.Value);
        }
    }

    private bool CheckRoomPos(Room room, TileLayer tileLayer) {
        for (int i = 0; i < roomList.Count; i++) {
            for (int xx = room.position.x-minRoomSpacing; xx < room.position.x + room.size.x+minRoomSpacing; xx++) {
                 for (int yy = room.position.y-minRoomSpacing; yy < room.position.y + room.size.y+minRoomSpacing; yy++) {
                     Vector2Int pos = new Vector2Int(xx, yy);
                    
                     if (tileLayer.tileDictionary.ContainsKey(pos)) {
                         return false;
                     }

                     if (Mathf.Abs(pos.x) > maxRange | Mathf.Abs(pos.y) > maxRange) {
                         return false;
                     }
                 }
            }
        }

        return true;
    }

    private void AddRoomToDungeon(Room room) {
        for (int x = room.position.x; x < room.position.x+room.size.x; x++) {
            for (int y = room.position.y; y < room.position.y+room.size.y; y++) {
                Vector2Int pos = new Vector2Int(x, y);
                AddTileToDictionary(pos, GenTile.PickRandomTile(floorTilelayer.tiles).tile, floorTilelayer, false);
            }
        }
    }

}

