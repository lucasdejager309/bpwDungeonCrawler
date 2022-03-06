using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour {
    public float animationSpeed = 10f;

    public IEnumerator DoInteract(Vector2Int pos) {
        foreach(var kv in EntityManager.Instance.entityDict) {
            if (kv.Value == pos) {
                kv.Key.GetComponent<InteractableObject>().Interact(this.gameObject);

                bool finished = false;
                Task t = new Task(MeleeAttack.AttackAnimMelee(pos, this.GetComponent<Entity>(), animationSpeed));
                t.Finished += delegate {
                    finished = true;
                };

                while (true) {
                    if (finished) {
                        yield break;
                    }
                    else yield return null;
                }
            }
        }
    }
}