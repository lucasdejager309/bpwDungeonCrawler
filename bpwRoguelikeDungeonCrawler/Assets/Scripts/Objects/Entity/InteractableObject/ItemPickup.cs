using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : InteractableObject
{
    public Item item;
    public int amount;

    public override void Start() {
        base.Start();
        transform.position += new Vector3(0.5f, 0.5f, 0);
        transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
    }

    public void SetItem(Item item, int amount = 1) {
        isSolid = false;
        this.item = item;
        this.amount = amount;
        GetComponent<SpriteRenderer>().sprite = this.item.itemSprite;
    }

    public override void Interact(GameObject interacter)
    {
        interacter.GetComponent<Inventory>().AddItem(item, amount);
        cantBeDestroyed = false;
        
        if (amount > 1) {
            LogText.Instance.Log("You picked up " + item.itemName + " (" + amount + ")");
        } else {
            LogText.Instance.Log("You picked up " + item.itemName);
        }
        
        Die();
    }

    public override InspectInfo GetInfo()
    {
        InspectInfo info = base.GetInfo();
        info.name = item.itemName + " (" + amount + ")";
        info.description = item.GetDescription();
        return info;
    }
}
