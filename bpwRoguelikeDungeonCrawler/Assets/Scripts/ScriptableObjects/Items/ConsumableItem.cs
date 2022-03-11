using UnityEngine;
//[CreateAssetMenu(fileName = "New ConsumableItem", menuName = "Item/ConsumableItem")]
public class ConsumableItem : Item
{
    public override void Use() {
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();
        inventory.RemoveItem(this);
    }
}


