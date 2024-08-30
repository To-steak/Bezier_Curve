using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterLevel : MonoBehaviour
{
    public int level;
    private void Update()
    {

        if (GameManager.Instance.MissionClear(level))
        {
            GameManager.Instance.MissionClear(level);
            gameObject.layer = 0;
            gameObject.GetComponent<SphereCollider>().isTrigger = true;
        }

        if (GameManager.Instance.MissionClear(level))
        {
            GameManager.Instance.MissionClear(level);
            gameObject.layer = 0;
            gameObject.GetComponent<SphereCollider>().isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.EnterLevel(level);
        }
    }
}
