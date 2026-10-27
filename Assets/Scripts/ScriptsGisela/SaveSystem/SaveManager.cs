using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class CollectedItemsData : SaveData
{
    public List<string> collectedIds = new List<string>();
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Auto-Save Config")]
    [SerializeField] private bool enableAutoSave = true;
    [SerializeField] private float autoSaveInterval = 300f; // Cada 5 minutos

    [Header("UI Canvas Feedback")]
    [SerializeField] private GameObject saveNotificationPanel;
    [SerializeField] private float notificationDuration = 2f;

    private Dictionary<string, SaveableEntity> registeredEntities = new Dictionary<string, SaveableEntity>();
    private Dictionary<string, SaveData> temporaryStateBuffer = new Dictionary<string, SaveData>();

    private HashSet<string> collectedItemsIds = new HashSet<string>();
    private const string COLLECTED_ITEMS_BUFFER_KEY = "_GLOBAL_COLLECTED_ITEMS_";

    public static bool IsLoading { get; private set; } = false;

    private void Awake()
    {
        InitializeSingleton();
        LoadGameDataQuietly(); // <- CARGA EN AWAKE: Vital para que los ítems sepan si están recogidos al nacer
    }

    private void Start()
    {
        CheckAndStartAutoSave();
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CheckAndStartAutoSave()
    {
        if (enableAutoSave)
        {
            StartCoroutine(AutoSaveRoutine());
        }
    }

    // Carga silenciosa en Awake antes de que los ítems ejecuten su Start()
    private void LoadGameDataQuietly()
    {
        string path = Application.persistentDataPath + "/savegame.dat";
        if (!File.Exists(path)) return;

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Dictionary<string, SaveData> loadedData = formatter.Deserialize(stream) as Dictionary<string, SaveData>;
            stream.Close();

            if (loadedData != null && loadedData.ContainsKey(COLLECTED_ITEMS_BUFFER_KEY))
            {
                if (loadedData[COLLECTED_ITEMS_BUFFER_KEY] is CollectedItemsData itemsData)
                {
                    collectedItemsIds = new HashSet<string>(itemsData.collectedIds);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[SaveManager] Error leyendo datos en Awake: " + e.Message);
        }
    }

    public void RegisterEntity(SaveableEntity entity)
    {
        if (!registeredEntities.ContainsKey(entity.UniqueId))
        {
            registeredEntities.Add(entity.UniqueId, entity);
        }
    }

    public void UnregisterEntity(SaveableEntity entity)
    {
        if (registeredEntities.ContainsKey(entity.UniqueId))
        {
            registeredEntities.Remove(entity.UniqueId);
        }
    }

    public void RecordChange(SaveableEntity entity)
    {
        if (entity != null)
        {
            temporaryStateBuffer[entity.UniqueId] = entity.CaptureState();
        }
    }

    public void MarkItemAsCollected(string itemId)
    {
        if (!collectedItemsIds.Contains(itemId))
        {
            collectedItemsIds.Add(itemId);
        }
    }

    public bool IsItemCollected(string itemId)
    {
        return collectedItemsIds.Contains(itemId);
    }

    public void CommitSave()
    {
        ActualizarBufferCompleto();
        GuardarEnDiscoBinario(temporaryStateBuffer);
        Debug.Log("[SaveManager] Guardado definitivo completado con F5.");
        TriggerSaveNotification();
    }

    private void ActualizarBufferCompleto()
    {
        foreach (var kvp in registeredEntities)
        {
            temporaryStateBuffer[kvp.Key] = kvp.Value.CaptureState();
        }

        CollectedItemsData itemsData = new CollectedItemsData();
        itemsData.sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        itemsData.collectedIds = new List<string>(collectedItemsIds);
        temporaryStateBuffer[COLLECTED_ITEMS_BUFFER_KEY] = itemsData;
    }

    private void GuardarEnDiscoBinario(Dictionary<string, SaveData> dataToSave)
    {
        string path = Application.persistentDataPath + "/savegame.dat";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, dataToSave);
        stream.Close();
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/savegame.dat";
        if (!File.Exists(path))
        {
            Debug.LogWarning("[SaveManager] No se encontró archivo de guardado.");
            return;
        }

        StartCoroutine(LoadRoutine(path));
    }

    private IEnumerator LoadRoutine(string path)
    {
        IsLoading = true;

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);
        Dictionary<string, SaveData> loadedData = formatter.Deserialize(stream) as Dictionary<string, SaveData>;
        stream.Close();

        yield return null;

        RestaurarDatosCargados(loadedData);

        IsLoading = false;
        Debug.Log("[SaveManager] Partida cargada y restaurada correctamente.");
        TriggerSaveNotification();
    }

    private void RestaurarDatosCargados(Dictionary<string, SaveData> loadedData)
    {
        foreach (var kvp in loadedData)
        {
            if (kvp.Key == COLLECTED_ITEMS_BUFFER_KEY)
            {
                if (kvp.Value is CollectedItemsData itemsData)
                {
                    collectedItemsIds = new HashSet<string>(itemsData.collectedIds);
                }
            }
            else if (registeredEntities.ContainsKey(kvp.Key))
            {
                registeredEntities[kvp.Key].RestoreState(kvp.Value);
            }
        }

        // Destruimos al instante cualquier ítem presente en la escena que ya conste como recogido
        ItemCollectable[] allItems = FindObjectsByType<ItemCollectable>(FindObjectsSortMode.None);
        foreach (var item in allItems)
        {
            if (item != null && item.IsCollectedInSave(collectedItemsIds))
            {
                Destroy(item.gameObject);
            }
        }
    }

    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveInterval);
            CommitSave();
        }
    }

    private void TriggerSaveNotification()
    {
        if (saveNotificationPanel != null)
        {
            StopAllCoroutines();
            StartCoroutine(ShowNotificationRoutine());
        }
    }

    private IEnumerator ShowNotificationRoutine()
    {
        saveNotificationPanel.SetActive(true);
        yield return new WaitForSeconds(notificationDuration);
        saveNotificationPanel.SetActive(false);
    }
}