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
        entityIsSolid = false;
        this.item = item;
        this.amount = amount;
        GetComponent<SpriteRenderer>().sprite = this.item.itemSprite;
    }

    public override void Interact(GameObject interacter)
    {
        interacter.GetComponent<Inventory>().AddItem(item, amount);
        Die();
    }
}
