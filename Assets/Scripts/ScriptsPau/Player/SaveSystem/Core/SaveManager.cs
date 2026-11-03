
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class SaveManager : MonoBehaviour
{
    [Header("Auto Save")]
    [SerializeField] private bool autoSaveEnabled = true;
    [SerializeField] private float autoSaveInterval = 60f;

    private float autoSaveTimer;
    private bool hasPendingChanges;

    private bool isLoading = true;
    public bool IsLoading => isLoading;

    public static SaveManager Instance { get; private set; }

    private BinarySaveSystem binarySaveSystem;

    private Dictionary<string, SaveData> temporaryData =
        new Dictionary<string, SaveData>();

    private Dictionary<string, SaveableObject> saveableObjects =
        new Dictionary<string, SaveableObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // La carga comienza antes de los Start de los demás scripts.
        isLoading = true;

        binarySaveSystem = new BinarySaveSystem();
    }

    private IEnumerator Start()
    {
        yield return LoadGameRoutine();
    }

    private void Update()
    {
        if (!autoSaveEnabled || isLoading)
        {
            return;
        }

        autoSaveTimer += Time.deltaTime;

        if (autoSaveTimer >= autoSaveInterval)
        {
            autoSaveTimer = 0f;

            if (hasPendingChanges)
            {
                SaveGame();
            }
        }
    }

    private void RegisterSceneObjects()
    {
        SaveableObject[] objects = FindObjectsByType<SaveableObject>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (SaveableObject saveableObject in objects)
        {
            RegisterObject(saveableObject);
        }
    }

    private IEnumerator LoadGameRoutine()
    {
        if (isLoading && binarySaveSystem == null)
        {
            yield break;
        }

        isLoading = true;

        RegisterSceneObjects();

        Dictionary<string, SaveData> loadedData = binarySaveSystem.Load();

        foreach (KeyValuePair<string, SaveData> entry in loadedData)
        {
            if (saveableObjects.TryGetValue(entry.Key, out SaveableObject saveableObject))
            {
                if (saveableObject != null)
                {
                    saveableObject.LoadData(entry.Value);
                }
            }
            else
            {
                Debug.LogWarning("No se ha encontrado el objeto con ID: " + entry.Key);
            }
        }

        // Dejamos que Unity procese los cambios de activación.
        yield return null;

        temporaryData.Clear();

        foreach (KeyValuePair<string, SaveableObject> entry in saveableObjects)
        {
            if (entry.Value != null)
            {
                temporaryData[entry.Key] = entry.Value.GetSaveData();
            }
        }

        hasPendingChanges = false;
        isLoading = false;

        Debug.Log("Inicialización y carga completadas.");
    }

    public void RegisterChange(SaveableObject saveableObject)
    {
        if (saveableObject == null || isLoading)
        {
            return;
        }

        if (!RegisterObject(saveableObject))
        {
            return;
        }

        string id = saveableObject.GetID();

        temporaryData[id] = saveableObject.GetSaveData();

        hasPendingChanges = true;
    }


    public void SaveGame()
    {
        if (isLoading)
        {
            Debug.LogWarning("No se puede guardar mientras se carga la partida.");
            return;
        }

        foreach (KeyValuePair<string, SaveableObject> entry in saveableObjects)
        {
            if (entry.Value != null)
            {
                temporaryData[entry.Key] = entry.Value.GetSaveData();
            }
        }

        bool saveSuccessful = binarySaveSystem.Save(temporaryData);

        if (saveSuccessful)
        {
            hasPendingChanges = false;
            Debug.Log("Partida guardada correctamente.");
        }
        else
        {
            hasPendingChanges = true;
            Debug.LogError("El guardado ha fallado. Se volverá a intentar.");
        }
    }


    public void LoadGame()
    {
        if (isLoading)
        {
            Debug.LogWarning("Ya hay un proceso de carga en marcha.");
            return;
        }

        StartCoroutine(LoadGameRoutine());
    }

    public bool RegisterObject(SaveableObject saveableObject)
    {
        if (saveableObject == null)
        {
            return false;
        }

        string id = saveableObject.GetID();

        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("Un objeto tiene un ID vacío.", saveableObject);
            return false;
        }

        if (saveableObjects.TryGetValue(id, out SaveableObject registeredObject))
        {
            if (registeredObject == saveableObject)
            {
                return true;
            }

            Debug.LogError("ID duplicado: " + id, saveableObject);
            return false;
        }

        saveableObjects.Add(id, saveableObject);

        return true;
    }

    public void UnregisterObject(SaveableObject saveableObject)
    {
        if (saveableObject == null)
        {
            return;
        }

        string id = saveableObject.GetID();

        if (saveableObjects.TryGetValue(id, out SaveableObject registeredObject) &&
            registeredObject == saveableObject)
        {
            saveableObjects.Remove(id);
        }
    }
}
