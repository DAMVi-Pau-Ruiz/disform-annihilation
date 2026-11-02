using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerStats))]
public class PlayerSaveable : SaveableObject
{
    [Header("Save Settings")]
    [SerializeField] private string id = "PLAYER";

    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    public override string GetID()
    {
        return id;
    }

    public override SaveData GetSaveData()
    {
        PlayerSaveData data = new PlayerSaveData();

        data.position = transform.position;

        data.currentHealth = playerStats.GetCurrentHealth();

        data.damage = playerStats.GetBaseDamage();
        data.damageMultiplier = playerStats.GetDamageMultiplier();
        data.bulletsPerSecond = playerStats.GetBulletsPerSecond();
        data.bulletSpeed = playerStats.GetBulletSpeed();

        return data;
    }

    public override void LoadData(SaveData saveData)
    {
        PlayerSaveData data = saveData as PlayerSaveData;

        if (data == null)
        {
            Debug.LogError("Los datos guardados no corresponden al Player.");
            return;
        }

        transform.position = data.position;

        playerStats.LoadSaveStats(
            data.currentHealth,
            data.damage,
            data.damageMultiplier,
            data.bulletsPerSecond,
            data.bulletSpeed
        );
    }
}