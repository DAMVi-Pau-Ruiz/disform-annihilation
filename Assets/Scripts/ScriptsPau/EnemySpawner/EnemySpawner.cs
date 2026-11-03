using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : SaveableObject
{
    [Header("Save Settings")]
    [SerializeField] private string id = "ENEMY_SPAWNER_01";

    [Header("Enemigos")]
    [SerializeField] private GameObject[] enemiesPrefabsToSpawn;

    [Header("Spawn points")]
    [SerializeField] private GameObject[] enemiesSpawnPoints;

    private bool hasSpawned;

    private GameObject[] spawnedEnemies;
    private int[] spawnedEnemyIndices;
    private bool[] pointHasSpawned;
    private Vector3[] lastPositions;

    private void Awake()
    {
        if (enemiesSpawnPoints == null)
        {
            enemiesSpawnPoints = new GameObject[0];
        }

        if (enemiesPrefabsToSpawn == null)
        {
            enemiesPrefabsToSpawn = new GameObject[0];
        }

        int count = enemiesSpawnPoints.Length;

        spawnedEnemies = new GameObject[count];
        spawnedEnemyIndices = new int[count];
        pointHasSpawned = new bool[count];
        lastPositions = new Vector3[count];
    }

    public override string GetID()
    {
        return id;
    }

    private void LateUpdate()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.IsLoading)
        {
            return;
        }

        for (int i = 0; i < spawnedEnemies.Length; i++)
        {
            GameObject enemy = spawnedEnemies[i];

            if (enemy == null)
            {
                continue;
            }

            if (Vector3.Distance(enemy.transform.position, lastPositions[i]) > 0.05f)
            {
                lastPositions[i] = enemy.transform.position;
                NotifySaveChange();
            }
        }
    }

    public void SpawnEnemies()
    {
        if (hasSpawned)
        {
            return;
        }

        if (enemiesPrefabsToSpawn == null ||
            enemiesPrefabsToSpawn.Length == 0 ||
            enemiesSpawnPoints == null ||
            enemiesSpawnPoints.Length == 0)
        {
            return;
        }

        hasSpawned = true;

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

            GameObject enemy = Instantiate(
                enemiesPrefabsToSpawn[enemyIndex],
                enemiesSpawnPoints[i].transform.position,
                enemiesSpawnPoints[i].transform.rotation
            );

            RegisterEnemy(i, enemy, enemyIndex);
        }

        NotifySaveChange();
    }

    private void RegisterEnemy(int index, GameObject enemy, int enemyIndex)
    {
        spawnedEnemies[index] = enemy;
        spawnedEnemyIndices[index] = enemyIndex;
        pointHasSpawned[index] = true;

        lastPositions[index] = enemy.transform.position;

        Enemy_Controller controller = enemy.GetComponent<Enemy_Controller>();

        if (controller != null)
        {
            controller.OnStateChanged += () => HandleEnemyChanged(index);
        }
        else
        {
            Debug.LogError("El prefab del enemigo no tiene Enemy_Controller.", enemy);
        }
    }

    private void HandleEnemyChanged(int index)
    {
        if (spawnedEnemies[index] == null)
        {
            return;
        }

        Enemy_Controller controller =
            spawnedEnemies[index].GetComponent<Enemy_Controller>();

        if (controller != null && controller.IsDead())
        {
            spawnedEnemies[index] = null;
        }

        NotifySaveChange();
    }

    public override SaveData GetSaveData()
    {
        EnemySpawnerSaveData data = new EnemySpawnerSaveData();

        data.isActive = gameObject.activeSelf;
        data.hasSpawned = hasSpawned;

        for (int i = 0; i < enemiesSpawnPoints.Length; i++)
        {
            EnemySpawnData enemyData = new EnemySpawnData();

            enemyData.hasSpawned = pointHasSpawned[i];
            enemyData.enemyIndex = spawnedEnemyIndices[i];

            GameObject enemy = spawnedEnemies[i];

            if (enemy != null)
            {
                Enemy_Controller controller = enemy.GetComponent<Enemy_Controller>();

                if (controller != null && !controller.IsDead())
                {
                    enemyData.isAlive = true;
                    enemyData.position = enemy.transform.position;
                    enemyData.currentHealth = controller.GetCurrentHealth();
                }
            }

            data.enemies.Add(enemyData);
        }

        return data;
    }

    public override void LoadData(SaveData saveData)
    {
        EnemySpawnerSaveData data = saveData as EnemySpawnerSaveData;

        if (data == null)
        {
            Debug.LogError("Los datos no corresponden a un EnemySpawner.");
            return;
        }

        hasSpawned = data.hasSpawned;

        for (int i = 0; i < spawnedEnemies.Length; i++)
        {
            if (spawnedEnemies[i] != null)
            {
                Destroy(spawnedEnemies[i]);
                spawnedEnemies[i] = null;
            }

            pointHasSpawned[i] = false;
        }

        int count = Mathf.Min(data.enemies.Count, enemiesSpawnPoints.Length);

        for (int i = 0; i < count; i++)
        {
            EnemySpawnData enemyData = data.enemies[i];

            pointHasSpawned[i] = enemyData.hasSpawned;
            spawnedEnemyIndices[i] = enemyData.enemyIndex;

            // Este punto nunca llegó a generar un enemigo.
            if (!enemyData.hasSpawned)
            {
                continue;
            }

            // El enemigo fue derrotado antes de guardar.
            if (!enemyData.isAlive)
            {
                continue;
            }

            if (enemyData.enemyIndex < 0 ||
                enemyData.enemyIndex >= enemiesPrefabsToSpawn.Length ||
                enemiesPrefabsToSpawn[enemyData.enemyIndex] == null)
            {
                Debug.LogError("No se encuentra el prefab guardado del enemigo.");
                continue;
            }

            GameObject enemy = Instantiate(
                enemiesPrefabsToSpawn[enemyData.enemyIndex],
                enemyData.position,
                enemiesSpawnPoints[i] != null
                    ? enemiesSpawnPoints[i].transform.rotation
                    : Quaternion.identity
            );

            RegisterEnemy(i, enemy, enemyData.enemyIndex);

            Enemy_Controller controller = enemy.GetComponent<Enemy_Controller>();

            if (controller != null)
            {
                controller.SetCurrentHealth(enemyData.currentHealth);
            }
        }

        gameObject.SetActive(data.isActive);
    }
}