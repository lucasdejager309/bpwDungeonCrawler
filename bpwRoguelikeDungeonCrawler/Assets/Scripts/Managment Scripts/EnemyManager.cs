using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    public bool spawnEnemies;
    public float minEnemyDensity;
    public float maxEnemyDensity;
    public GameObject tempEnemyPrefab;

    void Start() {
        EventManager.AddListener("DUNGEON_GENERATED", SpawnEnemies);
    }

    void SpawnEnemies() {
        foreach (Room room in DungeonGen.Instance.roomList) {
            room.enemyDensity = Random.Range(minEnemyDensity, maxEnemyDensity);

            foreach(Vector2Int spawnPos in room.validSpawnPositions) {
                if (Random.Range(0, 101) < room.enemyDensity) {
                    GameObject enemy = Instantiate(tempEnemyPrefab, new Vector3(spawnPos.x, spawnPos.y, 1), Quaternion.identity);
                    Debug.Log("spawning enemy!");
                    EntityManager.Instance.entityPositions.Add(spawnPos, enemy);
                }
            }
        }
    }
}
