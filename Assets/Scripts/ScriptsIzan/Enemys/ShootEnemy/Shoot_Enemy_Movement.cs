using UnityEngine;
using UnityEngine.AI;

public class Shoot_Enemy_Movement : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el objeto GameObject/Anillo que hace de Spawner")]
    public Transform balaSpawner;

    [Header("Configuración de Disparo")]
    [Tooltip("Prefab de la bala que se va a disparar")]
    public GameObject prefabBala;
    [Tooltip("El radio físico de tu objeto Spawner (distancia del centro al borde)")]
    public float radioDelSpawner = 1.5f;
    [Tooltip("Cantidad de balas disparadas por segundo")]
    [SerializeField] private float balasPorSegundo = 2f;
    [Tooltip("Velocidad de movimiento de la bala")]
    public float velocidadBala = 15f;
    [Tooltip("Daño que inflige la bala del enemigo")]
    public int danoBala = 1;

    private Transform playerTransform;
    private NavMeshAgent agent;
    private float cronometroDisparo;

    private void Awake()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("No se detecta el objeto con el tag 'Player'");
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        cronometroDisparo = ObtenerIntervaloDisparo();
    }

    void Update()
    {
        if (playerTransform != null)
        {
            RotarEnemigo();

            if (agent != null && agent.isStopped)
            {
                ControlarDisparo();
            }
            else
            {
                cronometroDisparo = ObtenerIntervaloDisparo();
            }
        }

        if (playerTransform != null && agent != null && agent.isOnNavMesh && !agent.isStopped)
        {
            agent.SetDestination(playerTransform.position);
        }
    }

    private void RotarEnemigo()
    {
        Vector3 direccionEnemigo = playerTransform.position - transform.position;
        direccionEnemigo.y = 0;
        if (direccionEnemigo != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direccionEnemigo);
        }
    }

    private void ControlarDisparo()
    {
        if (balasPorSegundo <= 0f)
        {
            return;
        }

        cronometroDisparo -= Time.deltaTime;

        if (cronometroDisparo <= 0)
        {
            DispararDesdeSuperficieSpawner();
            cronometroDisparo = ObtenerIntervaloDisparo();
        }
    }

    void DispararDesdeSuperficieSpawner()
    {
        if (balaSpawner == null)
        {
            Debug.LogError("¡No has asignado el 'Bala Spawner' en el Inspector!");
            return;
        }

        GameObject balaNueva = Instantiate(prefabBala, balaSpawner.position, balaSpawner.rotation);

        BalaComportamiento scriptBala = balaNueva.GetComponent<BalaComportamiento>();
        if (scriptBala != null)
        {
            // CAMBIO: Añadimos 'gameObject' al final para decirle a la bala "yo soy tu creador"
            scriptBala.InicializarBala(velocidadBala, danoBala, "Player", gameObject);
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (balaSpawner != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(balaSpawner.position, radioDelSpawner);
        }
    }

    // CAMBIO: Ahora son funciones públicas creadas expresamente para que las llame el hijo
    public void ProcesarTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            Debug.Log("Jugador detectado por el hijo: Deteniendo enemigo e iniciando temporizador de disparo.");
        }
    }

    public void ProcesarTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && agent != null)
        {
            agent.isStopped = false;
            Debug.Log("Jugador fuera de rango del hijo: Reanudando persecución y pausando disparos.");
        }
    }

    private float ObtenerIntervaloDisparo()
    {
        if (balasPorSegundo <= 0f)
        {
            return 0f;
        }

        return 1f / balasPorSegundo;
    }

}
