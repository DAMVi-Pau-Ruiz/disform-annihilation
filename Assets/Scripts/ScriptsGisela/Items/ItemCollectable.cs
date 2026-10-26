using UnityEngine;

public class ItemCollectable : MonoBehaviour
{
    [Header("Datos del Ítem (ScriptableObject)")]
    public ItemData itemData; // Arrastra aquí el ItemData correspondiente

    private bool yaRecogido = false;

    private void OnTriggerEnter(Collider other)
    {
        if (yaRecogido) return;

        if (other.CompareTag("Player"))
        {
            yaRecogido = true;
            Debug.Log($"¡Ítem recogido!");

            // Obtenemos los componentes del jugador que ya tienes creados
            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            if (playerStats != null && itemData != null)
            {
                // 1. Aplicamos Salud / Curación usando el método AddHealth o GetHurt que YA existen en tu PlayerStats
                if (itemData.healthModifier > 0)
                {
                    playerStats.AddHealth(itemData.healthModifier);
                    Debug.Log($"Curación aplicada: {itemData.healthModifier}");
                }
                else if (itemData.healthModifier < 0)
                {
                    playerStats.GetHurt(Mathf.Abs(itemData.healthModifier));
                    Debug.Log($"Daño aplicado: {itemData.healthModifier}");
                }

                // 2. Si el ítem es una reducción de tamaño / colisión
                if (itemData.shrinksCollision)
                {
                    other.transform.localScale = Vector3.one * 0.5f;
                    Debug.Log("Colisión reducida.");
                }

                // 3. Aquí puedes añadir lógicas visuales o de otros sistemas si las necesitas
                if (itemData.grantsShield)
                {
                    Debug.Log("¡Escudo activado!");
                }
            }

            // Destruimos el ítem del mapa al ser recogido
            Destroy(gameObject);
        }
    }
}