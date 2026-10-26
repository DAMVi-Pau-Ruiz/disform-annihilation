using UnityEngine;
using System.Collections.Generic;

public class ItemRandom : MonoBehaviour
{
    [Header("Lista de Todos los Ítems (ScriptableObjects)")]
    [Tooltip("Arrastra aquí tus archivos ItemData creados en el proyecto")]
    public List<ItemData> allItems;

    // Método que calcula la rareza por probabilidad y devuelve un ItemData de la lista
    public ItemData GetRandomItemByRarity()
    {
        if (allItems == null || allItems.Count == 0)
        {
            Debug.LogWarning("La lista 'allItems' en ItemRandom está vacía. ¡Añade tus ItemData en el Inspector!");
            return null;
        }

        int roll = Random.Range(1, 101);
        ItemRarity targetRarity;

        // Probabilidades de rareza:
        // - Common: 45% (<= 45)
        // - Rare: 35% (<= 80)
        // - Epic: 15% (<= 95)
        // - Legendary: 5% (> 95)
        if (roll <= 45)
        {
            targetRarity = ItemRarity.Common;
        }
        else if (roll <= 80)
        {
            targetRarity = ItemRarity.Rare;
        }
        else if (roll <= 95)
        {
            targetRarity = ItemRarity.Epic;
        }
        else
        {
            targetRarity = ItemRarity.Legendary;
        }

        // Filtramos de la lista general los ítems que coincidan con la rareza seleccionada
        List<ItemData> validItems = allItems.FindAll(item => item.rarity == targetRarity);

        // Si por lo que sea no hay ningún ítem configurado con esa rareza exacta, devolvemos uno aleatorio de seguridad
        if (validItems.Count == 0)
        {
            Debug.LogWarning($"No se encontraron ítems con rareza {targetRarity}. Seleccionando uno aleatorio de respaldo.");
            return allItems[Random.Range(0, allItems.Count)];
        }

        // Devolvemos un ítem aleatorio de los que sí coinciden con la rareza
        ItemData selectedItem = validItems[Random.Range(0, validItems.Count)];
        Debug.Log($"[ItemRandom] Rareza: {targetRarity} | Ítem seleccionado: {selectedItem.itemName}");

        return selectedItem;
    }
}