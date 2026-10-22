using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    private bool hasSpawned;
    [Header("Referencias")]
    [SerializeField] private ItemRandom itemRandom; 
    // Arrastra aquí el objeto con el script ItemRandom

    [Header("Puntos de Spawn (Exactamente 4)")]
    [SerializeField] private Transform[] spawnPoints = new Transform[4]; // Tus 4 puntos en el plano

    [System.Serializable]
    public struct ItemPrefabMapping
    {
        public int itemId;          
        // 1: curar, 2: daño, 3: muerte, 4: revivir
        public GameObject prefab;   
        // El prefab físico (cuadrado, círculo, etc.)
    }

    [Header("Prefabs de los Ítems en el Mundo")]
    [SerializeField] private List<ItemPrefabMapping> itemPrefabsList;

    public void GenerarItemsEnSpawnPoints()
    {
        if (hasSpawned)
        {
            return;
        }

        // Recorremos los 4 puntos de spawn de manera independiente
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint == null) continue;

            // 1. Calculamos la probabilidad para ESTE punto específico
            int idObtenido = itemRandom.ItemProbability();

            // 2. Buscamos el prefab correspondiente a ese ID
            GameObject prefabAInstanciar = ObtenerPrefabPorId(idObtenido);

            if (prefabAInstanciar != null)
            {
                // 3. Instanciamos el ítem en la posición del spawn point
                Instantiate(prefabAInstanciar, spawnPoint.position, spawnPoint.rotation);
                Debug.Log($"[Spawn] Ítem ID {idObtenido} generado en {spawnPoint.name}");
            }
           
        }
    }

    private GameObject ObtenerPrefabPorId(int id)
    {
        foreach (var mapping in itemPrefabsList)
        {
            if (mapping.itemId == id)
            {
                return mapping.prefab;
            }
        }
        return null;
    }
}