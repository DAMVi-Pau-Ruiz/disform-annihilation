
using UnityEngine;
using UnityEngine.AI;

public class Normal_Enemy_Movement : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private float damage = 1f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.3f;

    private Transform playerTransform;
    private NavMeshAgent agent;
    private Enemy_Controller enemyController;

    private bool isKnockedBack;
    private bool playerInContact;

    private float knockbackTimer;
    private Vector3 knockbackDirection;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyController = GetComponent<Enemy_Controller>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    private void Update()
    {
        if (agent == null || enemyController == null)
        {
            return;
        }

        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;

            if (agent.isOnNavMesh)
            {
                agent.Move(
                    knockbackDirection *
                    knockbackForce *
                    Time.deltaTime
                );
            }

            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
                agent.ResetPath();
            }

            return;
        }

        if (playerTransform != null && agent.isOnNavMesh)
        {
            agent.SetDestination(playerTransform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || playerInContact)
        {
            return;
        }

        if (enemyController == null)
        {
            return;
        }

        playerInContact = true;

        // El enemigo recibe daño.
        enemyController.GetHurt(damage);

        // Si muere, no continuamos con el choque.
        if (enemyController.GetCurrentHealth() <= 0)
        {
            return;
        }

        // El jugador recibe daño.
        IHealthSystem playerHealth =
            other.GetComponent<IHealthSystem>()
            ?? other.GetComponentInParent<IHealthSystem>();

        if (playerHealth != null)
        {
            playerHealth.GetHurt(damage);
        }

        // Dirección desde el jugador hacia el enemigo.
        knockbackDirection =
            transform.position - other.transform.position;

        knockbackDirection.y = 0f;

        if (knockbackDirection.sqrMagnitude > 0.001f)
        {
            knockbackDirection.Normalize();
        }
        else
        {
            knockbackDirection = -transform.forward;
        }

        // Activamos el retroceso.
        knockbackTimer = knockbackDuration;
        isKnockedBack = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInContact = false;
        }
    }
}
