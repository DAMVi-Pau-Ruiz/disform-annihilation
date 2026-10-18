using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BalaComportamiento : MonoBehaviour
{
    private float velocidad = 15f;
    private int dano = 1;
    private string tagObjetivo = "Player";
    private GameObject creadorBala; // Guarda la referencia de quién disparó
    private Rigidbody rb;
    private bool inicializada = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    // Añadimos el parámetro "GameObject creador" al método
    public void InicializarBala(float nuevaVelocidad, int nuevoDano, string nuevoTagObjetivo, GameObject creador)
    {
        velocidad = nuevaVelocidad;
        dano = nuevoDano;
        tagObjetivo = nuevoTagObjetivo;
        creadorBala = creador; // Guardamos quién la disparó
        inicializada = true;
    }

    void FixedUpdate()
    {
        if (!inicializada) return;
        rb.MovePosition(rb.position + transform.forward * velocidad * Time.fixedDeltaTime);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!inicializada) return;

        // 1. FILTRO ANTISUICIDIO: Si la bala choca con el que la disparó (o su padre), pasa de largo
        if (other.gameObject == creadorBala || (other.transform.parent != null && other.transform.parent.gameObject == creadorBala))
        {
            return;
        }

        // 2. Si golpeamos un radio de detección u otro trigger, pasamos de largo
        if (other.isTrigger)
        {
            return;
        }

        // 3. Comprobamos si golpea al objetivo real (el Player)
        bool esObjetivo = other.CompareTag(tagObjetivo) || (other.transform.parent != null && other.transform.parent.CompareTag(tagObjetivo));

        if (esObjetivo)
        {
            IHealthSystem healthSystem = other.GetComponent<IHealthSystem>() ?? other.GetComponentInParent<IHealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.GetHurt(dano);
                Debug.Log($"¡La bala del enemigo ha dañado al {tagObjetivo}!");
            }

            Destroy(gameObject);
            return;
        }

        // 4. Si choca contra una pared o suelo
        Destroy(gameObject);
    }
}
