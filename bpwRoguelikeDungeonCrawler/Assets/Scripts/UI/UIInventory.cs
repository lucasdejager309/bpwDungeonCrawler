using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : UIPanel 
{
    public Text itemName;
    public Text description;

    public Text useButton;
    public Text STR;
    public Text INT;


    const int ROW_SIZE = 3;
    const int COLUMN_SIZE = 3;
    int MAX_SLOTS {
        get {
            return ROW_SIZE*COLUMN_SIZE;
        }
    }

    public void UpdateInventory() {
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();

        for (int i = 0; i < selectablePanels.Length; i++) {
            Image image = selectablePanels[i].transform.GetChild(0).GetComponent<Image>();
            Text text = selectablePanels[i].transform.GetChild(1).GetComponent<Text>();
            
            if (inventory.GetItem(i) != null) {

                image.sprite = inventory.GetItem(i).itemSprite;
                image.enabled = true;
                
                int itemAmount = inventory.GetItemAmount(inventory.GetItem(i));
                if (itemAmount > 1) {
                    text.text = itemAmount.ToString();
                } else {
                    text.text = "";
                }
                
            } else {
                image.sprite  = null;
                image.enabled = false;
                text.text = "";
            }
        }
    }

    public void UpdateCard() {
        PlayerInventory inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();
        Item item = inventory.GetItem(PointerIndex);
        if (item != null) {
            
            itemName.text = item.itemName;
            description.text = item.GetDescription();

            if (inventory.pointerIndex >= inventory.MAX_SLOTS) {
                useButton.text = "UNEQUIP";
            } else if (item.useButtonText != "") {
                useButton.text = item.useButtonText;
            } else {
                useButton.text = "USE";
            }
        }
    }

    public override void UpdatePointer(Vector2Int input) {
        if (input != new Vector2Int(0, 0)) {
            //REGULAR INVENTORY SLOTS
            if (PointerIndex < MAX_SLOTS) {
                if (Mathf.Abs(input.x) == 1) {
                if (PointerIndex+input.x >= 0 && PointerIndex+input.x < MAX_SLOTS) {
                    SetPointer(PointerIndex+input.x);
                }
                
                }
                if (Mathf.Abs(input.y) == 1) {
                    if (PointerIndex-input.y*ROW_SIZE >= 0 && PointerIndex-input.y*ROW_SIZE < MAX_SLOTS) {
                        SetPointer(PointerIndex-input.y*ROW_SIZE);
                    } else if (PointerIndex < ROW_SIZE && input.y == 1) {
                        SetPointer(MAX_SLOTS);
                    }
                }
            //EQUIP SLOTS
            } else {
                
                if (Mathf.Abs(input.x) == 1) {
                    if (PointerIndex+input.x >= MAX_SLOTS && PointerIndex+input.x < selectablePanels.Length) {
                        SetPointer(PointerIndex+input.x);
                    }
                }

                if (input.y == -1) {
                    SetPointer(0);
                }
            }
            
        }
        GameManager.Instance.player.GetComponent<PlayerInventory>().SetInventoryPointer(PointerIndex);
    }
}
