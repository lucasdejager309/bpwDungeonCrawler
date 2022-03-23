using UnityEngine;

[CreateAssetMenu(fileName = "NEW ThrowableItem", menuName = "Item/ThrowableItem")]
public class ThrowableItem : ConsumableItem, IWeapon {
    
    [Header("Throwable")]
    public GameObject projectilePrefab;
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;

    public int GetDamage() {
        return Random.Range(minDamage, maxDamage);
    }
    
    public override void Use()
    {
        GameManager.Instance.ToggleAimingPointer();
        UIManager.Instance.aimpointer.itemToThrow = this;
        
        RangedAttack ranged = GameManager.Instance.player.GetComponent<RangedAttack>();
        ranged.projectilePrefab = projectilePrefab;
    }

    public override string GetDescription()
    {
        string description = base.GetDescription();
        description = "Does " + minDamage + "-" + maxDamage + " damage when thrown.\n\n" + description;
        return description;
    }
}