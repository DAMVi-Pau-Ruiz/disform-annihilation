using UnityEngine;

public enum ItemRarity
{
    // estados según su rareza
    Common,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Información Básica")]
    public int itemId;
    public string itemName;
    [TextArea] public string description;
    public ItemRarity rarity;
    public GameObject worldPrefab; // Prefab físico que aparece en el mapa
    public GameObject uiImagePrefab; // Icono para la UI

    [Header("Modificadores de Estadísticas")]
    public float bulletSpeedModifier = 0f;    // Ej: +1 o -1
    public float bulletQuantityModifier = 0f; // Cadencia / Cantidad
    public float damageModifier = 0f;         // Multiplicador de daño
    public float healthModifier = 0f;         // Curación o daño directo
    public bool grantsShield = false;         // Escudo especial (quita mitad de vida para romper)
    public bool shrinksCollision = false;     // Colisión pequeña
}