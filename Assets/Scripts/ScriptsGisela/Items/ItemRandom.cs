using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ItemRandom : MonoBehaviour
{
    private List<int> itemIds = new List<int>();
    // Arrays de los ID's, Tag's, Descripciones, gambling e índice
    private List<string> itemTag = new List<string>();
    private List<string> itemDescription = new List<string>();
    private List<string> itemGambling = new List<string>();
    private List<int> validIndex = new List<int>();
    private string filePath;

    [SerializeField] private string defaultRarity = "comun";

    //  métodos para que la UI pueda leer el resultado actual
    [HideInInspector] public string lastSelectedTag = "";
    [HideInInspector] public string lastSelectedDesc = "";
    [HideInInspector] public string lastSelectedRarity = "";
    [HideInInspector] public int lastSelectedId = 0;
    void Start()
    {
        // Llamamos a GetRandomRarity 100 veces para pruebas 
        for (int i = 0; i < 100; i++)
        {
            GetRandomRarity();
        }
    }

    void Awake()
    {
        // Ruta para guardarlo
        filePath = Path.Combine(Application.persistentDataPath, "ItemSystem.csv");

        CreateDefaultCsvFile();
        ReadCsvFile();
    }

    void CreateDefaultCsvFile()
    {
        string csvContent = "ID,Tag,Description,Rareza\n" +
                            "1,curar,este objeto sana la vida que te han quitado,comun\n" +
                            "2,daño,te hace pupa si aprietas bobo,raro\n" +
                            "3,muerte,te mata,legendary\n" +
                            "4,revivir,revive por si te has matado,epico\n";

        File.WriteAllText(filePath, csvContent);
    }

    void ReadCsvFile()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("No se encuentra el archivo CSV en la ruta: " + filePath);
            return;
        }

        itemIds.Clear();
        itemTag.Clear();
        itemDescription.Clear();
        itemGambling.Clear();

        string[] lines = File.ReadAllLines(filePath);

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] parts = lines[i].Split(',');
            if (parts.Length >= 4)
            {
                if (int.TryParse(parts[0].Trim(), out int id))
                {
                    string tag = parts[1].Trim();
                    string description = parts[2].Trim();
                    string gambling = parts[3].Trim().ToLower();

                    itemIds.Add(id);
                    itemTag.Add(tag);
                    itemDescription.Add(description);
                    itemGambling.Add(gambling);
                }
            }
        }
    }

    public string GetRandomRarity()
    {
        int roll = Random.Range(1, 101);

        if (roll <= 45)
        {
            return defaultRarity; // 45% (comun)
        }
        else if (roll <= 80)
        {
            return "raro"; // 35% (raro)
        }
        else if (roll <= 95)
        {
            return "epico"; // 15% (epico)
        }
        else
        {
            return "legendary"; // 5% (legendary)
        }
    }

    // Muestra un único item y guarda los datos para la UI
    public void ItemProbability()
    {
        if (itemIds.Count == 0)
        {
            Debug.LogWarning("No hay ítems guardados.");
            return;
        }

        validIndex.Clear();

        string selectedRarity = GetRandomRarity();
        lastSelectedRarity = selectedRarity;

        for (int i = 0; i < itemGambling.Count; i++)
        {
            if (itemGambling[i].Trim().ToLower() == selectedRarity.Trim().ToLower())
            {
                validIndex.Add(i);
            }
        }

        if (validIndex.Count == 0)
        {
            for (int i = 0; i < itemIds.Count; i++)
            {
                validIndex.Add(i);
            }
        }

        if (validIndex.Count > 0)
        {
            int randomIndex = validIndex[Random.Range(0, validIndex.Count)];

            int id = itemIds[randomIndex];
            lastSelectedId = id;
            lastSelectedTag = itemTag[randomIndex];
            lastSelectedDesc = itemDescription[randomIndex];
            string rarity = itemGambling[randomIndex];

            Debug.Log($"RESULTADO Rareza: {selectedRarity} | ID: {id} | Tag: {lastSelectedTag} | Desc: {lastSelectedDesc}");
        }
    }

}