using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootItem {
    public Item item;
    
    [Header("Required/Optional")]
    public bool required; //required to drop
    public int pickChance;

    [Header("Amount")]
    public bool multiple;
    public Range amountRange;

    public int GetAmount() {
        if (multiple) {
            return (int)Random.Range(amountRange.min, amountRange.max);
        } else return 1;
    }
}

[CreateAssetMenu(fileName = "New LootTable", menuName = "LootTable")]
public class LootTable : ScriptableObject {
    public LootItem[] table;

    public Dictionary<Item, int> GetItemsFromTable(int amount) {
        Dictionary<Item, int> pickedItems = new Dictionary<Item, int>();

        foreach (LootItem item in table) {
            if (item.required) {
                pickedItems = AddItem(pickedItems, item.item, item.GetAmount());
            }
        }

        for (int i = 0; i < amount; i++) {
            LootItem pickedItem = null;
            
            float probabilitySum = 0;
        
            //get sum of probabilities
            foreach(LootItem item in table) {
                probabilitySum += item.pickChance;
            }

            //generate random number
            float randomFloat = Random.Range(0, probabilitySum+1);


            foreach(LootItem item in table) {
                if (!item.required) {
                    if (randomFloat > 0) {
                    randomFloat -= item.pickChance;
                    pickedItem = item;
                    } else break;
                }
            }

            if (pickedItem == null) {
                pickedItem = table[table.Length-1];
            }

            pickedItems = AddItem(pickedItems, pickedItem.item, pickedItem.GetAmount());
        }

        return pickedItems;
    }

    Dictionary<Item, int> AddItem(Dictionary<Item, int> dict, Item pickedItem, int amount) {
        if (dict.ContainsKey(pickedItem)) {
            dict[pickedItem] += amount;
        } else {
            dict.Add(pickedItem, amount);
        }

        return dict;
    }
}