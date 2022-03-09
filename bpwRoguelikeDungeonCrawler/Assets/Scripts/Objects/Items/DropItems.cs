using System.Collections.Generic;
using UnityEngine;

public class DropItems : MonoBehaviour {
    public int minItems = 1;
    public int maxItems = 4;
    public LootTable lootTable;
    
    public void DropFromLootTable(Vector2Int pos) {
        Dictionary<Item, int> lootItems = lootTable.GetItemsFromTable(Random.Range(minItems, maxItems));

        foreach (var kv in lootItems) {
            GameManager.Instance.player.GetComponent<Inventory>().SpawnDroppedItem(kv.Key, kv.Value, pos); 
        }
    }
}