using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class MenuOpcionesManager : MonoBehaviour
{
    [Header("Paneles de Contenido (Ventanas)")]
    [SerializeField] private GameObject graphicWindow;
    [SerializeField] private GameObject controlesWindow;
    [SerializeField] private GameObject languageWindow;

    [Header("Referencias UI de Vídeo y Gráficos")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown fpsDropdown;
    [SerializeField] private Toggle statsToggle;
    [SerializeField] private GameObject statsUIContainer;

    [Header("Referencias UI de Controles")]
    [SerializeField] private Slider sensitivitySlider;

    [Header("Panel de Alerta de Guardado (Rojo)")]
    [SerializeField] private GameObject saveWarningPanel;
    [SerializeField] private TextMeshProUGUI warningMessageText;

    [Header("Referencia al Menú Raíz (Para salir al juego)")]
    [SerializeField] private GameObject optionsMenuRoot;

    private Resolution[] resolutions;
    public static bool ShowStats { get; private set; } = true;

    // Propiedad estática para la sensibilidad del ratón
    public static float MouseSensitivityValue { get; private set; } = 1f;

    // Propiedad estática requerida por otros scripts (como LocalizedText.cs)
    public static int CurrentLanguageIndex { get; private set; } = 0;

    // Control de cambios sin guardar
    private bool hasUnsavedChanges = false;

    // ==========================================
    // MÉTODOS DE CICLO DE VIDA
    // ==========================================
    private void Awake()
    {
        LlamarApagarPanelesIniciales();
    }

    void Start()
    {
        LlamarInicializacionMenu();
    }

    void Update()
    {
        LlamarEscucharTeclaEscape();
    }

    // ==========================================
    // MÉTODOS DE LLAMADA Y GESTIÓN
    // ==========================================
    private void LlamarApagarPanelesIniciales()
    {
        MostrarGraphicWindow();
    }

    private void LlamarInicializacionMenu()
    {
        OcultarAvisoError();
        InitResolutions();
        InitQualityDropdown();
        InitFPSDropdown();
        LoadAllSettings();
        hasUnsavedChanges = false;
    }

    private void LlamarEscucharTeclaEscape()
    {
        if (optionsMenuRoot != null && optionsMenuRoot.activeSelf)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (saveWarningPanel != null && saveWarningPanel.activeSelf)
                {
                    OnClickBotonNo();
                }
                else
                {
                    OnClickIntentarSalir();
                }
            }
        }
    }

    // 1. GESTIÓN DE PESTAÑAS
    public void MostrarGraphicWindow()
    {
        if (graphicWindow) graphicWindow.SetActive(true);
        if (controlesWindow) controlesWindow.SetActive(false);
        if (languageWindow) languageWindow.SetActive(false);
    }

    public void MostrarControlesWindow()
    {
        if (graphicWindow) graphicWindow.SetActive(false);
        if (controlesWindow) controlesWindow.SetActive(true);
        if (languageWindow) languageWindow.SetActive(false);
    }

    public void MostrarLanguageWindow()
    {
        if (graphicWindow) graphicWindow.SetActive(false);
        if (controlesWindow) controlesWindow.SetActive(false);
        if (languageWindow) languageWindow.SetActive(true);
    }

    // 2. VÍDEO Y GRÁFICOS
    private void InitResolutions()
    {
        if (resolutionDropdown == null) return;
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRateRatio.value + "Hz";
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void InitQualityDropdown()
    {
        if (qualityDropdown == null) return;
    }

    private void InitFPSDropdown()
    {
        if (fpsDropdown == null) return;
        fpsDropdown.ClearOptions();
        List<string> fpsOptions = new List<string> { "Ilimitados", "30 FPS", "60 FPS", "120 FPS", "144 FPS" };
        fpsDropdown.AddOptions(fpsOptions);
        fpsDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        MarcarCambiosPendientes();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Resolution currentRes = Screen.currentResolution;
        Screen.SetResolution(currentRes.width, currentRes.height, isFullscreen);
        MarcarCambiosPendientes();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        MarcarCambiosPendientes();
    }

    public void SetFPSCap(int index)
    {
        switch (index)
        {
            case 0: Application.targetFrameRate = -1; break;
            case 1: Application.targetFrameRate = 30; break;
            case 2: Application.targetFrameRate = 60; break;
            case 3: Application.targetFrameRate = 120; break;
            case 4: Application.targetFrameRate = 144; break;
        }
        MarcarCambiosPendientes();
    }

    public void ToggleShowStats(bool isVisible)
    {
        ShowStats = isVisible;
        if (statsUIContainer != null)
        {
            statsUIContainer.SetActive(isVisible);
        }
        MarcarCambiosPendientes();
    }

    // 3. CONTROLES
    public void SetMouseSensitivity(float sensitivity)
    {
        MouseSensitivityValue = sensitivity;
        MarcarCambiosPendientes();
    }

    // 4. LENGUAJE (MÉTODO POR ÍNDICES PARA LOS BOTONES)
    public void CambiarIdiomaPorIndice(int index)
    {
        MarcarCambiosPendientes();

        var locales = LocalizationSettings.AvailableLocales.Locales;

        if (index >= 0 && index < locales.Count)
        {
            Locale localeACambiar = locales[index];
            LocalizationSettings.SelectedLocale = localeACambiar;
            CurrentLanguageIndex = index;

            PlayerPrefs.SetInt("IdiomaIndex", index);
            PlayerPrefs.Save();

            Debug.Log("Idioma cambiado a la posición: " + index + " (" + localeACambiar.Identifier.Code + ")");
        }
        else
        {
            Debug.LogError("El índice de idioma " + index + " no existe en la configuración de Unity Localization.");
        }
    }

    // ==========================================
    // SISTEMA DE GUARDADO Y VALIDACIÓN DE SALIDA
    // ==========================================
    private void MarcarCambiosPendientes()
    {
        hasUnsavedChanges = true;
    }

    public void OnClickGuardarConfiguracion()
    {
        try
        {
            if (fullscreenToggle) PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
            if (qualityDropdown) PlayerPrefs.SetInt("QualityIndex", qualityDropdown.value);
            if (fpsDropdown) PlayerPrefs.SetInt("FPSCapIndex", fpsDropdown.value);
            if (statsToggle) PlayerPrefs.SetInt("ShowStats", statsToggle.isOn ? 1 : 0);
            if (sensitivitySlider) PlayerPrefs.SetFloat("MouseSensitivity", sensitivitySlider.value);

            PlayerPrefs.Save();
            hasUnsavedChanges = false;
            OcultarAvisoError();
            Debug.Log("Configuración guardada con éxito.");
        }
        catch (System.Exception e)
        {
            MostrarAvisoError("Error al guardar: " + e.Message);
        }
    }

    public void OnClickIntentarSalir()
    {
        if (hasUnsavedChanges)
        {
            MostrarAvisoError("Estás saliendo sin guardar los cambios. ¿Quieres continuar?");
        }
        else
        {
            CerrarOptionsMenuDirectamente();
        }
    }

    private void MostrarAvisoError(string mensaje)
    {
        if (warningMessageText != null) warningMessageText.text = mensaje;
        if (saveWarningPanel != null) saveWarningPanel.SetActive(true);
    }

    private void OcultarAvisoError()
    {
        if (saveWarningPanel != null) saveWarningPanel.SetActive(false);
    }

    public void OnClickBotonSi()
    {
        hasUnsavedChanges = false;
        OcultarAvisoError();
        CerrarOptionsMenuDirectamente();
    }

    public void OnClickBotonNo()
    {
        OcultarAvisoError();
    }

    private void CerrarOptionsMenuDirectamente()
    {
        PauseMenu pauseMenu = Object.FindFirstObjectByType<PauseMenu>();
        if (pauseMenu != null)
        {
            pauseMenu.CloseOptions();
        }
        else
        {
            if (optionsMenuRoot != null)
            {
                optionsMenuRoot.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    // CARGAR CONFIGURACIONES GUARDADAS
    private void LoadAllSettings()
    {
        bool isFull = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        Screen.fullScreen = isFull;
        if (fullscreenToggle) fullscreenToggle.isOn = isFull;

        int quality = PlayerPrefs.GetInt("QualityIndex", QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(quality);
        if (qualityDropdown) qualityDropdown.value = quality;

        int fpsIndex = PlayerPrefs.GetInt("FPSCapIndex", 2);
        SetFPSCap(fpsIndex);
        if (fpsDropdown) fpsDropdown.value = fpsIndex;

        bool showStatsSaved = PlayerPrefs.GetInt("ShowStats", 1) == 1;
        ShowStats = showStatsSaved;
        if (statsToggle) statsToggle.isOn = showStatsSaved;
        if (statsUIContainer) statsUIContainer.SetActive(showStatsSaved);

        float sens = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        if (sensitivitySlider) sensitivitySlider.value = sens;
        MouseSensitivityValue = sens;

        // Cargar el idioma guardado mediante su índice numérico
        int indiceGuardado = PlayerPrefs.GetInt("IdiomaIndex", 0);
        CambiarIdiomaPorIndice(indiceGuardado);
    }
}