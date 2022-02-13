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
    }

    void SetCameraToFollow() {
        objectToFollow = GameObject.FindGameObjectWithTag(tagToFollow);
    }

    void Update() {
        if (objectToFollow != null) {
            transform.position = new Vector3(objectToFollow.transform.position.x+offset.x, objectToFollow.transform.position.y+offset.y, transform.position.z);
        } else {
            SetCameraToFollow();
        }
    }
}
