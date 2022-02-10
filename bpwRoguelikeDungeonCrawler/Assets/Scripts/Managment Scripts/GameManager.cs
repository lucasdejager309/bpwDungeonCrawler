using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddListener("DUNGEON_GENERATED", InitializeEntityDictionary);
        
        DungeonGen.Instance.GenerateDungeon();
    }

    void InitializeEntityDictionary() {
        Debug.Log("yeet");
    }
}
