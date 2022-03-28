using System.Collections.Generic;
using UnityEngine;

public class DropItems : MonoBehaviour {
    public Range itemsAmount;
    public LootTable table;
    public List<Item> privateTable;

    public void DropFromLootTable(Vector2Int pos) {
        if (privateTable.Count > 0) {
            for (int i = 0; i < privateTable.Count; i++) {
                SpawnDroppedItem(privateTable[i], 1, pos); 
            }
        }
        if (table != null) {
            Dictionary<Item, int> itemsToDrop = table.GetItemsFromTable((int)itemsAmount.GetRandom());
            if (itemsToDrop.Count > 0) {
                foreach (var kv in itemsToDrop) {
                    SpawnDroppedItem(kv.Key, kv.Value, pos);
                }
            }
        }
    }

    public static void SpawnDroppedItem(Item item, int amount, Vector2Int pos) {
        GameObject droppedItemObject = EntityManager.Instance.SpawnEntity(pos, GameManager.Instance.droppedItemPrefab);
        droppedItemObject.GetComponent<ItemPickup>().SetItem(item, amount);
    }
}