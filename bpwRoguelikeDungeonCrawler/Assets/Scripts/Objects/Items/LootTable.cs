using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootItem {
    public int pickChance;
    public Item item;

    [Header("multiple")]
    public bool multiple;
    public Vector2Int amountRange;

    public int GetAmount() {
        return Random.Range(amountRange.x, amountRange.y);
    }
}

[CreateAssetMenu(fileName = "New LootTable", menuName = "LootTable")]
public class LootTable : ScriptableObject {
    public LootItem[] table;

    public List<Item> GetItemsFromTable(int number) {
        
        List<Item> pickedItems = new List<Item>();
        
        for (int i = 0; i < number; i++) {
            LootItem pickedItem = null;
            
            float probabilitySum = 0;
        
            //get sum of probabilities
            foreach(LootItem item in table) {
                probabilitySum += item.pickChance;
            }

            //generate random number
            float randomFloat = Random.Range(0, probabilitySum+1);


            foreach(LootItem item in table) {
                if (randomFloat > 0) {
                    randomFloat -= item.pickChance;
                    pickedItem = item;
                } else break;
            }

            if (pickedItem == null) {
                pickedItem = table[table.Length-1];
            }


            if (pickedItem.multiple) {
                int amount = pickedItem.GetAmount();
                for (int a = 0; a < amount; a++) {
                    pickedItems.Add(pickedItem.item);
                }
            }
            pickedItems.Add(pickedItem.item);
        }
        
     
        return pickedItems;
    }
}