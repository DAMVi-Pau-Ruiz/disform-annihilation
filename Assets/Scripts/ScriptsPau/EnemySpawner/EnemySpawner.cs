
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemigos")]
    [SerializeField] private GameObject[] enemiesPrefabsToSpawn;

    [Header("Spawn points")]
    [SerializeField] private GameObject[] enemiesSpawnPoints;

    private bool hasSpawned;

    public void SpawnEnemies()
    {
        // Evitar generar enemigos varias veces.
        if (hasSpawned)
        {
            return;
        }

        if (enemiesPrefabsToSpawn == null || enemiesPrefabsToSpawn.Length == 0 || enemiesSpawnPoints == null || enemiesSpawnPoints.Length == 0)
        {
            return;
        }

        for (int i = 0; i < enemiesSpawnPoints.Length; i++)
        {
            if (enemiesSpawnPoints[i] == null)
            {
                continue;
            }

            int enemyIndex = Random.Range(0, enemiesPrefabsToSpawn.Length);

            if (enemiesPrefabsToSpawn[enemyIndex] == null)
            {
                continue;
            }

            Instantiate(enemiesPrefabsToSpawn[enemyIndex], enemiesSpawnPoints[i].transform.position, enemiesSpawnPoints[i].transform.rotation);
        }

        hasSpawned = true;
    }
}
