using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ItemRandom itemRandom;
    [SerializeField] private Transform[] spawnPoints = new Transform[4];
    private bool hasSpawned = false;

    public void GenerarItemsEnSpawnPoints()
    {
        if (hasSpawned) return;

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint == null) continue;

            ItemData selectedItem = itemRandom.GetRandomItemByRarity();

            if (selectedItem != null && selectedItem.worldPrefab != null)
            {
                GameObject instantiatedItem = Instantiate(selectedItem.worldPrefab, spawnPoint.position, spawnPoint.rotation);

                // Aseguramos que el prefab instanciado tenga la referencia al itemData
                ItemCollectable collectable = instantiatedItem.GetComponent<ItemCollectable>();
                if (collectable != null)
                {
                    collectable.itemData = selectedItem;
                }

                Debug.Log($"[Spawn] Ítem '{selectedItem.itemName}' ({selectedItem.rarity}) generado.");
            }
        }
        hasSpawned = true;
    }
}