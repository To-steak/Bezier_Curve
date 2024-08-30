using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Enemy
{
    private void Update()
    {
        transform.Rotate(Vector3.forward * 50 * Time.deltaTime);
    }

    public override void Attack()
    {
        // none
    }

    public override void Died()
    {
        // none
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Enemy") && !other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
