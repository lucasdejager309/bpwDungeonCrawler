using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AimpointerControlObject", menuName = "ControlObject/AimpointerControlObject")]
public class AimPointerControlObject : ControlObject
{
    public override void Interact()
    {
        GameObject player = GameManager.Instance.player;

        if (player.GetComponent<RangedAttack>().HasAimOnTarget(UIManager.Instance.aimpointer.GetPos())) {
            player.GetComponent<Player>().inputAllowed = false;
            GameManager.Instance.Esc();
            Task t = new Task(player.GetComponent<RangedAttack>().DoAttack(UIManager.Instance.aimpointer.GetPos(), 
            player.GetComponent<Player>().CalculateDamage(UIManager.Instance.aimpointer.itemToThrow), player.GetComponent<Entity>()));

            t.Finished += delegate {
                EventManager.InvokeEvent("PLAYER_TURN_FINISHED");
                UIManager.Instance.aimpointer.itemToThrow.DeleteItem();
            }; 
        }
    }

    public override void SetControlTo()
    {
        UIManager.Instance.aimpointer.SetPos(GameManager.Instance.player.GetComponent<Player>().GetPos());
        UIManager.Instance.aimpointer.SetActive(true);
        GameManager.Instance.cameraFollow.SetCameraToFollow(UIManager.Instance.aimpointer.gameObject);
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
