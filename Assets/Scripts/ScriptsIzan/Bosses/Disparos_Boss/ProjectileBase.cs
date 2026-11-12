
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
        if (EsPropietario(collision.collider))
            return;

        //// Comprobar si ha tocado una pared.
        //Debug.Log("COLISION CON: " + collision.collider.name);
        //Debug.Log("LAYER: " + LayerMask.LayerToName(collision.collider.gameObject.layer));
        //Debug.Log("CAPAS DE REBOTE: " + capasRebote.value);

        //if (((1 << collision.collider.gameObject.layer) & capasRebote) != 0)
        //{
        //    Debug.Log("VELOCIDAD ANTES DEL REBOTE: " + rb.linearVelocity); 
        //    Rebotar(collision);
        //    return;
        //}

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
