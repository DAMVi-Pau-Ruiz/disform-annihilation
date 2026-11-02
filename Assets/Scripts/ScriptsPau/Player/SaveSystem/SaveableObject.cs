using UnityEngine;

public abstract class SaveableObject : MonoBehaviour
{
    public abstract string GetID();

    public abstract SaveData GetSaveData();

    public abstract void LoadData(SaveData data);
}