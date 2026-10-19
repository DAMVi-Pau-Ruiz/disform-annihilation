using UnityEngine;

public class Enemy_Controller : MonoBehaviour, IHealthSystem
{
    [Header("Health")]
    [SerializeField] public int health;

    private int currentHealth;

    public void Awake()
    {
        currentHealth = health;
    }

    
    public void GetHurt(int quantity)
    {
        if (currentHealth > 0)
        {
            currentHealth -= quantity;
            Debug.Log($"Enemigo dañado. Vida restante: {currentHealth}");

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log("El enemigo ha muerto.");
        Destroy(gameObject); // O lanzar animación de muerte
    }
}
