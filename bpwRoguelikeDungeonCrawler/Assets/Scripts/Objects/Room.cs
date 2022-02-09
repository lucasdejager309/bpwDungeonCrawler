using UnityEngine;
using System.Collections.Generic;

public class Room {
    public Vector2Int position;
    public Vector2Int size;
    public bool spawnRoom;
    public bool endRoom;

    public bool sequencialRoom;
    public Room linkedToRoom;
    //public List<Enemy> enemies;
}