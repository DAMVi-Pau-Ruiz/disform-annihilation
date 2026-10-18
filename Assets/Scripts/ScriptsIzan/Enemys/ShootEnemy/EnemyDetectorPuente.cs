using UnityEngine;

public class EnemyDetectorPuente : MonoBehaviour
{
    private Shoot_Enemy_Movement scriptPrincipal;

    void Start()
    {
        // Buscamos el script de movimiento en nuestro objeto padre
        scriptPrincipal = GetComponentInParent<Shoot_Enemy_Movement>();

        if (scriptPrincipal == null)
        {
            Debug.LogError($"¡No se encontró el script Shoot_Enemy_Movement en el padre de {gameObject.name}!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (scriptPrincipal != null)
        {
            // Le reenviamos la colisión al script del padre
            scriptPrincipal.ProcesarTriggerEnter(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (scriptPrincipal != null)
        {
            // Le reenviamos la salida al script del padre
            scriptPrincipal.ProcesarTriggerExit(other);
        }
    }
}
