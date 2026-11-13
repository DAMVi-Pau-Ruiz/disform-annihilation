using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class GameSettingsSingleton : MonoBehaviour
{
    public static GameSettingsSingleton Instance { get; private set; }

    // Datos guardados en memoria listos para leer desde cualquier script
    public int IdiomaActual { get; private set; } = 1;
    public int CalidadActual { get; private set; } = 0;
    public float VolumenGeneral { get; private set; } = 1f;

    private void Awake()
    {
        // Si ya existe otro Singleton de una escena anterior, destruye este duplicado
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Carga los datos guardados al abrir el juego por primera vez
        CargarTodo();
    }

    private void OnEnable()
    {
        // Se suscribe a los cambios de escena y de idioma automáticamente
        SceneManager.sceneLoaded += AlCargarEscena;
        LocalizationSettings.SelectedLocaleChanged += AlCambiarIdiomaExterno;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
        LocalizationSettings.SelectedLocaleChanged -= AlCambiarIdiomaExterno;
    }

    // Cada vez que cambias de escena, reaplica la calidad y el volumen
    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        AplicarCalidadYAudio();
    }

    // Si tu LocaleSelector cambia el idioma, el Singleton se entera solo y lo guarda
    private void AlCambiarIdiomaExterno(UnityEngine.Localization.Locale nuevoLocale)
    {
        int indice = LocalizationSettings.AvailableLocales.Locales.IndexOf(nuevoLocale);
        if (indice >= 0)
        {
            IdiomaActual = indice;
            PlayerPrefs.SetInt("LocaleKey", IdiomaActual);
            PlayerPrefs.Save();
        }
    }

    // --- MÉTODOS PARA LLAMAR DESDE TUS OPCIONES ---

    public void GuardarCalidad(int indiceCalidad)
    {
        CalidadActual = indiceCalidad;
        QualitySettings.SetQualityLevel(CalidadActual);
        PlayerPrefs.SetInt("CalidadKey", CalidadActual);
        PlayerPrefs.Save();
    }

    public void GuardarVolumen(float volumen)
    {
        VolumenGeneral = volumen;
        AudioListener.volume = VolumenGeneral;
        PlayerPrefs.SetFloat("VolumenKey", VolumenGeneral);
        PlayerPrefs.Save();
    }

    private void CargarTodo()
    {
        IdiomaActual = PlayerPrefs.GetInt("LocaleKey", 1);
        CalidadActual = PlayerPrefs.GetInt("CalidadKey", 0);
        VolumenGeneral = PlayerPrefs.GetFloat("VolumenKey", 1f);

        AplicarCalidadYAudio();
        StartCoroutine(AplicarIdiomaInicial(IdiomaActual));
    }

    private void AplicarCalidadYAudio()
    {
        QualitySettings.SetQualityLevel(CalidadActual);
        AudioListener.volume = VolumenGeneral;
    }

    private IEnumerator AplicarIdiomaInicial(int indice)
    {
        yield return LocalizationSettings.InitializationOperation;
        if (indice >= 0 && indice < LocalizationSettings.AvailableLocales.Locales.Count)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[indice];
        }
    }
}
