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
    public static float MouseSensitivityValue { get; private set; } = 1f;
    public static int CurrentLanguageIndex { get; private set; } = 0;

    private bool hasUnsavedChanges = false;

    // ==========================================
    // MÉTODOS DE CICLO DE VIDA (1 LÍNEA CADA UNO)
    // ==========================================
    private void Awake()
    {
        LlamarApagarPanelesIniciales();
    }

    private void Start()
    {
        LlamarInicializacionMenu();
    }

    private void Update()
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
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + Mathf.RoundToInt((float)resolutions[i].refreshRateRatio.value) + "Hz";
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.SetValueWithoutNotify(currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();
    }

    private void InitQualityDropdown()
    {
        if (qualityDropdown == null) return;
        qualityDropdown.RefreshShownValue();
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
        if (resolutions == null || resolutionIndex < 0 || resolutionIndex >= resolutions.Length) return;
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

    // 4. LENGUAJE
    public void CambiarIdiomaPorIndice(int index)
    {
        CurrentLanguageIndex = index;
        SettingsSingleton.Instance.GuardarIdioma(index);
    }

    // ==========================================
    // CARGA Y GUARDADO CON SINGLETON
    // ==========================================
    private void LoadAllSettings()
    {
        var s = SettingsSingleton.Instance;

        if (fullscreenToggle) fullscreenToggle.SetIsOnWithoutNotify(s.Fullscreen);

        if (resolutionDropdown && resolutions != null && s.ResolutionIndex >= 0 && s.ResolutionIndex < resolutions.Length)
        {
            resolutionDropdown.SetValueWithoutNotify(s.ResolutionIndex);
            resolutionDropdown.RefreshShownValue();
        }

        if (qualityDropdown)
        {
            qualityDropdown.SetValueWithoutNotify(s.QualityIndex);
            qualityDropdown.RefreshShownValue();
        }

        if (fpsDropdown)
        {
            fpsDropdown.SetValueWithoutNotify(s.FPSCapIndex);
            fpsDropdown.RefreshShownValue();
        }

        ShowStats = s.ShowStats;
        if (statsUIContainer) statsUIContainer.SetActive(s.ShowStats);
        if (statsToggle) statsToggle.SetIsOnWithoutNotify(s.ShowStats);

        MouseSensitivityValue = s.MouseSensitivity;
        if (sensitivitySlider) sensitivitySlider.SetValueWithoutNotify(s.MouseSensitivity);

        CurrentLanguageIndex = s.LanguageIndex;

        s.AplicarGraficosYAudio();
    }

    private void MarcarCambiosPendientes()
    {
        hasUnsavedChanges = true;
    }

    public void OnClickGuardarConfiguracion()
    {
        try
        {
            bool fs = fullscreenToggle ? fullscreenToggle.isOn : true;
            int res = resolutionDropdown ? resolutionDropdown.value : -1;
            int qual = qualityDropdown ? qualityDropdown.value : 0;
            int fps = fpsDropdown ? fpsDropdown.value : 0;
            bool stats = statsToggle ? statsToggle.isOn : true;
            float sens = sensitivitySlider ? sensitivitySlider.value : 1f;

            SettingsSingleton.Instance.GuardarTodo(fs, res, qual, fps, stats, sens);

            hasUnsavedChanges = false;
            OcultarAvisoError();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al guardar configuración: " + e.Message);
        }
    }

    // ==========================================
    // GESTIÓN DE SALIDA Y PANEL DE ALERTA ROJO
    // ==========================================
    public void OnClickIntentarSalir()
    {
        if (hasUnsavedChanges)
        {
            if (saveWarningPanel) saveWarningPanel.SetActive(true);
        }
        else
        {
            CerrarMenuOpciones();
        }
    }

    public void OnClickBotonSi()
    {
        OnClickGuardarConfiguracion();
        CerrarMenuOpciones();
    }

    public void OnClickBotonNo()
    {
        LoadAllSettings();
        hasUnsavedChanges = false;
        OcultarAvisoError();
        CerrarMenuOpciones();
    }

    private void OcultarAvisoError()
    {
        if (saveWarningPanel) saveWarningPanel.SetActive(false);
    }

    private void CerrarMenuOpciones()
    {
        PauseMenu pauseMenu = Object.FindFirstObjectByType<PauseMenu>();
        if (pauseMenu != null)
        {
            pauseMenu.CloseOptions();
        }
        else if (optionsMenuRoot != null)
        {
            optionsMenuRoot.SetActive(false);
        }
    }
} // <--- Esta es la llave que cierra toda la clase MenuOpcionesManager
