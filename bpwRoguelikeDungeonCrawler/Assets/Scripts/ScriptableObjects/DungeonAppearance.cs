using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DungeonAppearance", menuName = "DungeonAppearance")]
public class DungeonAppearance : ScriptableObject
{
    public GameObject startOfLevel;
    public GameObject endOfLevel;

    public TileLayer floorTilelayer;
    public TileLayer solidTileLayer;
    public TileLayer dungeonFeaturelayer;
}
