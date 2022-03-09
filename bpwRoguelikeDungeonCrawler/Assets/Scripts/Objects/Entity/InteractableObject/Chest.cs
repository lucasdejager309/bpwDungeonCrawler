using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractableObject {
    
    public override void Interact(GameObject interacter)
    {
        GetComponent<DropItems>().DropFromLootTable(GetPos());

        Die();
    }
}
