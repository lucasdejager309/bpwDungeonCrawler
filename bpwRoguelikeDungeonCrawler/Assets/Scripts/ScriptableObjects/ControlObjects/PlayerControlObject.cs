using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerControlObject", menuName = "ControlObject/PlayerControlObject")]
public class PlayerControlObject : ControlObject
{
    public override void Interact()
    {
        GameObject player = GameManager.Instance.player;
        
        player.GetComponent<Interact>().DoInteractMultiple(player.GetComponent<Player>().GetPos());
    }

    public override void SetControlTo()
    {
        
    }

    public override void UpdateControl(Vector2Int input)
    {
        GameManager.Instance.player.GetComponent<Player>().UpdatePlayer(input);
    }
}
 