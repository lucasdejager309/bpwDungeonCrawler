using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyManager : Singleton<EnemyManager>
{
    public bool spawnEnemies;
    public float minEnemyDensity;
    public float maxEnemyDensity;
    public SpawnableObject[] enemyPrefabs;
    public Dictionary<Vector2Int, GameObject> enemies = new Dictionary<Vector2Int, GameObject>();

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
        enemies = EntityManager.Instance.SpawnByDensity(enemyPrefabs, minEnemyDensity, maxEnemyDensity);
        foreach (KeyValuePair<Vector2Int, GameObject> enemy in enemies) {
            EntityManager.Instance.SpawnEntity(enemy.Key, enemy.Value);
        }
    }

    IEnumerator DoActions() {
        List<GameObject> actionQueue = new List<GameObject>();

        foreach(KeyValuePair<Vector2Int, GameObject> enemy in enemies) {
            if (enemy.Value != null) {
                if (enemy.Value.GetComponent<Entity>().TileInSight(GameObject.FindGameObjectWithTag("Player").transform.position)) {
                    actionQueue.Add(enemy.Value);
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
        EventManager.InvokeEvent("OTHERS_TURN_FINISHED"); 
    }
}
