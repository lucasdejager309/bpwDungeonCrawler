using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Vector2Int roomSizeRange;
    [Tooltip("spacing between rooms")]
    public int newRoomRange;
    public int minRoomSpacing;

    [Header("Corridors")]
    public int corridorWidth;

    [Header("Dungeon Features")]
    public Vector2Int dungeonFeaturesAmountRange;

}
