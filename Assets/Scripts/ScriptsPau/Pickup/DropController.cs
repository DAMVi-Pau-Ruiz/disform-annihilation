using System;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour
{
    [Serializable]
    public class Drop
    {
        public GameObject prefab;

        [Range(0, 100)]
        public float probability = 50;
    }

    [Header("Drop Settings")]
    [SerializeField] private List<Drop> drops = new List<Drop>();

    [Header("Random Item")]
    [SerializeField] private ItemRandom itemRandom;
    [Range(0, 100)]
    [SerializeField] private float itemProbability = 1;

    [Header("Spawn Settings")]
    [SerializeField] private Transform dropPoint;
    [SerializeField] private float spawnRadius = 0.5f;

    private bool hasDropped;

    public void GenerateDrops()
    {
        if (hasDropped)
        {
            return;
        }

        hasDropped = true;

        Vector3 spawnPosition = dropPoint != null ? dropPoint.position : transform.position;

        foreach (Drop drop in drops)
        {
            if (drop.prefab == null)
            {
                continue;
            }

            if (UnityEngine.Random.Range(0, 100) < drop.probability)
            {
                SpawnDrop(drop.prefab, spawnPosition);
            }
        }

        if (UnityEngine.Random.Range(0, 100) < itemProbability)
        {
            GenerateRandomItem(spawnPosition);
        }
    }

    private void SpawnDrop(GameObject prefab, Vector3 spawnPosition)
    {
        Vector2 offset = UnityEngine.Random.insideUnitCircle * spawnRadius;

        Vector3 position = spawnPosition + new Vector3(offset.x, 0, offset.y);

        Instantiate(prefab, position, Quaternion.identity);
    }

    private void GenerateRandomItem(Vector3 position)
    {
        if (itemRandom == null)
        {
            return;
        }

        ItemData selectedItem = itemRandom.GetRandomItemByRarity();

        if (selectedItem == null || selectedItem.worldPrefab == null)
        {
            return;
        }

        SpawnDrop(selectedItem.worldPrefab, position);
    }

    public void ResetDrops()
    {
        hasDropped = false;
    }
}
