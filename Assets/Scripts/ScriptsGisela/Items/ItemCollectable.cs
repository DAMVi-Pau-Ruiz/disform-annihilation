using UnityEngine;

public class ItemCollectable : MonoBehaviour
{
    [Header("Datos del Ítem (Asignado automáticamente por el Spawner)")]
    public ItemData itemData;

    private bool yaRecogido = false;
    private string uniqueItemId;

    private void Awake()
    {
        if (itemData == null)
        {
            Debug.LogError($"[Error] El ítem '{gameObject.name}' NO tiene ningún ScriptableObject (itemData) asignado en el Inspector.");
            return;
        }

        // ID único basado en el nombre del ScriptableObject + posición redondeada a 2 decimales
        Vector3 pos = transform.position;
        string roundedPos = $"({Mathf.Round(pos.x * 100f) / 100f}, {Mathf.Round(pos.y * 100f) / 100f}, {Mathf.Round(pos.z * 100f) / 100f})";

        uniqueItemId = itemData.name + "_" + roundedPos;
    }

    private void Start()
    {
        // Comprobamos al arrancar si ya estaba guardado como recogido
        if (SaveManager.Instance != null && SaveManager.Instance.IsItemCollected(uniqueItemId))
        {
            Destroy(gameObject); // Se autodestruye al instante si ya fue recogido y guardado
        }
    }

    public bool IsCollectedInSave(System.Collections.Generic.HashSet<string> collectedSet)
    {
        return collectedSet.Contains(uniqueItemId);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (yaRecogido) return;

        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        if (playerStats == null) playerStats = other.GetComponentInParent<PlayerStats>();
        if (playerStats == null) playerStats = other.GetComponentInChildren<PlayerStats>();

        if (playerStats != null)
        {
            yaRecogido = true;

            if (itemData == null)
            {
                Destroy(gameObject);
                return;
            }

            Debug.Log($"¡Ítem recogido correctamente: {itemData.itemName}!");

            // 1. Aplicamos modificadores
            ApplyItemEffects(playerStats);

            // 2. Registramos en memoria (SIN GUARDAR EN DISCO TODAVÍA)
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.MarkItemAsCollected(uniqueItemId);
            }

            // 3. Destruimos el ítem de la escena actual
            Destroy(gameObject);
        }
    }

    private void ApplyItemEffects(PlayerStats playerStats)
    {
        if (itemData.healthModifier != 0)
        {
            if (itemData.healthModifier > 0) playerStats.AddHealth(itemData.healthModifier);
            else playerStats.GetHurt(Mathf.Abs(itemData.healthModifier));
        }

        if (itemData.damageModifier != 0) playerStats.AddDamageMultiplier(itemData.damageModifier);

        if (itemData.bulletQuantityModifier != 0)
        {
            if (itemData.bulletQuantityModifier > 0) playerStats.AddBulletsPerSecond(itemData.bulletQuantityModifier);
            else playerStats.SubsBulletsPerSecond(Mathf.Abs(itemData.bulletQuantityModifier));
        }

        if (itemData.bulletSpeedModifier != 0)
        {
            if (itemData.bulletSpeedModifier > 0) playerStats.AddBulletSpeed(itemData.bulletSpeedModifier);
            else playerStats.SubsBulletSpeed(Mathf.Abs(itemData.bulletSpeedModifier));
        }

        if (itemData.shrinksCollision)
        {
            playerStats.transform.localScale = Vector3.one * 0.5f;
        }
    }
}