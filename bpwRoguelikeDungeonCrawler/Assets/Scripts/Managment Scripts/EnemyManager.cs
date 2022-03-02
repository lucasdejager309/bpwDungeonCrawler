using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyManager : Singleton<EnemyManager>
{
    public bool spawnEnemies;
    public float minEnemyDensity;
    public float maxEnemyDensity;
    public SpawnableObject[] enemyPrefabs;
    public List<GameObject> enemies = new List<GameObject>();

    void Awake() {
        Instance = this;
    } 
 
    void Start() {
        EventManager.AddListener("DUNGEON_GENERATED", SpawnEnemies);
        EventManager.AddListener("PLAYER_TURN_FINISHED", OtherTurns);
    }

    void OtherTurns() {
        StartCoroutine(DoActions());
    }

    void SpawnEnemies() {
        Dictionary<Vector2Int, GameObject> enemiesToSpawn = EntityManager.Instance.SpawnByDensity(enemyPrefabs, minEnemyDensity, maxEnemyDensity);
        foreach (KeyValuePair<Vector2Int, GameObject> enemy in enemiesToSpawn) {
            enemies.Add(EntityManager.Instance.SpawnEntity(enemy.Key, enemy.Value));
        }
    }

    IEnumerator DoActions() {
        EventManager.InvokeEvent("UI_WAIT");

        List<GameObject> actionQueue = new List<GameObject>();

        foreach(GameObject enemy in enemies) {
            if (enemy != null) {
                bool playerInSight = false;
                bool playerInrange = false;
                if (Vector2.Distance(enemy.GetComponent<Enemy>().GetPos(), GameManager.Instance.player.GetComponent<Player>().GetPos()) <= enemy.GetComponent<Enemy>().sightRange*1.4f) {
                    playerInrange = true;
                    playerInSight = GameManager.Instance.player.GetComponent<Player>().TileInSight(enemy.GetComponent<Enemy>().GetPos());
                }
                if (playerInSight && playerInrange) {
                    actionQueue.Add(enemy);
                }   
            }
        }

        if (actionQueue.Count != 0) {
            int i = 0;
            bool next = true;
            while (i < actionQueue.Count) {
                Task t = new Task(actionQueue[i].GetComponent<Entity>().DoAction(), false);
                if (next) {
                    t.Start();
                    next = false;
                }
                t.Finished += delegate {
                    i++;
                    next = true;
                };
                yield return null;
            }
        }
        EventManager.InvokeEvent("UI_WAIT");
        EventManager.InvokeEvent("OTHERS_TURN_FINISHED"); 
    }
}
