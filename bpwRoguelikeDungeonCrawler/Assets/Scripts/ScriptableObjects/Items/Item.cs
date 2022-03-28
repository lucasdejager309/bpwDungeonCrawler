using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Item")]
[System.Serializable]
public class Item : ScriptableObject
{
    [Header("Basic Information")]
    public string itemName;
    public Sprite itemSprite;
    public string itemDescription;
    public bool stackAble;
    public string useButtonText;

    [Header("Equippable")]
    public bool equippable;
    public string equipSlotID;
    public int requiredStrength = 0;
    public int requiredIntelligence = 0;

    public virtual void Use() {
        //use!
    }

    public virtual string GetDescription() {
        string description = "";
        if (requiredStrength > 0) {
            description = description + "REQ STR: " + requiredStrength + "\n\n";
        }
        if (requiredIntelligence > 0) {
            description = description + "REQ INT: " + requiredIntelligence + "\n\n";
        }
        description = description + itemDescription;
        return description;
    }

    public void DeleteItem() {
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();
        inventory.RemoveItem(this);
    }
}
