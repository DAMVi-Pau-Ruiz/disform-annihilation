using UnityEngine;
using UnityEngine.AI;

public class ShootGun_Enemy_Movement : MonoBehaviour, IEnemyAI
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el objeto GameObject/Anillo que hace de Spawner")]
    public Transform balaSpawner;

    [Header("Configuración de Disparo")]
    [Tooltip("Prefab de la bala que se va a disparar")]
    public GameObject prefabBala;
    [Tooltip("El radio físico de tu objeto Spawner (distancia del centro al borde)")]
    public float radioDelSpawner = 1.5f;
    [Tooltip("Tiempo en segundos entre cada disparo")]
    public float cadenciaDisparo = 2.0f;
    [Tooltip("Velocidad de movimiento de la bala")]
    public float velocidadBala = 15f;
    [Tooltip("Daño que inflige la bala del enemigo")]
    public int danoBala = 1;

    [Header("Configuración de Escopeta / Flecha")]
    [Tooltip("Ángulo de separación para las balas laterales de la flecha")]
    public float anguloSeparacion = 15f;

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
        cronometroDisparo = cadenciaDisparo;
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
                cronometroDisparo = cadenciaDisparo;
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
        cronometroDisparo -= Time.deltaTime;

        if (cronometroDisparo <= 0)
        {
            DispararDesdeSuperficieSpawner();
            cronometroDisparo = cadenciaDisparo;
        }
    }

    void DispararDesdeSuperficieSpawner()
    {
        if (balaSpawner == null)
        {
            Debug.LogError("¡No has asignado el 'Bala Spawner' en el Inspector!");
            return;
        }

        // Definimos los 3 ángulos para formar la punta de flecha: Centro (0), Izquierda y Derecha
        float[] angulos = new float[] { 0f, -anguloSeparacion, anguloSeparacion };

        foreach (float angulo in angulos)
        {
            // Calculamos la rotación sumándole la desviación en el eje Y (giro horizontal)
            Quaternion rotacionBala = balaSpawner.rotation * Quaternion.Euler(0f, angulo, 0f);

            // Instanciamos la bala con la nueva rotación diagonal
            GameObject balaNueva = Instantiate(prefabBala, balaSpawner.position, rotacionBala);

            BalaComportamiento scriptBala = balaNueva.GetComponent<BalaComportamiento>();
            if (scriptBala != null)
            {
                scriptBala.InicializarBala(velocidadBala, danoBala, "Player", gameObject);
            }
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
}
