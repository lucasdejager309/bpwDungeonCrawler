using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] List<GameObject> objectsToFollow = new List<GameObject>();
    public float speed;
    public Vector2 offset;

    void Start() {
        EventManager.AddListener("RELOAD_DUNGEON", ClearObjectsToFollow);
    }

    public void SetCameraPos(Vector2 pos) {
        transform.position = new Vector3 (pos.x+offset.x, pos.y+offset.y, transform.position.z);
    }

    public void ClearObjectsToFollow() {
        objectsToFollow = new List<GameObject>();
    }

    public void SetCameraToFollow(GameObject objectToFollow) {
        objectsToFollow.Add(objectToFollow);
    }

    public void RemoveCameraFollow(GameObject objectToUnFollow) {
        objectsToFollow.Remove(objectToUnFollow);
    }

    void Update() {
        if (objectsToFollow.Count > 1) {
            float x = 0;
            float y = 0;

            foreach (GameObject obj in objectsToFollow) {
                x += obj.transform.position.x;
                y += obj.transform.position.y;
            }

            x = x/objectsToFollow.Count;
            y = y/objectsToFollow.Count;

            Vector3 targetPos = new Vector3 (x+offset.x, y+offset.y, transform.position.z);

            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed);
        } else {
            transform.position = new Vector3(objectsToFollow[0].transform.position.x+offset.x, objectsToFollow[0].transform.position.y+offset.y, transform.position.z);
        }
        
    }
}
