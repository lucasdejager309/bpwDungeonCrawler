using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RequiredObject {
    public int amount;
    public SpawnableObject spawnableObj;
}


[CreateAssetMenu(fileName = "New DungeonSettings", menuName = "DungeonSettings")]
public class DungeonSettings : ScriptableObject
{
    [Header("Rooms")]
    [Tooltip("max range from 0,0 where rooms can genrate")]
    public int maxRange;
    [Tooltip("sequential rooms")]
    public int amountRooms;
    [Tooltip("non-sequential rooms")]
    public int amountRandomRooms;
    public Range roomSizeRange;
    [Tooltip("spacing between rooms")]
    public int newRoomRange;
    public int minRoomSpacing;

    [Header("Corridors")]
    public int corridorWidth;

    [Header("Dungeon Features")]
    public Range dungeonFeaturesAmountRange;

    [Header("Interactable Objects")]
    public Range interactableObjectsDensityRange;
    public SpawnableObject[] interactableObjects;

    [Header("Required")]
    public RequiredObject[] requiredObjects;
    public InventoryItem[] requiredItems;
    
    [Header("Difficulty")]
    public float enemyDamageMultiplier = 1;
    public float enemyHealthMultiplier = 1;

    public Range enemyDensityRange;
    public SpawnableObject[] enemyPrefabs;

    [Header("Loot")]
    public int lootAmount;
    public LootTable lootTable;
}
