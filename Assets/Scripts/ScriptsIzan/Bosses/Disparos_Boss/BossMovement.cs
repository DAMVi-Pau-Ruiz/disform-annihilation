
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private float distanciaMinima = 5f;
    [SerializeField] private float velocidadRotacion = 8f;

    private Transform jugador;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            jugador = player.transform;
        }
        else
        {
            Debug.LogError("No se ha encontrado el jugador.");
        }
    }

    private void FixedUpdate()
    {
        if (jugador == null || rb == null)
            return;

        Vector3 direccion = jugador.position - rb.position;
        direccion.y = 0f;

        float distancia = direccion.magnitude;

        if (distancia > 0.01f)
        {
            direccion.Normalize();

            // Orientar el boss hacia el jugador.
            Quaternion rotacion = Quaternion.LookRotation(direccion);

            rb.MoveRotation(
                Quaternion.Slerp(
                    rb.rotation,
                    rotacion,
                    velocidadRotacion * Time.fixedDeltaTime
                )
            );

            // Acercarse sin llegar a tocar al jugador.
            if (distancia > distanciaMinima)
            {
                Vector3 movimiento = direccion * velocidad * Time.fixedDeltaTime;
                rb.MovePosition(rb.position + movimiento);
            }
        }
    }
}
