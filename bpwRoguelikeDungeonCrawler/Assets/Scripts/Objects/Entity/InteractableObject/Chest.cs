using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractableObject {
    public int minItemsFromChest = 1;
    public int maxItemsFromChest = 4;
    public LootTable lootTable;
    
    public override void Interact(GameObject interacter)
    {
        List<Item> lootItems = lootTable.GetItemsFromTable(Random.Range(minItemsFromChest, maxItemsFromChest));

        foreach (Item item in lootItems) {
            interacter.GetComponent<Inventory>().AddItem(item);
        }
        interacter.GetComponent<Inventory>().DebugLogContent();

        base.Interact(interacter);
    }
}
