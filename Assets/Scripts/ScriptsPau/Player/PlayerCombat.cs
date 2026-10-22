
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Inputs")]
    [SerializeField] private InputActionReference shoot;

    private PlayerStats stats;
    private float nextFireTime;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        OnShoot();
    }

    private void OnShoot()
    {
        if (stats == null || shoot == null || shoot.action == null)
        {
            return;
        }

        float bulletsPerSecond = stats.GetBulletsPerSecond();

        if (shoot.action.IsPressed() &&
            bulletsPerSecond > 0f &&
            Time.time >= nextFireTime)
        {
            Shoot();

            nextFireTime = Time.time + (1f / bulletsPerSecond);
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            return;
        }

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        PlayerProjectile projectileScript =
            projectile.GetComponent<PlayerProjectile>();

        if (projectileScript != null)
        {
            projectileScript.Initialize(stats);
        }
    }
}
