using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponItem", menuName = "Item/WeaponItem")]
public class WeaponItem : Item, IWeapon
{
    [Header("Weapon Attributes")]
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;

    public int GetDamage() {
        return Random.Range(minDamage, maxDamage);
    }

    public override string GetDescription()
    {
        string description = base.GetDescription();
        description = "Does " + minDamage + "-" + maxDamage + " damage.\n\n" + description;
        return description;
    }
}
