using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : Item
{
    public int value = 50;
    private int _curPlayerHp;
    private int _maxPlayerHp;
    private void Awake()
    {
        player = GameObject.Find("Player");
    }
    private void Update()
    {
        _curPlayerHp = player.GetComponent<Player>().playerHp;
        _maxPlayerHp = player.GetComponent<Player>().playerMaxHp;
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            int healAmount = _maxPlayerHp - _curPlayerHp;
            GameManager.Instance.PlayHpSc();
            if (healAmount < value)
            {
                value = healAmount;
            }
            player.GetComponent<Player>().playerHp += value;
            gameObject.SetActive(false);
        }
    }
}
