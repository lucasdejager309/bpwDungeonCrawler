using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AimpointerControlObject", menuName = "ControlObject/AimpointerControlObject")]
public class AimPointerControlObject : ControlObject
{
    public override void Interact()
    {            
        GameObject player = GameManager.Instance.player;

        AimPointer aimPointer = UIManager.Instance.aimpointer;

        if (!GameManager.Instance.player.GetComponent<RangedAttack>().HasAimOnTarget(aimPointer.GetPos()) && aimPointer.ignoreWalls == false) {
            //nothing
        } else {
            DoActionAtPointer();
        }
    }

    public virtual void DoActionAtPointer() {
        Task t = new Task(UIManager.Instance.aimpointer.itemToUse.DoAttack());

            t.Finished += delegate {
                EventManager.InvokeEvent("PLAYER_TURN_FINISHED");
                if (UIManager.Instance.aimpointer.itemToUse.useItemOnThrow) {
                    UIManager.Instance.aimpointer.itemToUse.DeleteItem();
                }
            };
        GameManager.Instance.SetControlTo(ControlMode.PLAYER);
    }

    public override void SetControlTo()
    {
        UIManager.Instance.aimpointer.SetPos(GameManager.Instance.player.GetComponent<Player>().GetPos());
        UIManager.Instance.aimpointer.SetActive(true);
        GameManager.Instance.cameraFollow.SetCameraToFollow(UIManager.Instance.aimpointer.gameObject);
        UIManager.Instance.aimpointer.IgnoreWalls(false);
    }

    public override void LoseControl()
    {
        UIManager.Instance.aimpointer.SetActive(false);
        GameManager.Instance.cameraFollow.RemoveCameraFollow(UIManager.Instance.aimpointer.gameObject);
    }

    public override void UpdateControl(Vector2Int input)
    {
        UIManager.Instance.aimpointer.UpdatePos(input);
    }
}
