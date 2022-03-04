using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectsManager : MonoBehaviour
{
    public SpawnableObject[] interactableObjects;
    public float minDensity;
    public float maxDensity;

    void Start()
    {
        EventManager.AddListener("DUNGEON_GENERATED", SpawnObjects);
    }
    
    void SpawnObjects() {
        Dictionary<Vector2Int, GameObject> objectsToSpawn = new Dictionary<Vector2Int, GameObject>();
        objectsToSpawn = EntityManager.Instance.SpawnByDensity(interactableObjects, minDensity, maxDensity);
        EntityManager.Instance.SpawnEntities(objectsToSpawn);
    }
}
