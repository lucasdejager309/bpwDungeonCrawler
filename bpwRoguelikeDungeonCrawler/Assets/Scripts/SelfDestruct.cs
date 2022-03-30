using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float time;

    void Start()
    {
        StartCoroutine(Destroy(time));
    }

    IEnumerator Destroy(float time) {
        yield return new WaitForSeconds(time); 
        Destroy(this.gameObject);
    }
}
