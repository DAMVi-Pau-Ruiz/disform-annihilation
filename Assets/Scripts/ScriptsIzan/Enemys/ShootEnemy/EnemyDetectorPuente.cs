using UnityEngine;

public class EnemyDetectorPuente : MonoBehaviour
{
    // Ahora guardamos la interfaz genérica en lugar de un script concreto
    private IEnemyAI enemigoPrincipal;

    void Start()
    {
        // Buscamos cualquier script en el padre que implemente IEnemyAI
        enemigoPrincipal = GetComponentInParent<IEnemyAI>();

        if (enemigoPrincipal == null)
        {
            Debug.LogError($"¡No se encontró ningún script de enemigo (IEnemyAI) en el padre de {gameObject.name}!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemigoPrincipal != null)
        {
            // Le reenviamos la colisión de forma genérica
            enemigoPrincipal.ProcesarTriggerEnter(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (enemigoPrincipal != null)
        {
            // Le reenviamos la salida de forma genérica
            enemigoPrincipal.ProcesarTriggerExit(other);
        }
    }
}
