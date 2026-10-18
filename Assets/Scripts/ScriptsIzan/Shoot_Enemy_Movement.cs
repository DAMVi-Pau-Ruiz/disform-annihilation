using UnityEngine;
using UnityEngine.AI;

public class Shoot_Enemy_Movement : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el objeto GameObject/Anillo que hace de Spawner")]
    public Transform balaSpawner;

    [Header("Configuración de Disparo")]
    [Tooltip("Prefab de la bala que se va a disparar")]
    public GameObject balaPrefab;
    [Tooltip("El radio físico de tu objeto Spawner (distancia del centro al borde)")]
    public float radioDelSpawner = 1.5f;
    [Tooltip("Tiempo en segundos entre cada disparo")]
    public float cadenciaDisparo = 2.0f; // Ajustado a 2 segundos por defecto
    [Tooltip("Velocidad de movimiento de la bala")]
    public float velocidadBala = 15f;

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
        // Inicializamos el cronómetro con el tiempo de espera
        cronometroDisparo = cadenciaDisparo;
    }

    void Update()
    {
        if (playerTransform != null)
        {
            RotarEnemigo();

            // CORRECCIÓN: El enemigo SOLO controla el tiempo y dispara si está detenido (jugador en rango)
            if (agent.isStopped)
            {
                ControlarDisparo();
            }
            else
            {
                // Opcional: Reinicia el cronómetro si el jugador escapa, 
                // para que cuando vuelva a entrar tenga que esperar 2 segundos completos antes del primer tiro.
                cronometroDisparo = cadenciaDisparo;
            }
        }

        if (playerTransform != null && agent.isOnNavMesh && !agent.isStopped)
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
        cronometroDisparo -= Time.deltaTime;

        if (cronometroDisparo <= 0)
        {
            DispararDesdeSuperficieSpawner();
            cronometroDisparo = cadenciaDisparo; // Se resetea a 2 segundos
        }
    }

    private void DispararDesdeSuperficieSpawner()
    {
        if (balaPrefab == null || playerTransform == null || balaSpawner == null) return;

        Vector3 direccionHaciaJugador = (playerTransform.position - balaSpawner.position).normalized;
        Vector3 posicionSalidaBala = balaSpawner.position + (direccionHaciaJugador * radioDelSpawner);
        Quaternion rotacionBala = Quaternion.LookRotation(direccionHaciaJugador);

        GameObject nuevaBala = Instantiate(balaPrefab, posicionSalidaBala, rotacionBala);

        Rigidbody rbBala = nuevaBala.GetComponent<Rigidbody>();
        if (rbBala != null)
        {
            rbBala.linearVelocity = direccionHaciaJugador * velocidadBala;
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

    // EVENTOS DE COLISIÓN (TRIGGERS)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            Debug.Log("Jugador detectado: Iniciando temporizador de disparo.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            agent.isStopped = false;
            Debug.Log("Jugador fuera de rango: Disparos pausados.");
        }
    }
}
