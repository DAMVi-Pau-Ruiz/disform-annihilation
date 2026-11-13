using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class SettingsSingleton : MonoBehaviour
{
    private static SettingsSingleton _instance;
    public static SettingsSingleton Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SettingsSingleton");
                _instance = go.AddComponent<SettingsSingleton>();
            }
            return _instance;
        }
    }

    // Valores guardados en memoria entre escenas
    public bool Fullscreen { get; private set; } = true;
    public int ResolutionIndex { get; private set; } = -1;
    public int QualityIndex { get; private set; } = 0;
    public int FPSCapIndex { get; private set; } = 0;
    public bool ShowStats { get; private set; } = true;
    public float MouseSensitivity { get; private set; } = 1f;
    public int LanguageIndex { get; private set; } = 0;

    private void Awake()
    {
        Inicializar();
    }

    private void Inicializar()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        CargarDesdePlayerPrefs();
    }

    private void CargarDesdePlayerPrefs()
    {
        Fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        ResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", -1);
        QualityIndex = PlayerPrefs.GetInt("QualityIndex", 0);
        FPSCapIndex = PlayerPrefs.GetInt("FPSCapIndex", 0);
        ShowStats = PlayerPrefs.GetInt("ShowStats", 1) == 1;
        MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        LanguageIndex = PlayerPrefs.GetInt("IdiomaIndex", 0);

        AplicarGraficosYAudio();
        StartCoroutine(AplicarIdiomaCoroutine(LanguageIndex));
    }

    public void GuardarTodo(bool fullscreen, int resIndex, int qualityIndex, int fpsIndex, bool showStats, float sensitivity)
    {
        Fullscreen = fullscreen;
        ResolutionIndex = resIndex;
        QualityIndex = qualityIndex;
        FPSCapIndex = fpsIndex;
        ShowStats = showStats;
        MouseSensitivity = sensitivity;

        PlayerPrefs.SetInt("Fullscreen", Fullscreen ? 1 : 0);
        PlayerPrefs.SetInt("ResolutionIndex", ResolutionIndex);
        PlayerPrefs.SetInt("QualityIndex", QualityIndex);
        PlayerPrefs.SetInt("FPSCapIndex", FPSCapIndex);
        PlayerPrefs.SetInt("ShowStats", ShowStats ? 1 : 0);
        PlayerPrefs.SetFloat("MouseSensitivity", MouseSensitivity);
        PlayerPrefs.Save();

        AplicarGraficosYAudio();
    }

    public void GuardarIdioma(int index)
    {
        LanguageIndex = index;
        PlayerPrefs.SetInt("IdiomaIndex", LanguageIndex);
        PlayerPrefs.Save();
        StartCoroutine(AplicarIdiomaCoroutine(LanguageIndex));
    }

    public void AplicarGraficosYAudio()
    {
        Screen.fullScreen = Fullscreen;
        QualitySettings.SetQualityLevel(QualityIndex);

        switch (FPSCapIndex)
        {
            case 0: Application.targetFrameRate = -1; break;
            case 1: Application.targetFrameRate = 30; break;
            case 2: Application.targetFrameRate = 60; break;
            case 3: Application.targetFrameRate = 120; break;
            case 4: Application.targetFrameRate = 144; break;
        }

        Resolution[] resoluciones = Screen.resolutions;
        if (resoluciones != null && resoluciones.Length > 0 && ResolutionIndex >= 0 && ResolutionIndex < resoluciones.Length)
        {
            Resolution res = resoluciones[ResolutionIndex];
            Screen.SetResolution(res.width, res.height, Fullscreen);
        }
    }

    private IEnumerator AplicarIdiomaCoroutine(int index)
    {
        yield return LocalizationSettings.InitializationOperation;
        var locales = LocalizationSettings.AvailableLocales.Locales;
        if (index >= 0 && index < locales.Count)
        {
            LocalizationSettings.SelectedLocale = locales[index];
        }
    }
}
