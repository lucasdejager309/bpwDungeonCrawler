using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonAction : UIAction
{
    public int sceneIndexToLoad;

    public override void DoAction()
    {
        SceneManager.LoadScene(sceneIndexToLoad);
    }
}
