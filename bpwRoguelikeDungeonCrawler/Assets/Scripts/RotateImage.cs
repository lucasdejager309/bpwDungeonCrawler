using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateImage : MonoBehaviour
{
    public bool rotating = true;
    public float rotateSpeed = 200;
    public float rotationDirection = 1;
    public Transform imageToRotate;

    void Update() {
        float smooth = Time.deltaTime * rotateSpeed;
        transform.Rotate(new Vector3(0, 0, rotationDirection) * smooth);
    }
}
