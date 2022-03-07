using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    [Header("Basic Information")]
    public string itemName;
    public Sprite itemSprite;
    public bool stackAble;

    [Header("Equippable")]
    public bool equippable;
    public string equipSlotID;
}
