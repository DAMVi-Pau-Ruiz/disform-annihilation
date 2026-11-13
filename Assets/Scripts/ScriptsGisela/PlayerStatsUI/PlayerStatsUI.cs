using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el script de estadísticas de tu jugador")]
    [SerializeField] private PlayerStats playerStats;

    [Header("Elemento de UI")]
    [SerializeField] private TextMeshProUGUI statsText;

    [Header("Traducciones")]
    [SerializeField] private LocalizedString textoVida;
    [SerializeField] private LocalizedString textoDanoTotal;
    [SerializeField] private LocalizedString textoDanoMult;
    [SerializeField] private LocalizedString textoDanoBase;
    [SerializeField] private LocalizedString textoVelocidad;
    [SerializeField] private LocalizedString textoCadencia;
    [SerializeField] private LocalizedString textoVelBala;

    // Variables en memoria para no buscar en la tabla cada frame
    private string strVida = "Vida";
    private string strDanoTotal = "Daño total";
    private string strDanoMult = "Daño multiplicador";
    private string strDanoBase = "Daño base";
    private string strVelocidad = "Velocidad";
    private string strCadencia = "Cadencia";
    private string strVelBala = "Vel. Bala";

    private void OnEnable()
    {
        // Nos suscribimos al cambio de idioma
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        ActualizarTraducciones();
    }

    private void OnDisable()
    {
        // Nos desuscribimos para evitar errores de memoria
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(Locale newLocale)
    {
        ActualizarTraducciones();
    }

    private void ActualizarTraducciones()
    {
        if (!textoVida.IsEmpty) strVida = textoVida.GetLocalizedString();
        if (!textoDanoTotal.IsEmpty) strDanoTotal = textoDanoTotal.GetLocalizedString();
        if (!textoDanoMult.IsEmpty) strDanoMult = textoDanoMult.GetLocalizedString();
        if (!textoDanoBase.IsEmpty) strDanoBase = textoDanoBase.GetLocalizedString();
        if (!textoVelocidad.IsEmpty) strVelocidad = textoVelocidad.GetLocalizedString();
        if (!textoCadencia.IsEmpty) strCadencia = textoCadencia.GetLocalizedString();
        if (!textoVelBala.IsEmpty) strVelBala = textoVelBala.GetLocalizedString();
    }

    void Update()
    {
        if (playerStats == null || statsText == null) return;

        statsText.text =
            $"{strVida}: {playerStats.GetCurrentHealth()} / {playerStats.GetTotalHealth()}\n" +
            $"{strDanoTotal}: {playerStats.GetTotalDamage():F1}\n" +
            $"{strDanoMult}: {playerStats.GetDamageMultiplier():F1}\n" +
            $"{strDanoBase}: {playerStats.GetBaseDamage():F1}\n" +
            $"{strVelocidad}: {playerStats.GetBaseSpeed():F1}\n" +
            $"{strCadencia}: {playerStats.GetBulletsPerSecond():F1} b/s\n" +
            $"{strVelBala}: {playerStats.GetBulletSpeed():F1}";
    }
}
