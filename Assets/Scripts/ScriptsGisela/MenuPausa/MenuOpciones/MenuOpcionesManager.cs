using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    private Resolution[] resolutions;
    public static bool ShowStats { get; private set; } = true;

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

    // ==========================================
    // MÉTODOS DE LLAMADA Y GESTIÓN
    // ==========================================
    private void LlamarApagarPanelesIniciales()
    {
        MostrarGraphicWindow(); // Oculta todo y deja limpia la vista inicial
    }

    private void LlamarInicializacionMenu()
    {
        InitResolutions();
        InitQualityDropdown();
        InitFPSDropdown();
        InitIdiomaDropdown();
        LoadAllSettings();
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
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityIndex", qualityIndex);
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
        PlayerPrefs.SetInt("FPSCapIndex", index);
    }

    public void ToggleShowStats(bool isVisible)
    {
        ShowStats = isVisible;
        if (statsUIContainer != null)
        {
            statsUIContainer.SetActive(isVisible);
        }
        PlayerPrefs.SetInt("ShowStats", isVisible ? 1 : 0);
    }

    // 3. CONTROLES
    public void SetMouseSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
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
        PlayerPrefs.SetInt("IdiomaIndex", index);
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

        int idiomaIndex = PlayerPrefs.GetInt("IdiomaIndex", 0);
        if (idiomaDropdown) idiomaDropdown.value = idiomaIndex;
    }
}