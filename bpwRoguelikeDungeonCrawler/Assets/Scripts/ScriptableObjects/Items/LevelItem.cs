using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LevelItem", menuName = "Item/LevelItem")]
public class LevelItem : ConsumableItem
{
    [Header("Level")]
    public int addINT = 0;
    public int addSTR = 0;

    public override void Use()
    {
        Player player = GameManager.Instance.player.GetComponent<Player>();
        if (addSTR != 0) {
            player.SetStrength(player.Strength + addSTR);
        }
        if (addINT != 0) {
            player.SetInteligence(player.Inteligence + addINT);
        }

        base.Use();
    }

    public override string GetDescription()
    {
        string description = "";
        if (addSTR != 0) {
            description = description +  "+" + addSTR + " STR\n";
        }
        if (addINT != 0) {
            description = description +  "+" + addINT + " INT\n";
        }
        description = description + "\n" + itemDescription;

        return description;
    }
}
