using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el script de estadísticas de tu jugador")]
    [SerializeField] private PlayerStats playerStats;

    [Header("Elemento de UI")]
    [SerializeField] private TextMeshProUGUI statsText; // El texto de TextMeshPro que mostrará todo

    void Update()
    {
        // Seguridad por si olvidas asignar las referencias
        if (playerStats == null || statsText == null) return;

        // Actualizamos todas las estadísticas usando los Getters de tu PlayerStats
        statsText.text =
            $"Vida: {playerStats.GetCurrentHealth()} / {playerStats.GetTotalHealth()}\n" +
            $"Daño total: {playerStats.GetTotalDamage():F1}\n" +
            $"Daño multiplicador:{playerStats.GetDamageMultiplier():F1}\n"+
            //daño base
            $"Daño base: {playerStats.GetBaseDamage():F1}\n"+
            $"Velocidad: {playerStats.GetBaseSpeed():F1}\n" +
            $"Cadencia: {playerStats.GetBulletsPerSecond():F1} b/s\n" +
            $"Vel. Bala: {playerStats.GetBulletSpeed():F1}";
    }
}