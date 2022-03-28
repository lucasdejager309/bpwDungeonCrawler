using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class Save {
    public Save(int currentLevel, int currentHealth, int currentStrength, int currentIntelligence, Dictionary<int, int> items, Dictionary<string, int> slots) {
        this.currentLevel = currentLevel;
        this.currentHealth = currentHealth;
        this.currentStrength = currentStrength;
        this.currentIntelligence = currentIntelligence;
        this.inventoryItems = items;
        this.equipSlotItems = slots;
    }

    public int currentLevel;
    public int currentHealth;
    public int currentStrength;
    public int currentIntelligence;
    public Dictionary<int, int> inventoryItems = new Dictionary<int, int>();
    public Dictionary<string, int> equipSlotItems;
}

[CreateAssetMenu(fileName = "New Saving", menuName = "Saving")]
public class Saving : ScriptableObject {
    public Item[] itemIndexes;

    public Save save;
    
    public void Save(int currentLevel, GameObject player) {
        EquipSlot[] slots = player.GetComponent<PlayerInventory>().equipSlots;
        List<InventoryItem> items = player.GetComponent<PlayerInventory>().Items;
        int currentHealth = player.GetComponent<Player>().Health;
        int currentStrength = player.GetComponent<Player>().Strength;
        int currentIntelligence = player.GetComponent<Player>().Intelligence;



        Dictionary<string, int> slotIndexes = new Dictionary<string, int>();
        foreach(EquipSlot slot in slots) {
            slotIndexes.Add(slot.slotID, GetItemIndex(slot.item));
        }
        
        Save save = new Save(currentLevel, currentHealth, currentStrength, currentIntelligence, ItemsToIndexes(items), slotIndexes);

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, save);
        stream.Close();
    }

    public Dictionary<int, int> ItemsToIndexes(List<InventoryItem> items) {
        Dictionary<int, int> indexes = new Dictionary<int, int>();
        foreach (InventoryItem item in items) {
            indexes.Add(GetItemIndex(item.item), item.amount);
        }
        return indexes;
    }

    public int GetItemIndex(Item item) {
        for (int i = 0; i < itemIndexes.Length; i++) {
            if (itemIndexes[i] == item) {
                return i;
            }
        }

        return 999;
    }

    public Item GetItemFromIndex(int index) {
        if (index == 999) {
            return null;
        } else return itemIndexes[index];
    }

    public EquipSlot[] GetSlotsFromSave(EquipSlot[] slots) {
        
        int index = 0;
        foreach (var kv in save.equipSlotItems) {
            slots[index].slotID = kv.Key;
            if (GetItemFromIndex(kv.Value) != null) {
                slots[index].item = GetItemFromIndex(kv.Value);
            }
            index++;
        }

        return slots;
    }

    public List<InventoryItem> GetItemsFromSave() {
        List<InventoryItem> items = new List<InventoryItem>();

        foreach (var kv in save.inventoryItems) {
            items.Add(new InventoryItem(GetItemFromIndex(kv.Key), kv.Value));
        }

        return items;
    }

    public Save GetSave() {
        string path = Application.persistentDataPath + "/save.data";
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            save = formatter.Deserialize(stream) as Save;
            stream.Close();
            
            return save;
        } else {
            Debug.Log("oopsie woopsie file not found");
            return null;
        }
    }
}