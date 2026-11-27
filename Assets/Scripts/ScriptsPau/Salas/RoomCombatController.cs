using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemySpawner))]
[RequireComponent(typeof(RoomDoorGenerator))]
public class RoomCombatController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private RoomDoorGenerator doorGenerator;

    [Header("Door Settings")]
    [SerializeField] private float doorCloseDelay = 0.1f;

    private bool combatStarted;
    private bool roomCompleted;

    private void Awake()
    {
        if (enemySpawner == null)
            enemySpawner = GetComponent<EnemySpawner>();

        if (doorGenerator == null)
            doorGenerator = GetComponent<RoomDoorGenerator>();
    }

    private void OnEnable()
    {
        if (enemySpawner != null)
            enemySpawner.OnAllEnemiesDefeated += HandleAllEnemiesDefeated;
    }

    private void OnDisable()
    {
        if (enemySpawner != null)
            enemySpawner.OnAllEnemiesDefeated -= HandleAllEnemiesDefeated;
    }

    // Lo llama CameraFollow cuando el jugador entra en esta habitación.
    public void StartCombat()
    {
        if (enemySpawner == null || doorGenerator == null)
        {
            Debug.LogError(
                "Faltan referencias en RoomCombatController.",
                this
            );
            return;
        }

        // Si la sala ya se completó, las barreras permanecen apagadas.
        if (roomCompleted || enemySpawner.AreAllEnemiesDefeated)
        {
            roomCompleted = true;
            doorGenerator.SetBarriersActive(false);
            return;
        }

        if (combatStarted)
            return;

        combatStarted = true;

        Debug.Log("¡Comienza el combate en " + gameObject.name + "!");

        // Cerramos las puertas después del retraso.
        StartCoroutine(CloseDoorsWithDelay());

        // Generamos los enemigos.
        enemySpawner.SpawnEnemies();

        // Por si la sala no tiene enemigos que generar.
        if (enemySpawner.AreAllEnemiesDefeated)
            HandleAllEnemiesDefeated();
    }

    private IEnumerator CloseDoorsWithDelay()
    {
        yield return new WaitForSeconds(doorCloseDelay);

        // Cerramos todas las puertas.
        if (!roomCompleted)
            doorGenerator.SetBarriersActive(true);
    }

    private void HandleAllEnemiesDefeated()
    {
        if (roomCompleted)
            return;

        roomCompleted = true;
        combatStarted = false;

        Debug.Log("¡Sala completada! " + gameObject.name);

        // Abrimos todas las puertas inmediatamente.
        doorGenerator.SetBarriersActive(false);
    }
}