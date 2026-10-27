using UnityEngine;

[DisallowMultipleComponent]
public abstract class SaveableEntity : MonoBehaviour
{
    [SerializeField] private string uniqueId;

    public string UniqueId => uniqueId;

    // CAMBIADO A PROTECTED VIRTUAL PARA QUE LAS CLASES HIJAS PUEDAN ACCEDER Y EXTENDERLO
    protected virtual void Awake()
    {
        if (string.IsNullOrEmpty(uniqueId))
        {
            uniqueId = System.Guid.NewGuid().ToString();
        }
    }

    private void OnEnable()
    {
        SaveManager.Instance?.RegisterEntity(this);
    }

    private void OnDisable()
    {
        SaveManager.Instance?.UnregisterEntity(this);
    }

    public abstract SaveData CaptureState();
    public abstract void RestoreState(SaveData data);
}