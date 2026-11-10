
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] protected float daño = 1f;
    [SerializeField] protected float tiempoVida = 8f;
    [SerializeField] private LayerMask capasRebote;

    [Header("Rebotes")]
    [SerializeField] protected int rebotesMaximos = 5;

    protected Rigidbody rb;
    protected Transform propietario;

    private int rebotesActuales;
    protected bool haImpactado;

    public void ConfigurarPropietario(Transform boss)
    {
        propietario = boss;
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
            rb = GetComponentInParent<Rigidbody>();

        if (rb == null)
            rb = GetComponentInChildren<Rigidbody>();

        if (rb == null)
            Debug.LogError("No se encuentra Rigidbody en " + gameObject.name);
    }

    protected virtual void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (EsPropietario(collision.collider))
            return;

        // Comprobar si ha tocado una pared.
        if (((1 << collision.gameObject.layer) & capasRebote) != 0)
        {
            Rebotar(collision);
            return;
        }

        // Comprobar si ha tocado algo que pueda recibir daño.
        IHealthSystem salud =
            collision.collider.GetComponentInParent<IHealthSystem>();

        if (salud != null)
        {
            AplicarDaño(collision.collider);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        AplicarDaño(other);
    }


    private void Rebotar(Collision collision)
    {
        Debug.Log("ENTRANDO EN REBOTAR");

        if (rb == null)
        {
            Debug.LogError("Rigidbody es null");
            return;
        }

        if (collision.contactCount == 0)
        {
            Debug.LogError("No hay puntos de contacto");
            return;
        }

        if (rebotesActuales >= rebotesMaximos)
        {
            Debug.Log("Máximo de rebotes alcanzado");
            Destroy(gameObject);
            return;
        }

        Vector3 normal = collision.GetContact(0).normal;
        Vector3 velocidadAnterior = rb.linearVelocity;

        Debug.Log("Velocidad anterior: " + velocidadAnterior);
        Debug.Log("Normal: " + normal);

        Vector3 direccion = Vector3.Reflect(
            velocidadAnterior.normalized,
            normal
        );

        rb.linearVelocity = direccion * velocidadAnterior.magnitude;

        rebotesActuales++;

        Debug.Log("REBOTE REALIZADO: " + rebotesActuales);
    }


    private bool EsPropietario(Collider objetivo)
    {
        return propietario != null &&
               objetivo.transform.root == propietario.root;
    }

    protected void AplicarDaño(Collider objetivo)
    {
        if (haImpactado || EsPropietario(objetivo))
            return;

        IHealthSystem salud =
            objetivo.GetComponentInParent<IHealthSystem>();

        if (salud != null)
        {
            salud.GetHurt(daño);
            haImpactado = true;
            Destroy(gameObject);
        }
    }
}
