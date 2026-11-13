
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] protected float daño = 1f;
    [SerializeField] protected float tiempoVida = 8f;
    [SerializeField] private LayerMask capasRebote;

    protected Rigidbody rb;
    protected Transform propietario;

    private int rebotesActuales;
    protected bool haImpactado;

    public void ConfigurarPropietario(Transform boss)
    {
        propietario = boss;
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            Debug.Log("VELOCIDAD ACTUAL: " + rb.linearVelocity);
        }
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

        if (collision.collider.GetComponentInParent<PlayerStats>() != null)
        {
            AplicarDaño(collision.collider);
            return;
        }
        if (EsPropietario(collision.collider))
            return;
        PlayerStats jugador = collision.collider.GetComponentInParent<PlayerStats>();

        if (jugador != null)
        {
            AplicarDaño(collision.collider);
            return;
        }

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
        if (rb == null || collision.contactCount == 0)
            return;

        rb.linearVelocity = -rb.linearVelocity;
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
