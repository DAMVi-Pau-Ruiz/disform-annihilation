using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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

    [Header("Referencias UI de Lenguaje")]
    [SerializeField] private TMP_Dropdown idiomaDropdown;

    [Header("Panel de Alerta de Guardado (Rojo)")]
    [SerializeField] private GameObject saveWarningPanel;
    [SerializeField] private TextMeshProUGUI warningMessageText;

    [Header("Referencia al Menú Raíz (Para salir al juego)")]
    [SerializeField] private GameObject optionsMenuRoot;

    private Resolution[] resolutions;
    public static bool ShowStats { get; private set; } = true;

    // Propiedad estática para que la lea el script de movimiento del jugador
    public static float MouseSensitivityValue { get; private set; } = 1f;

    // Control de cambios sin guardar
    private bool hasUnsavedChanges = false;

    // ==========================================
    // MÉTODOS DE CICLO DE VIDA (Únicamente para llamar)
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
        MostrarGraphicWindow(); // Oculta todo y deja limpia la vista inicial
    }

    private void LlamarInicializacionMenu()
    {
        OcultarAvisoError();
        InitResolutions();
        InitQualityDropdown();
        InitFPSDropdown();
        InitIdiomaDropdown();
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
                    OnClickBotonNo(); // Si está el panel rojo abierto, ESC equivale a "No" (volver al menú)
                }
                else
                {
                    OnClickIntentarSalir(); // Si no, actúa como intentar salir
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
        qualityDropdown.ClearOptions();
        List<string> names = new List<string>(QualitySettings.names);
        qualityDropdown.AddOptions(names);
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
        MouseSensitivityValue = sensitivity; // Actualiza el valor en tiempo real
        MarcarCambiosPendientes();
    }

    // 4. LENGUAJE
    private void InitIdiomaDropdown()
    {
        if (idiomaDropdown == null) return;
        idiomaDropdown.ClearOptions();
        List<string> idiomas = new List<string> { "Castellano", "Català", "English" };
        idiomaDropdown.AddOptions(idiomas);
        idiomaDropdown.RefreshShownValue();
    }

    public void SetIdioma(int index)
    {
        MarcarCambiosPendientes();
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
            if (idiomaDropdown) PlayerPrefs.SetInt("IdiomaIndex", idiomaDropdown.value);

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
        OcultarAvisoError(); // Vuelve al menú de opciones para poder guardar
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
        MouseSensitivityValue = sens; // Carga la sensibilidad guardada

        int idiomaIndex = PlayerPrefs.GetInt("IdiomaIndex", 0);
        if (idiomaDropdown) idiomaDropdown.value = idiomaIndex;
    }
}