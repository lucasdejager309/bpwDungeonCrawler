using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToNextLevel : InteractableObject
{
    public int sceneToLoad;

    public override void Interact(GameObject interacter)
    {
        GameManager.Instance.NextLevel();
        DungeonGen.Instance.GenerateDungeon(GameManager.Instance.GetAppearance(), GameManager.Instance.GetSettings());
    }
}
