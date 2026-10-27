using UnityEngine;

public class PlayerSaveable : SaveableEntity
{
    private PlayerStats playerStats;

    // CAMBIADO A PROTECTED OVERRIDE
    protected override void Awake()
    {
        base.Awake(); // Llama al Awake de SaveableEntity para generar el GUID si no lo tiene
        playerStats = GetComponent<PlayerStats>();
    }

    public override SaveData CaptureState()
    {
        PlayerSaveData data = new PlayerSaveData();
        data.uniqueId = UniqueId;
        data.sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Posición y rotación de la nave
        data.position = new float[] { transform.position.x, transform.position.y, transform.position.z };
        data.rotation = new float[] { transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w };

        // Salud actual
        if (playerStats != null)
        {
            data.currentHealth = playerStats.GetCurrentHealth();
        }

        return data;
    }

    public override void RestoreState(SaveData savedData)
    {
        PlayerSaveData data = savedData as PlayerSaveData;
        if (data == null) return;

        // Restaurar posición y rotación
        transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
        transform.rotation = new Quaternion(data.rotation[0], data.rotation[1], data.rotation[2], data.rotation[3]);

        // Restaurar vida de forma segura usando tus métodos existentes
        if (playerStats != null)
        {
            float healthDiff = data.currentHealth - playerStats.GetCurrentHealth();
            if (healthDiff > 0) playerStats.AddHealth(healthDiff);
            else if (healthDiff < 0) playerStats.GetHurt(Mathf.Abs(healthDiff));
        }
    }
}