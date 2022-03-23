using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour {
    public float animationSpeed = 10f;

    public IEnumerator DoInteract(Vector2Int pos) {
        foreach(var kv in EntityManager.Instance.entityDict) {
            if (kv.Value == pos) {
                if (kv.Key.GetComponent<InteractableObject>() != null) {
                    kv.Key.GetComponent<InteractableObject>().Interact(this.gameObject);

                    bool finished = false;

                    if (pos != GameManager.Instance.player.GetComponent<Player>().GetPos() && kv.Key.GetComponent<InteractableObject>().doInteractAnim) {
                        Task t = new Task(MeleeAttack.AttackAnimMelee(pos, this.GetComponent<Entity>(), animationSpeed));
                        t.Finished += delegate {
                            finished = true;
                        };
                    } else {
                        finished = true;
                    }

                    

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

    public void DoInteractMultiple(Vector2Int pos) {
        List<InteractableObject> objects = new List<InteractableObject>();
        foreach (var kv in EntityManager.Instance.entityDict) {
            if (kv.Value == pos) {
                if (kv.Key.GetComponent<InteractableObject>() != null) {
                    objects.Add(kv.Key.GetComponent<InteractableObject>());
                }
            }
        }

        foreach (InteractableObject obj in objects) {
            obj.GetComponent<InteractableObject>().Interact(this.gameObject);
        }
    }
}