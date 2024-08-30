using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;

    public List<GameObject> pool;
    private void Start()
    {
        pool = new List<GameObject>();
        for (int i = 0; i < 21; i++)
        {
            
            int index = Random.Range(0, enemyPrefabs.Length);
            GameObject obj = Instantiate(enemyPrefabs[index]);
            obj.SetActive(false);
            pool.Add(obj);
        }
        InvokeRepeating("Spawn", 0f, 5f);
    }

    private GameObject GetObjPool()
    {
        int index = Random.Range(0, enemyPrefabs.Length);
        if (!pool[index].activeInHierarchy)
        { 
            return pool[index];
        }
        
        GameObject newObj = Instantiate(enemyPrefabs[index]);
        pool.Add(newObj);
        return newObj;
    }

    private void Spawn()
    {
        if (GameManager.Instance.monsterCount >= 30)
            return;
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject mob = GetObjPool();
        mob.SetActive(true);
        mob.transform.position = spawnPoint.position;
        GameManager.Instance.monsterCount++;
    }
    public void OnDisable()
    {
        CancelInvoke("Spawn");
    }
}
