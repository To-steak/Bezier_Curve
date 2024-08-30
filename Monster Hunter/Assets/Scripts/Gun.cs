using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gun : Weapon
{
    public Transform bulletCasePivot;
    public Transform bulletPivot;
    
    public GameObject bulletPrefab;
    public GameObject bulletCasePrefab;

    public List<GameObject> bulletPool = new List<GameObject>();
    public List<GameObject> bulletCasePool = new List<GameObject>();
    
    public AudioSource audioSource;
    
    public AudioClip fireSource;
    public AudioClip reloadSource;
    public AudioClip emptySource;
    
    private GameObject _instantBullet;
    private GameObject _instantBulletCase;
    
    private Rigidbody _bulletRigidbody;
    private Rigidbody _bulletCaseRigid;
        
    private Player _player;
    
    private int _remainedAmmo;
    private const int MaxAmmo = 30;
    private int _curAmmo = 30;
    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    private void Start()
    {
        for (int i = 0; i < MaxAmmo; i++)
        {
            GameObject bulletObj = Instantiate(bulletPrefab);
            GameObject bulletCaseObj = Instantiate(bulletCasePrefab);
            
            bulletObj.SetActive(false);
            bulletCaseObj.SetActive(false);
            
            bulletCasePool.Add(bulletCaseObj);
            bulletPool.Add(bulletObj);
        }
    }
    private void Update()
    {
        _remainedAmmo = _player.remainedAmmo;
    }

    public GameObject GetBulletPoolObject()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (!bulletPool[i].activeInHierarchy)
            {
                return bulletPool[i];
            }
        }
        
        GameObject newObj = Instantiate(bulletPrefab);
        newObj.SetActive(false);
        bulletPool.Add(newObj);
        return newObj;
    }
    public GameObject GetBulletCasePooledObject()
    {
        for (int i = 0; i < bulletCasePool.Count; i++)
        {
            if (!bulletCasePool[i].activeInHierarchy)
            {
                return bulletCasePool[i];
            }
        }
        
        GameObject newObj = Instantiate(bulletCasePrefab);
        newObj.SetActive(false);
        bulletCasePool.Add(newObj);
        return newObj;
    }
    public override void Use()
    {
        _player.GetComponentInChildren<Animator>().SetTrigger("doShot");
        Shot();
    }

    public override void Reload()
    {
        if (_remainedAmmo > 0)
        {
            if (_curAmmo >= MaxAmmo)
                return;
            audioSource.PlayOneShot(reloadSource);
            _player.GetComponentInChildren<Animator>().SetTrigger("doReload");
            Invoke("ReloadOut", 2.8f);
        }
        else
        {
            _player.SetIsReload(false);
        }
    }

    private void ReloadOut()
    {
        int insertAmmo = MaxAmmo - _curAmmo; // 장전할 총알 수
        if (_remainedAmmo <= insertAmmo)
        {
            _curAmmo += _remainedAmmo;
            _remainedAmmo = 0;
            _player.SetRemainedBullet(0);
        }
        else
        {
            _curAmmo += insertAmmo;
            _player.SetRemainedBullet(_remainedAmmo - insertAmmo);
        }
        _player.SetIsReload(false);
    }
    private void Shot()
    {
        if (_curAmmo > 0)
        {
            audioSource.PlayOneShot(fireSource);
            _curAmmo--;
        }
        else
        {
            audioSource.PlayOneShot(emptySource);
            return;
        }

        _instantBullet = GetBulletPoolObject();
        _bulletRigidbody = _instantBullet.GetComponent<Rigidbody>();
        
        _instantBullet.transform.position = bulletPivot.position;
        _bulletRigidbody.velocity = bulletPivot.forward * 50f;
        _instantBullet.SetActive(true);
        
        _instantBulletCase = GetBulletCasePooledObject();
        _bulletCaseRigid = _instantBulletCase.GetComponent<Rigidbody>();
        Vector3 caseVector = bulletCasePivot.forward * Random.Range(-3f, -1f) + Vector3.up * Random.Range(2f, 3f);
        
        _instantBulletCase.transform.position = bulletCasePivot.position;
        _instantBulletCase.SetActive(true);
        
        _bulletCaseRigid.AddForce(caseVector, ForceMode.Impulse);
        _bulletCaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }
    public override int GetCurAmmo()
    {
        return _curAmmo;
    }
}
