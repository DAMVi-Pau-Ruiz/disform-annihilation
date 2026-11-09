
using UnityEngine;

public class ProjectileRicochet : ProjectileBase
{
    [Header("Rebotes")]
    [SerializeField] private int rebotesMaximos = 4;

    private int rebotesActuales;

    protected override void OnCollisionEnter(Collision collision)
    {
        if (haImpactado) return;

        IHealthSystem salud =
            collision.collider.GetComponentInParent<IHealthSystem>();

        if (salud != null)
        {
            AplicarDaño(collision.collider);
            return;
        }

        rebotesActuales++;

        if (rebotesActuales >= rebotesMaximos)
        {
            Destroy(gameObject);
            return;
        }

        if (collision.contactCount > 0)
        {
            Vector3 direccion = Vector3.Reflect(
                GetComponent<Rigidbody>().linearVelocity.normalized,
                collision.GetContact(0).normal
            );

            GetComponent<Rigidbody>().linearVelocity =
                direccion * GetComponent<Rigidbody>().linearVelocity.magnitude;
        }
    }
}
