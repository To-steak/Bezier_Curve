using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public GameObject player;
    public abstract void OnTriggerEnter(Collider other);
    private void Update()
    {
        transform.Rotate(Vector3.up, 10 * Time.deltaTime);
    }
}
