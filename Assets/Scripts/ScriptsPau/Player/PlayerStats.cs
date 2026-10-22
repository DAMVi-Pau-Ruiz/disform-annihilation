
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IHealthSystem
{
    [Header("Health")]
    [SerializeField] private int totalHealth = 10;
    [SerializeField] private float invincibleTime = 1f;

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 10f;
    [SerializeField] private float slowMultiplier = 0.5f;

    [Header("Combat")]
    [SerializeField] private int damage = 5;
    [SerializeField] private float bulletsPerSecond = 2f;

    private float currentHealth;
    private bool isInvincible;

    private void Awake()
    {
        currentHealth = totalHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetTotalHealth()
    {
        return totalHealth;
    }

    public float GetBaseSpeed()
    {
        return baseSpeed;
    }

    public float GetSlowSpeed()
    {
        return baseSpeed * slowMultiplier;
    }

    public int GetDamage()
    {
        return damage;
    }

    public float GetBulletsPerSecond()
    {
        return bulletsPerSecond;
    }

    public void GetHurt(float quantity)
    {
        if (isInvincible || currentHealth <= 0)
        {
            return;
        }

        currentHealth -= quantity;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log("Te has hecho daño. Vida: " + currentHealth);

        if (currentHealth > 0)
        {
            StartCoroutine(CooldownCoroutine());
        }
        else
        {
            // Aquí puedes gestionar la muerte del jugador.
        }
    }

    public void AddHealth(float health)
    {
        currentHealth = Mathf.Clamp(
            currentHealth + health,
            0,
            totalHealth
        );
    }

    private IEnumerator CooldownCoroutine()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;
    }
}
