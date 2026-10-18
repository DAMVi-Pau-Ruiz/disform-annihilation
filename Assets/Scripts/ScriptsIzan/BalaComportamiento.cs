using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // Asegura que la bala tenga un Rigidbody para las físicas
public class BalaComportamiento : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Velocidad constante a la que viajará la bala")]
    public float velocidad = 15f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Forzamos a la bala a moverse siempre hacia adelante (su eje Z azul) 
        // en el momento exacto en el que nace
        rb.linearVelocity = transform.forward * velocidad;

        // Desactivamos la gravedad para que no se caiga al suelo mientras viaja
        rb.useGravity = false;
    }

    // Mantiene la velocidad constante por si choca con algo ligero y frena
    void FixedUpdate()
    {
        // Reaseguramos que mantenga su vector de velocidad constante hacia el frente
        rb.linearVelocity = transform.forward * velocidad;
    }

    // Se destruye automáticamente al salir de la pantalla
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
        Debug.Log("Bala fuera de la cámara: Destruida de forma segura.");
    }

    // Si choca contra el jugador
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Aquí puedes restar vida al jugador en el futuro
            Debug.Log("¡La bala impactó al jugador!");
            Destroy(gameObject); // Se destruye al impactar
        }
    }
}
