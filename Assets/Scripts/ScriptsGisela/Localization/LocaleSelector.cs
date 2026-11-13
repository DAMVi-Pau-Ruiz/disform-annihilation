using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour
{
    private bool active = false;

    private void Start()
    {
        // carga el idioma guardado al iniciar el juego (si no hay ninguno, usa el English)
        int savedID = PlayerPrefs.GetInt("LocaleKey", 1);
        ChangeLocale(savedID);
    }

    public void ChangeLocale(int localeID)
    {
        if (active == true)
        {
            return;
        }
        StartCoroutine(SetLocale(localeID));
    }

    private IEnumerator SetLocale(int _localeID)
    {
        active = true; // bloquea nuevos clics mientras cambia el idioma
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];

        // guarda el idioma seleccionado para la próxima vez
        PlayerPrefs.SetInt("LocaleKey", _localeID);
        active = false;
    }
}