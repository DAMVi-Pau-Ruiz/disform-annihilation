using UnityEngine;
using System.Collections.Generic;
public class ItemRandom : MonoBehaviour
{
    private List<int> itemIds = new List<int>();
    private List<string> itemTag = new List<string>();
    void Awake()
    {
        TextAsset file = Resources.Load<TextAsset>("Items/ItemSystem");
        if (file == null)
        {
            Debug.LogError("No se encuentra items.csv");
            return;
        }

        string[] lines = file.text.Split(
            new[] { "\r\n", "\n" },
            System.StringSplitOptions.RemoveEmptyEntries
        );

        for (int i = 1; i < lines.Length; i++)
        {
            if (int.TryParse(lines[i].Trim(), out int id))
            {
                itemTag.Add(tag);
                itemIds.Add(id);
            }
        }

        Debug.Log("IDs cargados: " + itemIds.Count);
    }

    public int GetRandomItemId()
    {
        if (itemIds.Count == 0)
            return -1;

        int index = Random.Range(0, itemIds.Count);
        return itemIds[index];
    }
}