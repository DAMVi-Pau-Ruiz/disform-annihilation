
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] protected float daño = 1f;
    [SerializeField] protected float tiempoVida = 8f;

    protected Rigidbody rb;
    protected Transform propietario;

    protected bool haImpactado;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("No se encuentra Rigidbody en " + gameObject.name);
        }
    }

    protected virtual void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    public void ConfigurarPropietario(Transform boss)
    {
        propietario = boss;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        AplicarDaño(other);
    }

    protected void AplicarDaño(Collider objetivo)
    {
        if (haImpactado || objetivo == null)
            return;

        if (EsPropietario(objetivo))
            return;

        IHealthSystem salud =
            objetivo.GetComponentInParent<IHealthSystem>();

        if (salud == null)
            return;

        haImpactado = true;

        salud.GetHurt(daño);



        Debug.Log("PROYECTIL DEL BOSS DESTRUIDO: " + objetivo.name);
        Destroy(gameObject);
    }

    private bool EsPropietario(Collider objetivo)
    {
        return propietario != null &&
               objetivo.transform.root == propietario.root;
    }
}
