using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject boss;
    
    public TMP_Text ammoText;
    public TMP_Text coinText;
    public TMP_Text hpText;
    public TMP_Text bossHpText;
    public TMP_Text monsterCountText;
    public TMP_Text missionText;

    public GameObject menuPanel;
    public GameObject userInterface;
    public GameObject bossGroup;
    public GameObject clearGroup;
    public GameObject failedGroup;
    public GameObject QuitGroup;
    

    public RectTransform bossGauge;
    public RectTransform hpGroup;
    public RectTransform hpGauge;

    public GameObject[] monsterSpawner;
    public GameObject[] items;

    public int monsterCount = 0;
    public int totalCount = 0;
    
    public bool isBossDied;
    public bool isEnd;

    public AudioSource audioSource;
    public AudioSource bgm;
    public AudioSource bossBgm;
    
    public AudioClip coinSc;
    public AudioClip hpSc;
    public AudioClip ammoSc;
    public AudioClip successSc;
    public AudioClip failSc;
    
    private int _coinAmount;
    private int _playerMaxHp;
    private int _playerHp;
    private int _bossMaxHp;
    private int _bossHp;
    private Weapon _playerWeapon;
    private bool _empty;
    private bool _isEnterLevel2;
    private static GameManager instance;
    
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        _playerMaxHp = player.GetComponent<Player>().playerMaxHp;
        _bossMaxHp = boss.GetComponent<Enemy>().enemyMaxHp;
    }
    private void Update()
    {
        UpdateAmmo();
        UpdateCoin();
        UpdateHp();
        UpdateBossHp();
        UpdateMonsterCount();
        if (isBossDied && !isEnd)
        {
            GameClear();
            isEnd = true;
        }
    }
    public void GameStart()
    {
        menuPanel.SetActive(false);
        userInterface.SetActive(true);
        player.SetActive(true);
        monsterSpawner[0].SetActive(true);
        bgm.Play();
    }
    private void UpdateAmmo()
    {
        _playerWeapon = player.GetComponent<Player>().GetEquipWeapon();
        int remainedBullet = player.GetComponent<Player>().remainedAmmo;
        int curWeaponBullet = -1;
        if(_playerWeapon is not null)
        {
            curWeaponBullet = _playerWeapon.GetCurAmmo();
        }
        if (_playerWeapon is null || curWeaponBullet < 0)
        {
            _empty = true;
        }
        else
        {
            _empty = false;
        } 
        ammoText.text = (_empty ? "-" : curWeaponBullet) + (" / " + remainedBullet);
    }
    private void UpdateCoin()
    {
        _coinAmount = player.GetComponent<Player>().coinAmount;
        coinText.text = _coinAmount.ToString();
    }
    private void UpdateHp()
    {
        _playerHp = player.GetComponent<Player>().playerHp;
        hpGauge.localScale = new Vector3((float)_playerHp / _playerMaxHp, 1, 1);
        hpText.text = _playerHp + " / " + _playerMaxHp;
    }
    private void UpdateBossHp()
    {
        _bossHp = boss.GetComponent<Enemy>().enemyCurrentHp;
        if (_bossHp < 0)
        {
            _bossHp = 0;
        }
        bossGauge.localScale = new Vector3((float)_bossHp / _bossMaxHp, 1, 1);
        bossHpText.text = _bossHp + " / " + _bossMaxHp;
    }

    private void UpdateMonsterCount()
    {
        monsterCountText.text = "Monster: " + monsterCount + " / 30" + "\nTotal kill Count: " + totalCount;
    }

    public void MissionUpdate(int level)
    {
        if (level == 1)
        {
            missionText.text = "- Mission -\n Kill enemy 45";
        }

        if (level == 2)
        {
            missionText.text = "- Mission -\n Get the boss";
        }
    }

    public bool MissionClear(int level)
    {
        switch (level)
        {
            case 1 when player.GetComponent<Player>().coinAmount >= 30:
            case 2 when totalCount >= 45:
                return true;
            default:
                return false;
        }
    }
    public void DropItem(Vector3 position)
    {
        int random = Random.Range(0, items.Length);
        GameObject item = items[random];
        int dropChance = Random.Range(0, 10);
        position += Vector3.up * 1.5f;
        if (dropChance < 8) // 80%
        {
            Instantiate(item, position, Quaternion.identity);
        }
    }
    public void EnterLevel(int level)
    {
        if (level is -1)
            return;
        if (level is 1)
        {
            MissionUpdate(level);
        }
        if (level is 2)
        {
            bossGroup.SetActive(true);
            boss.SetActive(true);
            bgm.Stop();
            bossBgm.Play();
        }
        monsterSpawner[level].SetActive(true);
        monsterSpawner[level-1].SetActive(false);
        monsterSpawner[level-1].GetComponent<MonsterSpawner>().OnDisable();
    }
    public void TitleButton()
    {
        SceneManager.LoadScene(0);
    }
    public void GameClear()
    {
        clearGroup.SetActive(true);
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.StopAllCoroutines();
            enemy.Died();
        }
        foreach (var spawner in monsterSpawner)
        {
            spawner.GetComponent<MonsterSpawner>().OnDisable();
        }
        bossBgm.Stop();
        audioSource.PlayOneShot(successSc);
    }
    public void GameFailed()
    {
        failedGroup.SetActive(true);
        bossBgm.Stop();
        bgm.Stop();
        audioSource.PlayOneShot(failSc);
    }
    
    public void QuitButton()
    {
        QuitGroup.SetActive(true);
    }
    public void IsQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void IsNoButton()
    {
        QuitGroup.SetActive(false);
    }

    public void PlayCoinSc()
    {
        audioSource.PlayOneShot(coinSc);
    }
    public void PlayAmmoSc()
    {
        audioSource.PlayOneShot(ammoSc);
    }
    public void PlayHpSc()
    {
        audioSource.PlayOneShot(hpSc);
    }
}
