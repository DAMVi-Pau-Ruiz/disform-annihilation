using UnityEngine;
using System;

[Serializable]
public class PlayerSaveData : SaveData
{
    public Vector3 position;

    public float currentHealth;

    public float damage;
    public float damageMultiplier;
    public float bulletsPerSecond;
    public float bulletSpeed;
}