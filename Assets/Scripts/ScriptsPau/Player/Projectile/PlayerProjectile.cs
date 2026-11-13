
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    private PlayerStats stats;

    public void Initialize(PlayerStats playerStats)
    {
        stats = playerStats;
    }

    private void Update()
    {
        transform.position += transform.forward * stats.GetBulletSpeed() * Time.deltaTime;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vacio"))
        {
            return;
        }
        
        // Ignorar detectores que sean Trigger.
        if (other.isTrigger)
        {
            return;
        }

        // Ignorar proyectiles del boss.
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            return;
        }
        
        if (other.CompareTag("Player"))
        {
            return;
        }

        IHealthSystem health = other.GetComponent<IHealthSystem>() ?? other.GetComponentInParent<IHealthSystem>();

        if (health != null)
        {
            float damage = stats != null
                ? stats.GetTotalDamage()
                : 0;

            health.GetHurt(damage);
        }

        Destroy(gameObject);
    }
}
