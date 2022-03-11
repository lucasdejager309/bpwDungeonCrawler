using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ArmorItem", menuName = "Item/ArmorItem")]
public class ArmorItem : Item
{
    [Header("Armor attributes")]
    public int maxAbsorbedDamage;

    public int AbsorbDamage() {
        return Random.Range(0, maxAbsorbedDamage);
    }

    public override string GetDescription()
    {
        string description = base.GetDescription();
        description = "Protects from up to " + maxAbsorbedDamage +  " damage when worn.\n\n" + description;
        return description;
    }
}
