using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Boss : Enemy
{
    public GameObject enemyMissile;
    public GameObject enemyRock;

    public Transform missilePivotA;
    public Transform missilePivotB;

    private Vector3 _lookVector;
    private Material _enemyMaterial;

    public AudioClip tauntSc;
    public AudioClip missileSc;
    public AudioClip rockSc;
    public AudioClip deidSc;
    
    private const float MissileSpeed = 60f;
    private const int TauntDamage = 60;
    private void Awake()
    {
        enemyRigid = GetComponent<Rigidbody>();
        enemyBoxCollider = GetComponent<BoxCollider>();
        _enemyMaterial = GetComponentInChildren<MeshRenderer>().material;
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponentInChildren<Animator>();
        target = GameObject.Find("Player");

        isWalk = true;
        isDied = false;
        isAttack = false;
        isDamaged = false;
        
        Attack();
    }
    private void Update()
    {
        LookPlayer();
    }

    private void LookPlayer()
    {
        if (isDied)
        {
            return;
        }
        _lookVector = target.transform.position;
        _lookVector.y = 0;
        transform.LookAt(_lookVector);
    }
    public override void Attack()
    {
        if (isDied)
        {
            return;
        }
        StartCoroutine(AttackIE());
    }
    public override void Died()
    {
        isDied = true;
        _enemyMaterial.color = Color.gray;
        gameObject.layer = 12;
        isWalk = false;
        navMeshAgent.enabled = false;
        enemyAnimator.SetTrigger("doDied");
        audioSource.PlayOneShot(deidSc);
        Invoke("DiedOut", 4f);
    }
    private void DiedOut()
    {
        gameObject.SetActive(false);
        GameManager.Instance.isBossDied = true;
    }
    IEnumerator AttackIE()
    {
        yield return new WaitForSeconds(2f);
        int attackType = Random.Range(0, 3);
        switch (attackType)
        {
            case 0:
                StartCoroutine(MissileShot());
                break;
            case 1:
                StartCoroutine(BigShot());
                break;
            case 2:
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        enemyAnimator.SetTrigger("doShot");
        audioSource.PlayOneShot(missileSc);
        yield return new WaitForSeconds(0.2f);
        
        GameObject missileA = Instantiate(enemyMissile, missilePivotA.position, transform.rotation);
        Rigidbody missileRigidA = missileA.GetComponent<Rigidbody>();
        missileRigidA.velocity = transform.forward * MissileSpeed;
        yield return new WaitForSeconds(0.2f);
        
        GameObject missileB = Instantiate(enemyMissile, missilePivotB.position, transform.rotation);
        Rigidbody missileRigidB = missileB.GetComponent<Rigidbody>();
        missileRigidB.velocity = transform.forward * MissileSpeed;
        yield return new WaitForSeconds(2.5f);
        
        Attack();
    }

    IEnumerator BigShot()
    {
        enemyAnimator.SetTrigger("doBigShot");
        audioSource.PlayOneShot(rockSc);
        GameObject rock = Instantiate(enemyRock, transform.position, transform.rotation);
        yield return new WaitForSeconds(2.5f);
        Attack();
    }

    IEnumerator Taunt()
    {
        enemyAnimator.SetTrigger("doTaunt");
        
        yield return new WaitForSeconds(1.6f);
        audioSource.PlayOneShot(tauntSc);
        if (target.GetComponent<Player>().GetIsGrounded())
        {
            target.GetComponent<Player>().playerHp -= TauntDamage;
        }
        yield return new WaitForSeconds(.7f);
        Attack();
    }
    IEnumerator OnDamage()
    {
        isDamaged = true;
        audioSource.PlayOneShot(hitSc);
        if (enemyCurrentHp <= 0 && !isDied)
        {
            StopAllCoroutines();
            Died();
            yield break;
        }
        _enemyMaterial.color = Color.red;
        
        yield return new WaitForSeconds(0.1f);
        _enemyMaterial.color = Color.white;
        
        yield return new WaitForSeconds(0.2f);
        isDamaged = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isDied)
            return;
        if (other.gameObject.CompareTag("Weapon"))
        {
            if (isDamaged)
                return;
            Weapon weapon = other.GetComponent<Weapon>();
            enemyCurrentHp -= weapon.damage;
            StartCoroutine(OnDamage());
        }
        if (other.gameObject.CompareTag("Bullet"))
        {
            StartCoroutine(OnDamage());
        }
    }
}
