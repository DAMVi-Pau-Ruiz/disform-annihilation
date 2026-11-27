
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Divide_Enemy_Movement : MonoBehaviour
{
    [Header("Configuración de división")]
    [SerializeField] private float tiempoDivision = 5f;
    [SerializeField] private GameObject prefabEnemigo;

    [Header("Combate")]
    [SerializeField] private float damage = 1f;

    private bool hasDivided = false;
    private bool playerInContact = false;

    private Enemy_Controller enemyController;
    private NavMeshAgent agent;
    private Transform playerTransform;

    private void Awake()
    {
        enemyController = GetComponent<Enemy_Controller>();
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    private void Start()
    {
        StartCoroutine(TemporizadorDivision());
    }

    private void Update()
    {
        if (enemyController == null || enemyController.IsDead())
            return;

        // Movimiento hacia el jugador.
        if (agent != null &&
            agent.isOnNavMesh &&
            playerTransform != null)
        {
            agent.SetDestination(playerTransform.position);
        }
    }

    private IEnumerator TemporizadorDivision()
    {
        yield return new WaitForSeconds(tiempoDivision);

        if (enemyController == null || enemyController.IsDead())
            yield break;

        Dividirse();
    }

    private void Dividirse()
    {
        if (prefabEnemigo == null || hasDivided)
        {
            Debug.LogError("No se ha asignado el prefab del enemigo divisor.");
            return;
        }

        Vector3 posicion = transform.position;

        Vector3 posicionIzquierda = posicion + Vector3.left * 0.6f;
        Vector3 posicionDerecha = posicion + Vector3.right * 0.6f;

        Instantiate(prefabEnemigo, posicionIzquierda, transform.rotation);
        Instantiate(prefabEnemigo, posicionDerecha, transform.rotation);

        hasDivided = true;

        enemyController.DesaparecerPorDivision();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerInContact)
            return;

        // Comprobamos si el collider pertenece al jugador.
        Transform jugador = other.transform;

        while (jugador != null && !jugador.CompareTag("Player"))
        {
            jugador = jugador.parent;
        }

        if (jugador == null)
            return;

        IHealthSystem playerHealth =
            jugador.GetComponent<IHealthSystem>() ??
            jugador.GetComponentInParent<IHealthSystem>();

        if (playerHealth == null)
        {
            Debug.LogWarning("No se ha encontrado IHealthSystem en el jugador.");
            return;
        }

        playerInContact = true;

        playerHealth.GetHurt(damage);

        Debug.Log("Enemigo divisor ha hecho daño al jugador: " + damage);
    }

    private void OnTriggerExit(Collider other)
    {
        Transform jugador = other.transform;

        while (jugador != null && !jugador.CompareTag("Player"))
        {
            jugador = jugador.parent;
        }

        if (jugador != null)
        {
            playerInContact = false;
        }
    }
}
