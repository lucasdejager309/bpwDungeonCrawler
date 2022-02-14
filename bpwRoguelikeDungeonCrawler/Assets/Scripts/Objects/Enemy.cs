using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public override IEnumerator DoAction()
    {
        Debug.Log("I did action! @ " + transform.position);
        yield return new WaitForSeconds(0.02f);
    }
}
