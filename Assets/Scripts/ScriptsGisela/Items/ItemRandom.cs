using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ItemRandom : MonoBehaviour
{
    private List<int> itemIds = new List<int>();
    //Arrays de los ID's, Tag's, Descripciones, gambling e índice
    private List<string> itemTag = new List<string>();
    private List<string> itemDescription = new List<string>();
    private List<string> itemGambling = new List<string>();
    private List<int> validIndex = new List<int>();
    private string filePath;

    
    [SerializeField] private string defaultRarity = "comun";

    void Start()
    {
        // LLamamos a GetRandomRarity 100 veces para ver cuántas veces sale cada una
        for (int i = 0; i < 100; i++)
        {
            GetRandomRarity();
        }
    }

    void Awake()
    {
        //ruta para guardarlo
        filePath = Path.Combine(Application.persistentDataPath, "ItemSystem.csv");

        CreateDefaultCsvFile();
        ReadCsvFile();
    }

    void CreateDefaultCsvFile()
    {
        //el default de nuestro csv
        string csvContent = "ID,Tag,Description,Rareza\n" +
                            "1,curar,este objeto sana la vida que te han quitado,comun\n" +
                            "2,daño,te hace pupa si aprietas bobo,raro\n" +
                            "3,muerte,te mata,legendary\n" +
                            "4,revivir,revive por si te has matado,epico\n";

        File.WriteAllText(filePath, csvContent);
    }

    void ReadCsvFile()
    {
        //lectura de csv donde si no existe nos lo dirá
        if (!File.Exists(filePath))
        {
            Debug.LogError("No se encuentra el archivo CSV en la ruta: " + filePath);
            return;
        }
        //limpiamos para evitar duplicados y asegurar optimización
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
                    //recorremos los elementos y los agregamos
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
        // Obtención de rareza donde el rango será de 1 a 100
        int roll = Random.Range(1, 101);

        if (roll <= 45)
        {
            Debug.Log(defaultRarity);
        }  // 45% (comun)
        else if (roll <= 80)
        {
            Debug.Log("raro");
        }// 35% (raro)
        else if (roll <= 95)
        {
            Debug.Log("epico");
        }// 15% (epico)
        else
        {
            Debug.Log("Legendary");// 5% (legendary)
        }
        return "si";
    
    }
    // Muestra un único item
    public void ItemProbability()
    {
        if (itemIds.Count == 0)
        {
            Debug.LogWarning("No hay ítems guardados.");
            return;
        }

        validIndex.Clear();

        // Obtenemos la rareza real calculada
        string selectedRarity = GetRandomRarity();

        // Buscamos qué posiciones del CSV coinciden con esa rareza
        for (int i = 0; i < itemGambling.Count; i++)
        {
            if (itemGambling[i].Trim().ToLower() == selectedRarity.Trim().ToLower())
            {
                validIndex.Add(i);
            }
        }

        // Si no hay ítems de esa rareza exacta, usamos todos como respaldo
        if (validIndex.Count == 0)
        {
            for (int i = 0; i < itemIds.Count; i++)
            {
                validIndex.Add(i);
            }
        }

        // Si encontramos ítems válidos, seleccionamos uno al azar y lo imprimimos
        if (validIndex.Count > 0)
        {
            int randomIndex = validIndex[Random.Range(0, validIndex.Count)];

            int id = itemIds[randomIndex];
            string tag = itemTag[randomIndex];
            string desc = itemDescription[randomIndex];
            string rarity = itemGambling[randomIndex];

            Debug.Log($"[RESULTADO DE PROBABILIDAD] rareza seleccionada: {selectedRarity} | Ítem obtenido -> ID: {id} | Tag: {tag} | Descripción: {desc} | Rareza CSV: {rarity}");
        }
    }

}