using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToNextLevel : InteractableObject
{
    public int sceneToLoad;

    public InventoryItem[] itemsNeeded;

    public override void Interact(GameObject interacter)
    {
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();

        bool allowed = true;
        foreach (InventoryItem item in itemsNeeded) {
            if (inventory.GetItemAmount(item.item) < item.amount) {
                allowed = false;
            }
        }
        
        if (allowed) {
            GameManager.Instance.NextLevel();
        
            foreach (InventoryItem item in itemsNeeded) {
                inventory.RemoveItems(item.item, item.amount);
            }
        } else {
            foreach (InventoryItem item in itemsNeeded) {
                LogText.Instance.Log("You need " + (item.amount - inventory.GetItemAmount(item.item)) + " more of item " + item.item.itemName + " to descend!");
            }
        }
    }
}
