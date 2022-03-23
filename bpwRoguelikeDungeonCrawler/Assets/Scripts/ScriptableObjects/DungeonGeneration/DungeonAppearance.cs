using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New DungeonAppearance", menuName = "DungeonAppearance")]
public class DungeonAppearance : ScriptableObject
{
    [Header("Level Generation")]

    public GameObject startOfLevel;
    public GameObject endOfLevel;

    public GenTile[] floorTiles;
    public GenTile[] solidTiles;
    public DungeonFeature[] dungeonFeatures;

    [Header("Minimap Generation")]

    public Tile floorTile;
    public Tile solidTile;
}
