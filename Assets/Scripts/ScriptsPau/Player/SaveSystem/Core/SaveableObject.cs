
using UnityEngine;

public abstract class SaveableObject : MonoBehaviour
{
    public abstract string GetID();

    public abstract SaveData GetSaveData();

    public abstract void LoadData(SaveData data);

    public void NotifySaveChange()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.RegisterChange(this);
        }
    }

    protected virtual void OnEnable()
    {
        NotifySaveChange();
    }

    protected virtual void OnDisable()
    {
        NotifySaveChange();
    }

    protected virtual void OnDestroy()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.UnregisterObject(this);
        }
    }
}
