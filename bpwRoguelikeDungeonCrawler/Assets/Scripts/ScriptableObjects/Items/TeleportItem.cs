using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TeleportItem", menuName = "Item/TeleportItem")]
public class TeleportItem : ConsumableItem
{
    public override void Use()
    {
        Player player = GameManager.Instance.player.GetComponent<Player>();
        player.SetPos(DungeonGen.Instance.playerSpawnPos);
        GameManager.Instance.cameraFollow.SetCameraPos(player.GetPos());
        base.Use();
    }
}
