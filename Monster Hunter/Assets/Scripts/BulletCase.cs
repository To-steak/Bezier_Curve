using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCase : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        Invoke("Clear", 3f);
    }

    private void Clear()
    {
        transform.position = Vector3.zero;
        gameObject.SetActive(false);
    }
}
