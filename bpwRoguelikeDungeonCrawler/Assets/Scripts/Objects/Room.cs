using UnityEngine;
using System.Collections.Generic;

enum Wall {
    bottom = 0,
    top = 1,
    left = 2,
    right = 3
}

public class Room {
    public Vector2Int position;
    public Vector2Int size;
    public bool spawnRoom;
    public bool endRoom;

    public bool sequencialRoom;
    public Room linkedToRoom;

    public float enemyDensity;
    public List<Vector2Int> validSpawnPositions = new List<Vector2Int>();

    public List<Vector2Int> entrances = new List<Vector2Int>();
    
    public Vector2Int GetRandomPosInRoom(bool wallAdjacent) {
        if (!wallAdjacent) {
            return new Vector2Int(Random.Range(position.x, position.x+size.x), Random.Range(position.y, position.y+size.y));
        } else {
            Vector2Int pos = new Vector2Int();
            bool posFound = false;
            
            while (!posFound) {
                Wall chosenWall = (Wall)Mathf.RoundToInt(Random.Range(0, 4));

                switch(chosenWall) {
                    case Wall.bottom:
                        pos.y = position.y;
                        break;
                    case Wall.top:
                        pos.y = position.y+size.y-1;
                        break;
                    case Wall.left:
                        pos.x = position.x;
                        break;
                    case Wall.right:
                        pos.x = position.x+size.x-1;
                        break;
                }

                if (chosenWall == Wall.bottom || chosenWall == Wall.top) {
                    pos.x = Random.Range(position.x, position.x+size.x-1);
                } else {
                    pos.y = Random.Range(position.y, position.y+size.y-1);
                }

                //if no entrances found around pos, return pos, else loop
                bool entranceFound = false;
                for (int x = pos.x-1; x < pos.x+1; x++) {
                    if (entrances.Contains(new Vector2Int(x, pos.y))) {
                        entranceFound = true;
                    }
                }
                for (int y = pos.y-1; y < pos.y+2; y++) {
                    if (entrances.Contains(new Vector2Int(pos.x, y))){
                        entranceFound = true;
                    }
                }

                if (!entranceFound) {
                    return pos;
                }
            }
        }

        return new Vector2Int();
        
    }
}