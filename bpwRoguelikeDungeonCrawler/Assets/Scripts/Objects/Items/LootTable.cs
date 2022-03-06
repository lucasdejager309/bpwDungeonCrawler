using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootItem {
    public int pickChance;
    public Item item;
}

[CreateAssetMenu(fileName = "New LootTable", menuName = "LootTable")]
public class LootTable : ScriptableObject {
    public LootItem[] table;

    public List<Item> GetItemsFromTable(int number) {
        
        List<Item> pickedItems = new List<Item>();
        
        for (int i = 0; i < number; i++) {
            Item pickedItem = null;
            
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
                    pickedItem = item.item;
                } else break;
            }

            if (pickedItem == null) {
                pickedItem = table[table.Length-1].item;
            }

            pickedItems.Add(pickedItem);
        }
        
     
        return pickedItems;
    }
}