using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HealItem", menuName = "Item/HealItem")]
public class HealItem : Item
{
    [Header("Healing")]
    public int healingAmount;

    public override void Use()
    {
        Player player = GameManager.Instance.player.GetComponent<Player>();
        player.SetHealth(player.Health + healingAmount);

        base.Use();
    }

    public override string GetDescription()
    {
        return "Heals " + healingAmount + " points of health!\n\n" + itemDescription; 
    }
} 
