using UnityEngine;

public class Enemy_Controller : MonoBehaviour, IHealthSystem
{
    [Header("Vida del enemigo")]
    [SerializeField] public int health = 3;

    private float currentHealth;
    private bool estaMuerto = false;

    private void Awake()
    {
        currentHealth = health;
    }

    public void GetHurt(float quantity)
    {
        if (estaMuerto || quantity <= 0)
        {
            return;
        }

        currentHealth -= quantity;

        Debug.Log("Enemigo dañado. Vida restante: " + Mathf.Max(0, currentHealth));

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        estaMuerto = true;
        Debug.Log("El enemigo ha muerto.");
        Destroy(gameObject);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}