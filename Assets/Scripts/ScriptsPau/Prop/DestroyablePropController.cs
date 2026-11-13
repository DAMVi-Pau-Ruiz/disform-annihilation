using System;
using Unity.VisualScripting;
using UnityEngine;

public class DestroyablePropController : MonoBehaviour, IHealthSystem
{
    [Header("References")]
    [SerializeField] private DropController dropController;
    
    [Header("Features")]
    [SerializeField] private float totalHealth = 10;

    private float currentHealth;

    private void Start()
    {
        currentHealth = totalHealth;
    }

    public void GetHurt(float quantity)
    {
        currentHealth -= quantity;

        if (currentHealth <= 0)
        {

            if (dropController != null)
            {
                dropController.GenerateDrops();
            }

            Destroy(gameObject);
        }   
    }
}
