
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 15f;

    private PlayerStats stats;

    public void Initialize(PlayerStats playerStats)
    {
        stats = playerStats;
    }

    private void Update()
    {
        transform.position +=
            transform.forward * speed * Time.deltaTime;
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

        bool esEnemigo =
            other.CompareTag("Enemy") ||
            (other.transform.parent != null &&
             other.transform.parent.CompareTag("Enemy"));

        if (esEnemigo)
        {
            IHealthSystem health =
                other.GetComponent<IHealthSystem>() ??
                other.GetComponentInParent<IHealthSystem>();

            if (health != null)
            {
                int damage = stats != null
                    ? stats.GetDamage()
                    : 0;

                health.GetHurt(damage);
            }

            Debug.Log("¡Impacto confirmado contra el CUERPO del enemigo!");

            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}
