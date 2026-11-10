
using UnityEngine;

public class ConfiguracionColisiones : MonoBehaviour
{
    private void Awake()
    {
        int capaJugador = LayerMask.NameToLayer("PlayerBullet");
        int capaBoss = LayerMask.NameToLayer("BossBullet");

        if (capaJugador == -1 || capaBoss == -1)
        {
            Debug.LogError("Faltan las Layers PlayerBullet o BossBullet.");
            return;
        }

        Physics.IgnoreLayerCollision(
            capaJugador,
            capaBoss,
            true
        );
    }
}
