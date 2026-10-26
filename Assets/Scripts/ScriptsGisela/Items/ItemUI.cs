using UnityEngine;
using System.Collections.Generic;

public class ItemUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ItemRandom itemRandom;

    [System.Serializable]
    public struct ItemUIMapping
    {
        public ItemData itemData;          // El ScriptableObject del ítem
        public GameObject itemImageObject; // El GameObject de la imagen en el Canvas
    }

    [Header("Configuración de Imágenes por Ítem")]
    [SerializeField] private List<ItemUIMapping> itemImagesList;

    void Start()
    {
        // Se ejecuta automáticamente al iniciar la escena
        GenerarYMostrarItemAleatorio();
    }

    public void GenerarYMostrarItemAleatorio()
    {
        if (itemRandom == null)
        {
            Debug.LogWarning("Falta asignar el script ItemRandom en ItemUI.");
            return;
        }

        // 1. Obtenemos un ítem aleatorio usando el nuevo sistema de rarezas
        ItemData obtenido = itemRandom.GetRandomItemByRarity();

        // 2. Ocultamos todas las imágenes de la UI primero
        OcultarTodasLasImagenes();

        if (obtenido == null) return;

        // 3. Buscamos la imagen que coincide con este ItemData y la activamos
        foreach (var mapping in itemImagesList)
        {
            if (mapping.itemData == obtenido)
            {
                if (mapping.itemImageObject != null)
                {
                    mapping.itemImageObject.SetActive(true);
                    Debug.Log($"[UI] Ítem mostrado en pantalla: {obtenido.itemName}");
                }
                break;
            }
        }
    }

    private void OcultarTodasLasImagenes()
    {
        foreach (var mapping in itemImagesList)
        {
            if (mapping.itemImageObject != null)
            {
                mapping.itemImageObject.SetActive(false);
            }
        }
    }
}