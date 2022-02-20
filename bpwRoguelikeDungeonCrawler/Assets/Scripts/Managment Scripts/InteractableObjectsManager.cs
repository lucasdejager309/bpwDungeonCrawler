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
        EventManager.AddListener("INTERACT", Interact);
    }
    
    void SpawnObjects() {
        Dictionary<Vector2Int, GameObject> objectsToSpawn = new Dictionary<Vector2Int, GameObject>();
        objectsToSpawn = EntityManager.Instance.SpawnByDensity(interactableObjects, minDensity, maxDensity);
        EntityManager.Instance.SpawnEntities(objectsToSpawn);
    }

    void Interact() {
        foreach(KeyValuePair<Entity, Vector2Int> entry in EntityManager.Instance.entityDict) {
            if (entry.Key.GetComponent<InteractableObject>() != null) {
                if (entry.Value == GameManager.Instance.GetPlayerPos()) {
                    entry.Key.GetComponent<InteractableObject>().Interact();
                }
            }
        }
        
        //if entity is interactable object

        //do interaction in entity
    }
}
