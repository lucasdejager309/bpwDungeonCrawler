using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToNextLevel : InteractableObject
{
    public int sceneToLoad;

    public override void Interact(GameObject interacter)
    {
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }
}
