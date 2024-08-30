using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Quick : Enemy
{
    private Material _enemyMaterial;
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
    }
    private void Update()
    {
        Move();
        Attack();
    }

    private void Move()
    {
        if (navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(target.transform.position);
            enemyAnimator.SetBool("isWalk", true);
            navMeshAgent.isStopped = !isWalk;
        }
    }
    public override void Attack()
    {
        float radius = 1.5f;
        float range = 3f;

        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, radius, transform.forward, range,
            LayerMask.GetMask("Player"));
        if (raycastHits.Length > 0)
        {
            if (isAttack || isDied)
                return;
            StartCoroutine(AttackCo());
        }
    }

    IEnumerator AttackCo()
    {
        isWalk = false;
        isAttack = true;
        enemyAnimator.SetBool("isAttack", true);

        yield return new WaitForSeconds(0.1f);
        enemyHitBox.enabled = true;

        yield return null;
        enemyHitBox.enabled = false;

        yield return new WaitForSeconds(2f);
        isAttack = false;
        isWalk = true;
        enemyAnimator.SetBool("isAttack", false);
    }
    public override void Died()
    {
        isDied = true;
        StopAllCoroutines();
        _enemyMaterial.color = Color.gray;
        gameObject.layer = 12;
        isWalk = false;
        navMeshAgent.enabled = false;
        enemyAnimator.SetTrigger("doDied");
        GameManager.Instance.monsterCount--;
        GameManager.Instance.totalCount++;
        Invoke("DiedOut", 3f);
    }
    private void DiedOut()
    {
        gameObject.SetActive(false);
        DropItem();
        isDamaged = false;
        isAttack = false;
        isDied = false;
        _enemyMaterial.color = Color.white;
        gameObject.layer = 11;
        isWalk = true;
        navMeshAgent.enabled = true;
        enemyCurrentHp = enemyMaxHp;
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
}
