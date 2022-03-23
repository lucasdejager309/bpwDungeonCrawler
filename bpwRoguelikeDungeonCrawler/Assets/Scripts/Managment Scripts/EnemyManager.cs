using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyManager : Singleton<EnemyManager>
{
    public bool spawnEnemies;
    public List<GameObject> enemies = new List<GameObject>();

    void Awake() {
        Instance = this;
    } 
 
    void Start() {
        EventManager.AddListener("DUNGEON_GENERATED", SpawnEnemies);
        EventManager.AddListener("PLAYER_TURN_FINISHED", OtherTurns);
        EventManager.AddListener("DAMAGE_HAPPENED", UpdateHealthBars);
    }

    void OtherTurns() {
        StartCoroutine(DoActions());
    }

    public void SetEnemyTarget(GameObject target) {
        foreach (GameObject enemyObject in enemies) {
            enemyObject.GetComponent<Enemy>().target = target;
        }
    }

    void UpdateHealthBars() {
        foreach (GameObject enemyObject in enemies) {
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            enemy.slider.SetMaxValue(enemy.MaxHealth);
            

            if (enemy.Health < enemy.MaxHealth) { 
                enemy.slider.SetActive(true);
                enemy.slider.SetValue(enemy.Health);
                               
            } else {
                enemy.slider.SetActive(false);
            }
        }
    }

    void SpawnEnemies() {
        DungeonSettings settings = GameManager.Instance.GetSettings();

        if (spawnEnemies) {
            Dictionary<Vector2Int, GameObject> enemiesToSpawn = EntityManager.Instance.SpawnByDensity(settings.enemyPrefabs, settings.enemyDensityRange.min, settings.enemyDensityRange.max);
            foreach (KeyValuePair<Vector2Int, GameObject> enemy in enemiesToSpawn) {
                enemies.Add(EntityManager.Instance.SpawnEntity(enemy.Key, enemy.Value));
            }
        }
    }

    public void WipeEnemyList() {
        enemies.Clear();
    }

    public void SpawnEnemy(GameObject enemy, Vector2Int pos) {
        DungeonSettings settings = GameManager.Instance.GetSettings();
        
        GameObject spawnedObject = EntityManager.Instance.SpawnEntity(pos, enemy);
        Enemy spawnedEnemy = spawnedObject.GetComponent<Enemy>();
        spawnedEnemy.SetHealth(Mathf.CeilToInt(spawnedEnemy.MaxHealth*settings.enemyHealthMultiplier));
        spawnedEnemy.SetDamage(settings.enemyDamageMultiplier);
        enemies.Add(spawnedObject);
    }

    IEnumerator DoActions() {
        EventManager.InvokeEvent("UI_TOGGLE_WAIT");

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
            //this is a bad fix for the wrong attack pos bug
            yield return new WaitForSeconds(0.1f);

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
                    GameManager.Instance.AddTurn();
                };
                yield return null;
            }
        }
        EventManager.InvokeEvent("UI_TOGGLE_WAIT");
        EventManager.InvokeEvent("OTHER_TURNS_FINISHED"); 
    }
}
