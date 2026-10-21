using UnityEngine;

public class ItemCollectable : MonoBehaviour
{
    [Header("Identificación del Ítem")]
    public int itemId; // 1: curar, 2: daño, 3: muerte, 4: revivir

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Algo ha tocado el ítem (3D): " + other.gameObject.name + " | Tag: " + other.tag);

        // Comprobamos si choca con la nave según si es tag Player
        if (other.CompareTag("Player"))
        {
            Debug.Log($"¡La nave ha recogido el ítem con ID: {itemId}! Destruyendo objeto...");

            switch (itemId)
            {
                case 1:
                    // curar
                    break;
                case 2:
                    // daño
                    break;
                case 3:
                    // muerte
                    break;
                case 4:
                    // revivir
                    break;
            }

            // Destruimos el objeto del mapa al ser recogido
            Destroy(gameObject);
        }
      
    }
}