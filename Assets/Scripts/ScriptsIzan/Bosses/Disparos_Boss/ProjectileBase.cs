
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] protected float daño = 1f;
    [SerializeField] protected float tiempoVida = 8f;

    protected Transform propietario;

    protected bool haImpactado;

    protected virtual void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    public void ConfigurarPropietario(Transform boss)
    {
        propietario = boss;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        AplicarDaño(collision.collider);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        AplicarDaño(other);
    }

    protected void AplicarDaño(Collider objetivo)
    {
        if (haImpactado)
            return;

        // Ignorar colisiones con el propio boss.
        if (propietario != null &&
            objetivo.transform.root == propietario.root)
        {
            return;
        }

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
