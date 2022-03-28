using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectInfo {
    public Sprite sprite;
    public string name;
    public string description;

    public InspectInfo() {

    }
    public InspectInfo(Sprite sprite, string name, string description = "") {
        this.sprite = sprite;
        this.name = name;
        this.description = description;
    }

    public static InspectInfo GetInfo(Vector2Int pos) {
        InspectInfo info = EntityManager.Instance.GetInfo(pos);
        if (info == null) {
            info = DungeonGen.Instance.GetInfo(pos);
            info = new InspectInfo(info.sprite, "Nothing", "There's nothing there!");
        }
        return info;
    }
}

public interface Inspectable{
    public InspectInfo GetInfo(Vector2Int pos);
}