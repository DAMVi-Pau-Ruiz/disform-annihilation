using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour, IHealthSystem
{
    [Header("Projectile")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float bulletsPerSecond = 2f;

    [Header("Health")]
    [SerializeField] private int totalHealth = 10;

    [Header("Inputs")]
    [SerializeField] private InputActionReference shoot;

    [Header("Cooldowns")]
    [SerializeField] private float invincibleTime = 1;
    
    private int currentHealth;
    private float nextFireTime;
    private bool isInvincible = false;

    private void Awake()
    {
        currentHealth = totalHealth;
        StartCoroutine(CooldownCoroutine());
    }

    private void Update()
    {
        OnShoot();
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            return;
        }

        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }

    private void OnShoot()
    {
        if (shoot == null || shoot.action == null)
        {
            return;
        }

        if (shoot.action.IsPressed() &&
            bulletsPerSecond > 0f &&
            Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + (1f / bulletsPerSecond);
        }
    }



    public void GetHurt(int quantity)
    {
        if (isInvincible)
        {
            return;
        }

        if (currentHealth > 0)
        {
            currentHealth -= quantity;
            StartCoroutine(CooldownCoroutine());
            Debug.Log("Te has hecho daño");

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                //vida actual es 0 => muere de forma epica
            }
        }
    }

    private IEnumerator CooldownCoroutine()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetTotalHealth()
    {
        return totalHealth;
    }

    public void AddHealth(int health)
    {
        currentHealth += health;
    }
}
