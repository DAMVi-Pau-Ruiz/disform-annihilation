
using System;
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
    private bool isLoadingData;
    private bool allEnemiesDefeatedNotified;

    private GameObject[] spawnedEnemies;
    private int[] spawnedEnemyIndices;
    private bool[] pointHasSpawned;
    private Vector3[] lastPositions;

    // Se ejecuta cuando todos los enemigos de esta sala han muerto.
    public event Action OnAllEnemiesDefeated;

    public bool HasSpawned => hasSpawned;

    public bool AreAllEnemiesDefeated =>
        hasSpawned && GetAliveEnemyCount() == 0;

    private void Awake()
    {
        if (enemiesSpawnPoints == null)
            enemiesSpawnPoints = new GameObject[0];

        if (enemiesPrefabsToSpawn == null)
            enemiesPrefabsToSpawn = new GameObject[0];

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
            return;

        for (int i = 0; i < spawnedEnemies.Length; i++)
        {
            GameObject enemy = spawnedEnemies[i];

            if (enemy == null)
                continue;

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
            return;

        hasSpawned = true;
        allEnemiesDefeatedNotified = false;

        if (enemiesPrefabsToSpawn == null ||
            enemiesPrefabsToSpawn.Length == 0 ||
            enemiesSpawnPoints == null ||
            enemiesSpawnPoints.Length == 0)
        {
            Debug.LogWarning(
                "El spawner no tiene enemigos o puntos de aparición. " +
                "La sala se considerará completada.",
                this
            );

            NotifySaveChange();
            CheckAllEnemiesDefeated();
            return;
        }

        for (int i = 0; i < enemiesSpawnPoints.Length; i++)
        {
            if (enemiesSpawnPoints[i] == null)
                continue;

            int enemyIndex = UnityEngine.Random.Range(
                0,
                enemiesPrefabsToSpawn.Length
            );

            if (enemiesPrefabsToSpawn[enemyIndex] == null)
                continue;

            GameObject enemy = Instantiate(
                enemiesPrefabsToSpawn[enemyIndex],
                enemiesSpawnPoints[i].transform.position,
                enemiesSpawnPoints[i].transform.rotation
            );

            RegisterEnemy(i, enemy, enemyIndex);
        }

        NotifySaveChange();
        CheckAllEnemiesDefeated();
    }

    private void RegisterEnemy(int index, GameObject enemy, int enemyIndex)
    {
        spawnedEnemies[index] = enemy;
        spawnedEnemyIndices[index] = enemyIndex;
        pointHasSpawned[index] = true;

        lastPositions[index] = enemy.transform.position;

        Enemy_Controller controller =
            enemy.GetComponent<Enemy_Controller>();

        if (controller != null)
        {
            controller.OnStateChanged += () => HandleEnemyChanged(index);
        }
        else
        {
            Debug.LogError(
                "El prefab del enemigo no tiene Enemy_Controller.",
                enemy
            );
        }
    }

    private void HandleEnemyChanged(int index)
    {
        if (isLoadingData)
            return;

        if (index < 0 || index >= spawnedEnemies.Length)
            return;

        GameObject enemy = spawnedEnemies[index];

        if (enemy == null)
            return;

        Enemy_Controller controller =
            enemy.GetComponent<Enemy_Controller>();

        if (controller != null && controller.IsDead())
        {
            spawnedEnemies[index] = null;
        }

        NotifySaveChange();
        CheckAllEnemiesDefeated();
    }

    public int GetAliveEnemyCount()
    {
        int count = 0;

        if (spawnedEnemies == null)
            return 0;

        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy == null)
                continue;

            Enemy_Controller controller =
                enemy.GetComponent<Enemy_Controller>();

            // Si no tiene controlador, lo contamos como vivo
            // para evitar abrir la sala incorrectamente.
            if (controller == null || !controller.IsDead())
                count++;
        }

        return count;
    }

    private void CheckAllEnemiesDefeated()
    {
        if (!hasSpawned || isLoadingData)
            return;

        if (GetAliveEnemyCount() > 0)
            return;

        if (allEnemiesDefeatedNotified)
            return;

        allEnemiesDefeatedNotified = true;

        Debug.Log("¡Todos los enemigos de esta sala han sido derrotados!", this);

        OnAllEnemiesDefeated?.Invoke();
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
                Enemy_Controller controller =
                    enemy.GetComponent<Enemy_Controller>();

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

        isLoadingData = true;

        hasSpawned = data.hasSpawned;
        allEnemiesDefeatedNotified = false;

        for (int i = 0; i < spawnedEnemies.Length; i++)
        {
            if (spawnedEnemies[i] != null)
            {
                Destroy(spawnedEnemies[i]);
                spawnedEnemies[i] = null;
            }

            pointHasSpawned[i] = false;
        }

        int count = Mathf.Min(
            data.enemies.Count,
            enemiesSpawnPoints.Length
        );

        for (int i = 0; i < count; i++)
        {
            EnemySpawnData enemyData = data.enemies[i];

            pointHasSpawned[i] = enemyData.hasSpawned;
            spawnedEnemyIndices[i] = enemyData.enemyIndex;

            if (!enemyData.hasSpawned || !enemyData.isAlive)
                continue;

            if (enemyData.enemyIndex < 0 ||
                enemyData.enemyIndex >= enemiesPrefabsToSpawn.Length ||
                enemiesPrefabsToSpawn[enemyData.enemyIndex] == null)
            {
                Debug.LogError(
                    "No se encuentra el prefab guardado del enemigo."
                );
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

            Enemy_Controller controller =
                enemy.GetComponent<Enemy_Controller>();

            if (controller != null)
            {
                controller.SetCurrentHealth(enemyData.currentHealth);
            }
        }

        // Terminamos de cargar antes de comprobar los enemigos.
        isLoadingData = false;

        gameObject.SetActive(data.isActive);

        CheckAllEnemiesDefeated();
    }
}
