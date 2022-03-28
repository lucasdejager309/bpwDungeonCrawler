using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponItem", menuName = "Item/WeaponItem")]
public class WeaponItem : Item, IWeapon
{
    public Range DamageRange {
        get {
            return damageRange;
        }
        set {
            damageRange = value;
        }
    }
    [Header("Weapon Attributes")]
    [SerializeField] Range damageRange;

    public int GetDamage() {
        return (int)damageRange.GetRandom();
    }

    public override string GetDescription()
    {
        string description = base.GetDescription();
        description = "Does " + damageRange.min + "-" + damageRange.max + " damage.\n\n" + description;
        return description;
    }
}
