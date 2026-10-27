[System.Serializable]
public class SaveData
{
    public string uniqueId;
    public string sceneName;
}

[System.Serializable]
public class PlayerSaveData : SaveData
{
    public float[] position = new float[3]; // x, y, z
    public float[] rotation = new float[4]; // x, y, z, w
    public float currentHealth;
}

[System.Serializable]
public class GenericObjectSaveData : SaveData
{
    public bool isActive;
}