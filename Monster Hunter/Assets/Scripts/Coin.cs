using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item
{
    public int value;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }
    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.PlayCoinSc();
            player.GetComponent<Player>().coinAmount += (1 * value);
            gameObject.SetActive(false);
        }
    }
}
