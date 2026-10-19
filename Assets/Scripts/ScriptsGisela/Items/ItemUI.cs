using UnityEngine;
using System.Collections.Generic;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private ItemRandom itemRandom; 

    [System.Serializable]
    public struct ItemImageMapping
    {
        public int itemId;          // ID del ítem (1: curar, 2: daño, 3: muerte, 4: revivir)
        public GameObject itemImage; // El GameObject de la imagen en el Canvas
    }

    [Header("Configuración de Imágenes por ID")]
    [SerializeField] private List<ItemImageMapping> itemImagesList;

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

        // Ejecutamos la función de probabilidad para calcular el ítem ganador del CSV
        itemRandom.ItemProbability();

        //  Obtenemos el ID del ítem que ha salido seleccionado
        int idObtenido = itemRandom.lastSelectedId;

        //  Ocultamos todas las imágenes primero
        OcultarTodasLasImagenes();

        //  Buscamos la imagen que coincide con ese ID y la activamos
        foreach (var mapping in itemImagesList)
        {
            if (mapping.itemId == idObtenido)
            {
                if (mapping.itemImage != null)
                {
                    mapping.itemImage.SetActive(true);
                    Debug.Log($"[UI] Ítem inicial generado automáticamente -> ID: {idObtenido}");
                }
                break;
            }
        }
    }

    private void OcultarTodasLasImagenes()
    {
        foreach (var mapping in itemImagesList)
        {
            if (mapping.itemImage != null)
            {
                mapping.itemImage.SetActive(false);
            }
        }
    }
}