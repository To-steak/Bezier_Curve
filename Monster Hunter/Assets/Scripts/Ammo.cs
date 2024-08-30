using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : Item
{
    public int value = 30;
    private void Awake()
    {
        player = GameObject.Find("Player");
    }
    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.PlayAmmoSc();
            player.GetComponent<Player>().remainedAmmo += value;
            gameObject.SetActive(false);
        }
    }
}
