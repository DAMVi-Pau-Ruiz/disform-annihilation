using UnityEngine;
using UnityEngine.AI;

public class Normal_Enemy_Movement : MonoBehaviour
{
    [Header("Daño por choque")]
    [SerializeField] private int dañoPorChoque = 1;

    [Header("Empujón")]
    [SerializeField] private float fuerzaEmpujon = 3f;
    [SerializeField] private float duracionEmpujon = 0.25f;

    private Transform playerTransform;
    private NavMeshAgent agent;
    private Enemy_Controller enemyController;

    private Vector3 direccionEmpujon;
    private float tiempoEmpujonRestante;
    private bool recibiendoEmpujon;
    private bool choqueProcesado;

    private void Awake()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }

        agent = GetComponent<NavMeshAgent>();
        enemyController = GetComponent<Enemy_Controller>();
    }

    private void Update()
    {
        if (enemyController == null)
        {
            return;
        }

        if (recibiendoEmpujon)
        {
            tiempoEmpujonRestante -= Time.deltaTime;

            if (agent != null && agent.isOnNavMesh)
            {
                agent.Move(direccionEmpujon * fuerzaEmpujon * Time.deltaTime);
            }

            if (tiempoEmpujonRestante <= 0f)
            {
                recibiendoEmpujon = false;
                choqueProcesado = false;
            }

            return;
        }

        if (playerTransform != null && agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(playerTransform.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            RecibirChoque(collision.transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RecibirChoque(other.transform);
        }
    }

    private void RecibirChoque(Transform jugador)
    {
        if (recibiendoEmpujon || choqueProcesado || enemyController == null)
        {
            return;
        }

        enemyController.GetHurt(dañoPorChoque);

        if (enemyController.GetCurrentHealth() <= 0)
        {
            return;
        }

        direccionEmpujon = transform.position - jugador.position;
        direccionEmpujon.y = 0f;

        if (direccionEmpujon.sqrMagnitude > 0.001f)
        {
            direccionEmpujon.Normalize();
        }
        else
        {
            direccionEmpujon = -transform.forward;
        }

        tiempoEmpujonRestante = duracionEmpujon;
        recibiendoEmpujon = true;
        choqueProcesado = true;
    }
}