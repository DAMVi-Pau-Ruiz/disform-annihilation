using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [Header("Traducciones (0: Castellano, 1: Català, 2: English)")]
    [SerializeField] private string textoCastellano;

    [TextArea(2, 5)]
    [SerializeField] private string textoCatala;

    [TextArea(2, 5)]
    [SerializeField] private string textoEnglish;

    private TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        ActualizarTexto();
    }

    private void OnEnable()
    {
        ActualizarTexto();
    }

    public void ActualizarTexto()
    {
        if (textMeshPro == null) return;

        int idiomaActual = MenuOpcionesManager.CurrentLanguageIndex;

        switch (idiomaActual)
        {
            case 0:
                textMeshPro.text = textoCastellano;
                break;
            case 1:
                textMeshPro.text = string.IsNullOrEmpty(textoCatala) ? textoCastellano : textoCatala;
                break;
            case 2:
                textMeshPro.text = string.IsNullOrEmpty(textoEnglish) ? textoCastellano : textoEnglish;
                break;
            default:
                textMeshPro.text = textoCastellano;
                break;
        }
    }
}