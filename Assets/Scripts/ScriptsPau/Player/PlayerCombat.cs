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

    private int currentHealth;
    private float nextFireTime;

    private void Awake()
    {
        currentHealth = totalHealth;
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
        if (currentHealth > 0)
        {
            currentHealth -= quantity;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                //muere
            }
        }
    }
}
