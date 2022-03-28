using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "NEW ThrowableItem", menuName = "Item/ThrowableItem")]
public class ThrowableItem : ConsumableItem, IWeapon {
    public Range DamageRange {
        set {
            damageRange = value;
        }
        get {
            return damageRange;
        }
    }


    [Header("Throwable")]
    public GameObject projectilePrefab;
    public bool useItemOnThrow;
    public int throwRange;
    public int attackRange;
    public string verb;
    [SerializeField] Range damageRange;

    public int GetDamage() {
        return (int)damageRange.GetRandom();
    }
    
    public virtual IEnumerator DoAttack() {
        GameObject player = GameManager.Instance.player;
        bool finished = false;

        Task t = new Task(player.GetComponent<RangedAttack>().DoAttack(UIManager.Instance.aimpointer.GetPos(), 
            player.GetComponent<Player>().CalculateDamage(UIManager.Instance.aimpointer.itemToUse), player.GetComponent<Entity>(), attackRange));
    
        t.Finished += delegate {
            finished = true;
        };

        while (true) {
                if (finished) {
                    yield break;
                } else yield return null;
            }
    }

    public override void Use()
    {
        GameManager.Instance.SetControlTo(ControlMode.AIM_POINTER);
        UIManager.Instance.aimpointer.itemToUse = this;
        
        RangedAttack ranged = GameManager.Instance.player.GetComponent<RangedAttack>();
        ranged.projectilePrefab = projectilePrefab;
    }

    public override string GetDescription() {  
        string description = base.GetDescription();
        description = "Does " + damageRange.min + "-" + damageRange.max + " damage when " + verb + ".\n\n" + description;
        return description;
    }
}