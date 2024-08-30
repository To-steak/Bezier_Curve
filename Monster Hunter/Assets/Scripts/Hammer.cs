using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : Weapon
{
    public BoxCollider hitBox;
    public TrailRenderer trailRenderer;
    
    public AudioSource swingSource;

    
    private Player _player;
    
    private bool _isAirAttack;
    private bool _isRollingAttack;
    
    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
    }
    public override void Reload()
    {
        _player.GetComponent<Player>().SetIsReload(false);
    }
    public override void Use()
    {
        _player.GetComponentInChildren<Animator>().SetTrigger("doSwing");
        if(!_player.GetIsGrounded() && !_isAirAttack)
        {
            StartCoroutine("AirAttack");
            return;
        }
        if (_player.GetIsRolling() && !_isRollingAttack)
        {
            StartCoroutine("RollingAttack");
            return;
        }
        StartCoroutine("Attack");
    }
    IEnumerator Attack()
    {
        trailRenderer.Clear();
        
        yield return new WaitForSeconds(0.2f);
        swingSource.Play();
        hitBox.enabled = true;
        trailRenderer.enabled = true;
        
        yield return new WaitForSeconds(0.4f);
        hitBox.enabled = false;
        trailRenderer.enabled = false;
    }
    IEnumerator AirAttack()
    {
        _isAirAttack = true;
        trailRenderer.Clear();
        yield return new WaitForSeconds(0.2f);
        
        swingSource.Play();
        hitBox.enabled = true;
        trailRenderer.enabled = true;
        float totalRotation = 0f;
        float rotationSpeed = 360 * 2f;
        while (totalRotation < 360f)
        {
            float rotationAmount = rotationSpeed * Time.deltaTime;
            _player.transform.Rotate(Vector3.right * rotationAmount);
            totalRotation += rotationAmount;
            yield return null;
        }
        _player.transform.rotation = Quaternion.Euler(0, 0, 0);
        trailRenderer.Clear();
        yield return new WaitForSeconds(0.4f);
        
        hitBox.enabled = false;
        trailRenderer.enabled = false;
        _isAirAttack = false;
    }
    IEnumerator RollingAttack()
    {
        _isRollingAttack = true;
        trailRenderer.Clear();
    
        yield return null;
        swingSource.Play();
        hitBox.enabled = true;
        trailRenderer.enabled = true;
        _player.transform.Rotate(Vector3.forward * 90);
        _player.transform.position += Vector3.up * 0.1f;
        float totalRotation = 0f;
        float rotationSpeed = 360f * 2;
        while (totalRotation < 360f)
        {
            float rotationAmount = rotationSpeed * Time.deltaTime;
            _player.transform.Rotate(Vector3.right * rotationAmount);
            totalRotation += rotationAmount;
            yield return null;
        }
        _player.transform.rotation = Quaternion.Euler(0, 0, 0);
        trailRenderer.Clear();
        yield return new WaitForSeconds(0.4f);
        
        hitBox.enabled = false;
        trailRenderer.enabled = false;
        _isRollingAttack = false;
    }
    public override int GetCurAmmo()
    {
        return -1;
    }
}