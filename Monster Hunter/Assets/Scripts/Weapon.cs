using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public abstract class Weapon : MonoBehaviour
{
    public int damage;
    public float attackSpeed;

    public abstract void Use();
    public abstract void Reload();
    public abstract int GetCurAmmo();
}
