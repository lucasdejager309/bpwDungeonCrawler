using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : Entity
{
    
    public void Interact() {
        Debug.Log("interacted @ "+ transform.position);
    }
}
