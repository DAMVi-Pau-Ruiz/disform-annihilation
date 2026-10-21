using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerCombat playerCombat; // Arrastra aquí tu Nave desde la Jerarquía
    private Slider healthSlider;

    void Awake()
    {
        healthSlider = GetComponent<Slider>();
    }

    void Start()
    {
        if (playerCombat != null && healthSlider != null)
        {
            healthSlider.maxValue = playerCombat.GetTotalHealth();
            healthSlider.value = playerCombat.GetCurrentHealth();
        }
    }

    void Update()
    {
        // Actualiza la barra de vida en tiempo real cada frame
        if (playerCombat != null && healthSlider != null)
        {
            healthSlider.value = playerCombat.GetCurrentHealth();
        }
    }
}