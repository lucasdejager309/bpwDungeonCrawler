using UnityEngine;

[CreateAssetMenu(fileName = "New ConsumableItem", menuName = "Item/ConsumableItem")]
public class ConsumableItem : Item
{
    public string consumeSound;

    public override void Use() {
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();
        inventory.RemoveItem(this);
        AudioManager.Instance.PlaySound(consumeSound);
        LogText.Instance.Log("You used " + itemName);

        EventManager.InvokeEvent("PLAYER_TURN_FINISHED");
    }
}


