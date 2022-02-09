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

    public TileLayer floorTilelayer;
    public TileLayer wallTilelayer;
    public TileLayer dungeonFeaturelayer;

    public Vector2Int SpawnPos;

    void Awake() {
        Instance = this;
    }

    public void GenerateDungeon() {
        WipeDungeon();
        AllocateRooms();
        AllocateCorridors();
        AllocateWalls();

        CreateLevelEnds();

        SpawnTiles();
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

            Vector2Int startPos = GetRandomPosInRoom(startRoom);
            Vector2Int endPos = GetRandomPosInRoom(otherRoom);

            Vector2Int corridorDir = new Vector2Int((int)Mathf.Sign(endPos.x - startPos.x), (int)Mathf.Sign(endPos.y - startPos.y));

            for (int x = startPos.x; x != endPos.x; x += corridorDir.x) {
                for (int c = 0; c < corridorWidth; c++) {
                    Vector2Int pos = new Vector2Int(x, startPos.y+c);

                    AddTileToDictionary(pos, PickRandomTile(floorTilelayer.tiles), floorTilelayer, false);
                }
            }

            //what does this do?? (i commented it out and it still works so idk)
            if (corridorDir.y < 0) {
                startPos.y += Mathf.RoundToInt(corridorWidth/2);
            }

            for (int y = startPos.y; y != endPos.y; y += corridorDir.y) {
                for (int c = 0; c < corridorWidth; c++) {
                    Vector2Int pos = new Vector2Int(endPos.x+c, y);
                    
                    AddTileToDictionary(pos, PickRandomTile(floorTilelayer.tiles), floorTilelayer, false);
                }
            }
        }
    }

    void AllocateWalls() {
        foreach(KeyValuePair<Vector2Int, Tile> kv in floorTilelayer.tileDictionary) {
            Vector2Int position = kv.Key;
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    Vector2Int gridPos = position + new Vector2Int(x, y);
                    if (!floorTilelayer.tileDictionary.ContainsKey(gridPos)) {
                        AddTileToDictionary(gridPos, PickRandomTile(wallTilelayer.tiles), wallTilelayer, false);
                    } 
                }
            }
        }
    }

    bool AddTileToDictionary(Vector2Int pos, Tile tiletoAdd, TileLayer tileLayer, bool overwrite) {
            foreach (GenTile genTile in tileLayer.tiles) {
                if (genTile.tile == tiletoAdd) {
                    if (overwrite || !tileLayer.tileDictionary.ContainsKey(pos)) {
                        tileLayer.tileDictionary.Add(pos, genTile.tile);
                        return true;
                    }
                }
        }

        return false;
    }

    void CreateLevelEnds() {
        foreach(Room room in roomList) {
            if (room.endRoom) {
                Vector2Int pos = GetRandomPosInRoom(room);

                AddTileToDictionary(pos, endTile, dungeonFeaturelayer, true);
            }

            if (room.spawnRoom) {
                Vector2Int pos = GetRandomPosInRoom(room);

                AddTileToDictionary(pos, startTile, dungeonFeaturelayer, true);
                SpawnPos = pos;
            }
        }
    }

    public Vector2Int GetRandomPosInRoom(Room room) {
        return new Vector2Int(Random.Range(room.position.x+corridorWidth, room.position.x + room.size.x - corridorWidth), 
        Random.Range(room.position.y + corridorWidth, room.position.y + room.size.y - corridorWidth));
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
                AddTileToDictionary(pos, PickRandomTile(floorTilelayer.tiles), floorTilelayer, false);
            }
        }
    }

    private Tile PickRandomTile(GenTile[] genTiles) {
        //how the fuck does relative probability work??
        //https://forum.unity.com/threads/random-item-spawn-using-array-with-item-rarity-variable.176234/
        
        Tile pickedTile = null;
        float probabilitySum = 0;
        
        //get sum of probabilities
        foreach(GenTile genTile in genTiles) {
            probabilitySum += genTile.spawnChance;
        }

        //generate random number
        float randomFloat = Random.Range(0, probabilitySum+1);

        //pick tile
        for (int i = 0; i < genTiles.Length && randomFloat > 0; i++) {
            randomFloat -= genTiles[i].spawnChance;
            pickedTile = genTiles[i].tile;
        }
        if (pickedTile == null) {
            pickedTile = genTiles[genTiles.Length-1].tile;
        }
     
        return pickedTile;
    }
}

