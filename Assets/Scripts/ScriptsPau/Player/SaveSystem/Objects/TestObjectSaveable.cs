using UnityEngine;

public class TestObjectSaveable : SaveableObject
{
    [SerializeField] private string id = "TEST_OBJECT";

    public override string GetID()
    {
        return id;
    }

    public override SaveData GetSaveData()
    {
        TestObjectSaveData data = new TestObjectSaveData();

        data.isActive = gameObject.activeSelf;

        return data;
    }

    public override void LoadData(SaveData saveData)
    {
        TestObjectSaveData data = saveData as TestObjectSaveData;

        if (data == null)
        {
            Debug.LogError("Los datos no corresponden a TestObject.");
            return;
        }

        gameObject.SetActive(data.isActive);
    }
}