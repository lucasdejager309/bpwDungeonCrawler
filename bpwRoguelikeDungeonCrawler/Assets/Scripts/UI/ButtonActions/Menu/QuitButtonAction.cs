using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitButtonAction : UIAction
{
    public bool save;

    public override void DoAction()
    {
        GameManager.Instance.SetControlTo(GameManager.Controlling.PLAYER);
        UIManager.Instance.escMenu.TogglePanel(false);

        if (save) {
            GameManager.Instance.Save();
        }

        Application.Quit();       
    }
}
