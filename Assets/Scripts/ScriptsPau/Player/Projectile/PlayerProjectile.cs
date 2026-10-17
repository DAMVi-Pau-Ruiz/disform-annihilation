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
        IHealthSystem healthSystem = other.GetComponent<IHealthSystem>();

        if (healthSystem != null)
        {
            healthSystem.GetHurt(damage);
        }
        
        Destroy(gameObject);
    }
}
