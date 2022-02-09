using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileLayer {
    public string name;
    public Tilemap tilemap;
    public Tile[] tiles;
    public Dictionary<Vector2Int, Tile> tileDictionary;

    public TileLayer() {
        tileDictionary = new Dictionary<Vector2Int, Tile>();
    }
}

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
    public Tile floorTile;
    public Tile wallTile;
    public TileLayer[] tileLayers;


    public Vector2Int SpawnPos;

    void Awake() {
        Instance = this;
    }

    public void GenerateDungeon() {

        AllocateRooms();
        AllocateCorridors();
        AllocateWalls();

        CreateLevelEnds();

        SpawnTiles();

        EventManager.InvokeEvent("DUNGEON_GENERATED");
    }

    public void WipeDungeon() {
        
        
        foreach (TileLayer tileLayer in tileLayers) {
            tileLayer.tilemap.ClearAllTiles();
            tileLayer.tileDictionary.Clear();
        }

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

                if (CheckRoomPos(room, "floorTileMap")) {
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
            if (CheckRoomPos(room, "floorTileMap"))  {
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

                    AddTileToDictionary(pos, floorTile, false);
                }
            }

            //what does this do?? (i commented it out and it still works so idk)
            if (corridorDir.y < 0) {
                startPos.y += Mathf.RoundToInt(corridorWidth/2);
            }

            for (int y = startPos.y; y != endPos.y; y += corridorDir.y) {
                for (int c = 0; c < corridorWidth; c++) {
                    Vector2Int pos = new Vector2Int(endPos.x+c, y);
                    
                    AddTileToDictionary(pos, floorTile, false);
                }
            }
        }
    }

    void AllocateWalls() {
        foreach(TileLayer tileLayer in tileLayers) {
            if (tileLayer.name == "floorTileMap") {
                foreach(KeyValuePair<Vector2Int, Tile> kv in tileLayer.tileDictionary) {
                    Vector2Int position = kv.Key;
                    for (int x = -1; x <= 1; x++) {
                        for (int y = -1; y <= 1; y++) {
                            Vector2Int gridPos = position + new Vector2Int(x, y);
                            if (!tileLayer.tileDictionary.ContainsKey(gridPos)) {
                                AddTileToDictionary(gridPos, wallTile, false);
                            } 
                        }
                    }
                }
            }
        }
    }

    bool AddTileToDictionary(Vector2Int pos, Tile tiletoAdd, bool overwrite) {
        foreach (TileLayer tileLayer in tileLayers) {
            foreach (Tile tile in tileLayer.tiles) {
                if (tile == tiletoAdd) {
                    if (overwrite || !tileLayer.tileDictionary.ContainsKey(pos)) {
                        tileLayer.tileDictionary.Add(pos, tile);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    void CreateLevelEnds() {
        foreach(Room room in roomList) {
            if (room.endRoom) {
                Vector2Int pos = GetRandomPosInRoom(room);

                AddTileToDictionary(pos, endTile, true);
            }

            if (room.spawnRoom) {
                Vector2Int pos = GetRandomPosInRoom(room);

                AddTileToDictionary(pos, startTile, true);
                SpawnPos = pos;
            }
        }
    }

    public Vector2Int GetRandomPosInRoom(Room room) {
        return new Vector2Int(Random.Range(room.position.x+corridorWidth, room.position.x + room.size.x - corridorWidth), 
        Random.Range(room.position.y + corridorWidth, room.position.y + room.size.y - corridorWidth));
    }

    void SpawnTiles() {
        foreach(TileLayer tileLayer in tileLayers) {
            foreach(KeyValuePair<Vector2Int, Tile> entry in tileLayer.tileDictionary) {
                Vector3Int location = new Vector3Int(entry.Key.x, entry.Key.y, 0);

                tileLayer.tilemap.SetTile(location, entry.Value);
                //Debug.Log("set tile" + entry.Value.name + " at " + location + " on layer " + tileLayer.name);
            }
        }
    }

    private bool CheckRoomPos(Room room, string tileLayerName) {
        TileLayer tileLayer = new TileLayer();
        foreach (TileLayer t in tileLayers) {
            if (t.name == tileLayerName) {
                tileLayer = t;
            }
        }

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
                AddTileToDictionary(pos, floorTile, false);
            }
        }
    }
}

