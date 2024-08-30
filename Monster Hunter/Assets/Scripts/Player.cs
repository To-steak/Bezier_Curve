using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10f;
    public float jumpPower = 15f;
    public int playerMaxHp;
    public int playerHp;
    public int coinAmount;
    public int remainedAmmo;

    public GameObject[] weapons; // 무기들 나열하는 배열

    public AudioSource audioSource;
    
    public AudioClip dodgeSc;
    public AudioClip jumpSc;
    public AudioClip getDamageSc;
    
    public Vector3 lookVector;
    
    private int _currentWeaponIndex = -1;

    private float _attackSpeed;
    
    private bool _isWorldBorder;
    private bool _isGrounded;
    private bool _isJumped;
    private bool _isDodged;
    private bool _isSwapped;
    private bool _isAttackReady = true;
    private bool _isRolling;
    private bool _isReload;
    private bool _isDamaged;
    private bool _isDied;

    private Animator _animator;
    
    private Rigidbody _rigidbody;

    private Weapon _equipWeapon;

    private MeshRenderer[] _meshRenderer;
    
    private Vector3 _moveVector;
    private Vector3 _dodgeVector;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _meshRenderer = GetComponentsInChildren<MeshRenderer>();
    }
    void Update()
    {
        if (!_isDied)
        {
            Move();
            RotatePlayer();
            Jump();
            Dodge();
            ChangeWeapon();
            Attack();
            Reload();
        }
    }
    private void FixedUpdate()
    {
        WorldBorder();
    }
    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        _moveVector = _isDodged ? _dodgeVector : new Vector3(h, 0, v).normalized;
        if (_isSwapped || !_isAttackReady || _isReload)
            _moveVector = Vector3.zero;
        transform.position += _moveVector * (speed * (_isWorldBorder ? 0:1) * Time.deltaTime);
        
        _animator.SetBool("isRun", _moveVector != Vector3.zero);
    }
    private void RotatePlayer()
    {
        if (Input.GetButton("Fire1"))
        {
            if (_isDodged || !_isAttackReady)
                return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, 1000))
            {
                lookVector = raycastHit.point - transform.position;
                lookVector.y = 0;
                transform.LookAt(transform.position + lookVector);
            }
        }
        else
        {
            transform.LookAt(transform.position + _moveVector);
        }
    }
    private void WorldBorder()
    {
        _isWorldBorder = Physics.Raycast(transform.position, _moveVector, 3, LayerMask.GetMask("World Border"));
    }
    private void Jump()
    {
        Vector3 playerJumpVector = transform.position;
        playerJumpVector.y += 1;
        _isGrounded = Physics.Raycast(playerJumpVector, Vector3.down, 1.5f, LayerMask.GetMask("Ground"));
        if (Input.GetButton("Jump") && _isGrounded && !_isJumped) {
            if (_isDodged || _isReload)
                return;
            _rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            _animator.SetBool("isJump", true);
            audioSource.PlayOneShot(jumpSc);;
            _animator.SetTrigger("doJump");
            _isJumped = true;
        }
    }
    private void Dodge()
    {
        if (Input.GetButton("Dodge"))
        {
            if (_isJumped || _isDodged || _moveVector == Vector3.zero || _isReload)
                return;
            _isDodged = true;
            _dodgeVector = _moveVector;
            speed *= 2f;
            audioSource.PlayOneShot(dodgeSc);
            _animator.SetTrigger("doDodge");

            Invoke("DodgeOut", 0.6f);
        }
    }
    private void DodgeOut()
    {
        speed *= 0.5f;
        _isDodged = false;
        _isRolling = true;
        Invoke("RollingOut", 0.1f);
    }
    private void RollingOut()
    {
        _isRolling = false;
    }
    private void ChangeWeapon()
    {
        int weaponIndex = -1;
        if (Input.GetButton("Hammer"))
            weaponIndex = 0;
        if (Input.GetButton("Gun"))
            weaponIndex = 1;
        
        if (Input.GetButton("Hammer") || Input.GetButton("Gun"))
        {
            if (_isJumped || _isDodged || _isSwapped || _isReload)
                return;
            if (_currentWeaponIndex == weaponIndex)
                return;
            Swap(weaponIndex);
        }
    }
    private void Swap(int weaponIndex)
    {
        if (_equipWeapon != null) // 손에 아무것도 없는 경우
        {
            _equipWeapon.gameObject.SetActive(false);
        }
        _currentWeaponIndex = weaponIndex;
        _equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
        _equipWeapon.gameObject.SetActive(true);
        _animator.SetTrigger("doSwap");
        _isSwapped = true;
        Invoke("SwapOut", 0.5f);
    }
    private void SwapOut()
    {
        _isSwapped = false;
    }
    private void Attack()
    {
        if (_equipWeapon == null || _isDodged || _isSwapped || _isReload)
            return;
        _attackSpeed += Time.deltaTime;
        _isAttackReady = _equipWeapon.attackSpeed < _attackSpeed;
        if (Input.GetButton("Fire1") && _isAttackReady)
        {
            _equipWeapon.Use();
            _attackSpeed = 0;
        }
    }
    public bool GetIsGrounded()
    {
        return _isGrounded;
    }
    public bool GetIsRolling()
    {
        return _isRolling;
    }
    public Weapon GetEquipWeapon()
    {
        return _equipWeapon;
    }
    public void SetIsReload(bool tmp)
    {
        _isReload = tmp;
    }
    public void SetRemainedBullet(int tmp)
    {
        remainedAmmo = tmp;
    }
    private void Reload()
    {
        if (Input.GetButton("Reload"))
        {
            if (_isDodged || _isJumped || _isSwapped || _isReload)
                return;
            _isReload = true;
            _equipWeapon.Reload();
        }
    }
    IEnumerator OnDamage()
    {
        _isDamaged = true;
        audioSource.PlayOneShot(getDamageSc);
        if (playerHp <= 0)
        {
            _isDied = true;
            _animator.SetTrigger("doDied");
            Invoke("DiedOut", 2.5f);
            yield break;
        }
        foreach (MeshRenderer mesh in _meshRenderer)
            mesh.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        foreach (MeshRenderer mesh in _meshRenderer)
            mesh.material.color = Color.white;
        yield return new WaitForSeconds(0.9f);
        _isDamaged = false;
    }

    private void DiedOut()
    {
        GameManager.Instance.GameFailed();
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _animator.SetBool("isJump", false);
            _isJumped = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyAttack"))
        {
            if (_isDamaged)
                return;
            if (playerHp < other.gameObject.GetComponentInParent<Enemy>().attackDamage)
            {
                playerHp = 0;
            }
            else
            {
                playerHp -= other.gameObject.GetComponentInParent<Enemy>().attackDamage;
            }
            StartCoroutine(OnDamage());
        }
    }
}