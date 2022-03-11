using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    public string tagToFollow;
    GameObject objectToFollow;
    public Vector2 offset;

    void Start() {
        EventManager.AddListener("PLAYER_SPAWNED", SetCameraToFollow);
        EventManager.AddListener("RELOAD_DUNGEON", UnCouple);
    }

    void UnCouple() {
        transform.parent = null;
    }

    void SetCameraToFollow() {
        objectToFollow = GameObject.FindGameObjectWithTag(tagToFollow);
        transform.parent = objectToFollow.transform;
        transform.position = new Vector3(transform.parent.position.x+0.5f, transform.parent.position.y+0.5f, -10);
    }

    void Update() {
        if (objectToFollow == null) {
            Debug.Log("oh oh stinky");
        }
        SetCameraToFollow();
    }
}
