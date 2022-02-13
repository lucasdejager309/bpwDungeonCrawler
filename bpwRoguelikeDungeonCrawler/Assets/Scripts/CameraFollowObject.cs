using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    public string tagToFollow;
    GameObject objectToFollow;
    public Vector2 offset;

    void Update() {
        if (objectToFollow == null) {
            objectToFollow = GameObject.FindGameObjectWithTag(tagToFollow);
            transform.parent = objectToFollow.transform;
            transform.position = new Vector3(objectToFollow.transform.position.x+offset.x, objectToFollow.transform.position.y+offset.y, transform.position.z);
        }
    }
}
