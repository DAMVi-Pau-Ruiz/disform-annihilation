using NUnit.Framework;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemigos")]
    [SerializeField] private GameObject[] enemiesPrefabsToSpawn;

    [Header("Spawn points")]
    [SerializeField] private GameObject[] enemiesSpawnPoints;

    private void SpawnEnemies()
    {

    }
}
