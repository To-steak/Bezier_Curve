using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    public int enemyMaxHp;
    public int enemyCurrentHp;
    public int attackDamage;
    
    public Rigidbody enemyRigid;
    
    public BoxCollider enemyHitBox;
    public BoxCollider enemyBoxCollider;
    
    public NavMeshAgent navMeshAgent;
    
    public GameObject target;
    public Animator enemyAnimator;

    public AudioSource audioSource;
    public AudioClip hitSc;
    
    public bool isWalk;
    public bool isDied;
    public bool isAttack;
    public bool isDamaged;
    public abstract void Attack();
    public abstract void Died();
    protected void DropItem()
    {
        GameManager.Instance.DropItem(gameObject.transform.position);
    }
    
}
