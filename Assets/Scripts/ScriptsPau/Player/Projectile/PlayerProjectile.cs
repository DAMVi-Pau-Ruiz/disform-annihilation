using JetBrains.Annotations;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 15f;

    [Header("Combat")]
    [SerializeField] private int damage = 5;

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        bool esVacio = other.CompareTag("Vacio");

        if (esVacio)
        {
            Debug.Log(esVacio);
            return;
        }
        
        // Si la bala toca el objeto hijo "Detector" (que es un Trigger), lo ignora por completo
        if (other.isTrigger)
        {
            return;
        }

        // Si toca el cuerpo sólido (que ahora mide 0.5 y es el objeto principal con el tag Enemy)
        bool esEnemigo = other.CompareTag("Enemy") || (other.transform.parent != null && other.transform.parent.CompareTag("Enemy"));

        if (esEnemigo)
        {
            IHealthSystem health = other.GetComponent<IHealthSystem>() ?? other.GetComponentInParent<IHealthSystem>();
            if (health != null)
            {
                health.GetHurt(damage);
            }

            Debug.Log("¡Impacto confirmado contra el CUERPO del enemigo!");
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }


}
