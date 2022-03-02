using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : Entity
{
    
    public virtual void Interact() {
        Debug.Log("interacted @ "+ transform.position);
    }
}
 