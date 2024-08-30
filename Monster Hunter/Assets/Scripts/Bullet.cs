using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private const int AmmoDamage = 25;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Enemy>().enemyCurrentHp -= AmmoDamage;
        }
        gameObject.GetComponent<TrailRenderer>().Clear();
        transform.position = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}