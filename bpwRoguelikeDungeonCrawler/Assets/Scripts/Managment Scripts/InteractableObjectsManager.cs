using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectsManager : Singleton<InteractableObjectsManager>
{       
    void Awake() {
        Instance = this;
    }

    public void SpawnObjects() {
        DungeonSettings settings = GameManager.Instance.GetSettings();

        Dictionary<Vector2Int, GameObject> objectsToSpawn = new Dictionary<Vector2Int, GameObject>();
        objectsToSpawn = EntityManager.Instance.SpawnByDensity(settings.interactableObjects, settings.interactableObjectsDensityRange.min, settings.interactableObjectsDensityRange.max);
        EntityManager.Instance.SpawnEntities(objectsToSpawn);
    }
}
