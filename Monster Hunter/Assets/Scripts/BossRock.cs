using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Enemy
{
    private Rigidbody _rockRigid;
    private float _angularPower = 2;
    private float _scaleValue = 0.1f;
    private bool _isShoot;
    private void Awake()
    {
        _rockRigid = GetComponent<Rigidbody>();
        StartCoroutine(GetBigger());
    }
    public override void Attack()
    {
        // none
    }

    public override void Died()
    {
        // none
    }

    private void Update()
    {
        _rockRigid.AddTorque(transform.right * _angularPower, ForceMode.Acceleration);
    }
    IEnumerator GetBigger()
    {
        while (transform.localScale.x < 2)
        {
            _angularPower += 5f;
            _scaleValue += 0.03f;
            transform.localScale = Vector3.one * _scaleValue;
            _rockRigid.AddTorque(transform.right * _angularPower, ForceMode.Acceleration);
            yield return null;
        }
        _rockRigid.useGravity = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Ground") && !other.gameObject.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}
