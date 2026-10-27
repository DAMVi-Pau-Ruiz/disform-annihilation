using UnityEngine;

public class ItemCollectable : MonoBehaviour
{
    [Header("Datos del Ítem (Asignado automáticamente por el Spawner)")]
    public ItemData itemData;

    private bool yaRecogido = false;

    private void OnTriggerEnter(Collider other)
    {
        if (yaRecogido) return;

        // Comprobamos si el objeto o su jerarquía pertenecen al Player
        // Buscamos PlayerStats directamente
        PlayerStats playerStats = other.GetComponent<PlayerStats>();

        if (playerStats == null)
        {
            // Si no está en el mismo objeto, lo buscamos en los padres o hijos por si a caso
            playerStats = other.GetComponentInParent<PlayerStats>();
        }

        if (playerStats == null)
        {
            playerStats = other.GetComponentInChildren<PlayerStats>();
        }

        // Si después de buscar por todas partes encontramos el PlayerStats:
        if (playerStats != null)
        {
            yaRecogido = true;

            if (itemData == null)
            {
                Debug.LogError($"[Error] El ítem '{gameObject.name}' NO tiene ningún ScriptableObject (itemData) asignado.");
                Destroy(gameObject);
                return;
            }

            Debug.Log($"¡Ítem recogido correctamente: {itemData.itemName}!");

            // Aplicamos los modificadores
            if (itemData.healthModifier != 0)
            {
                if (itemData.healthModifier > 0)
                    playerStats.AddHealth(itemData.healthModifier);
                else
                    playerStats.GetHurt(Mathf.Abs(itemData.healthModifier));
            }

            if (itemData.damageModifier != 0)
            {
                playerStats.AddDamageMultiplier(itemData.damageModifier);
            }

            if (itemData.bulletQuantityModifier != 0)
            {
                if (itemData.bulletQuantityModifier > 0)
                    playerStats.AddBulletsPerSecond(itemData.bulletQuantityModifier);
                else
                    playerStats.SubsBulletsPerSecond(Mathf.Abs(itemData.bulletQuantityModifier));
            }

            if (itemData.bulletSpeedModifier != 0)
            {
                if (itemData.bulletSpeedModifier > 0)
                    playerStats.AddBulletSpeed(itemData.bulletSpeedModifier);
                else
                    playerStats.SubsBulletSpeed(Mathf.Abs(itemData.bulletSpeedModifier));
            }

            if (itemData.shrinksCollision)
            {
                playerStats.transform.localScale = Vector3.one * 0.5f;
                Debug.Log("Colisión reducida.");
            }

            if (itemData.grantsShield)
            {
                Debug.Log("¡Escudo activado!");
            }

            // Destruimos el ítem del mapa
            Destroy(gameObject);
        }
        else
        {
            // Si sigue sin encontrarlo, te avisa en consola de qué objeto chocó exactamente
            Debug.LogWarning($"[Aviso] Algo tocó el ítem ({other.gameObject.name}), pero no se encontró el componente PlayerStats en su estructura.");
        }
    }
}