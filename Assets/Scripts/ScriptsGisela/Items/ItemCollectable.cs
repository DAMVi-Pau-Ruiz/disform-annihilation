using UnityEngine;

public class ItemCollectable : MonoBehaviour
{
    [Header("Identificación del Ítem")]
    public int itemId; // 1: curar, 2: daño, 3: muerte, 4: revivir

    private bool yaRecogido = false; // Bandera para evitar doble colisión

    private void OnTriggerEnter(Collider other)
    {
        // Si ya fue recogido, ignoramos cualquier otro choque instantáneo
        if (yaRecogido) return;

        // Comprobamos si choca con la nave 
        if (other.CompareTag("Player"))
        {
            yaRecogido = true; // Marcamos como recogido de inmediato
            Debug.Log($"¡La nave ha recogido el ítem con ID: {itemId}!");

            //  componente PlayerCombat en la nave
            PlayerCombat playerCombat = other.GetComponent<PlayerCombat>();

            if (playerCombat != null)
            {
                switch (itemId)
                {
                    case 1: // Curar
                        playerCombat.AddHealth(2);
                        Debug.Log("Se ha aplicado curación de 2 puntos.");
                        break;

                    case 2: // Daño
                        playerCombat.GetHurt(2);
                        Debug.Log("Se ha aplicado daño de 2 puntos.");
                        break;

                    case 3: // Muerte instantánea
                        playerCombat.GetHurt(playerCombat.GetCurrentHealth());
                        Debug.Log("Muerte instantánea aplicada.");
                        break;

                    case 4: // Revivir / Curar al máximo
                        int vidaFaltante = playerCombat.GetTotalHealth() - playerCombat.GetCurrentHealth();
                        playerCombat.AddHealth(vidaFaltante);
                        Debug.Log("Curado al máximo.");
                        break;
                }
            }

            // Destruimos el ítem del mapa al ser recogido
            Destroy(gameObject);
        }
    }
}