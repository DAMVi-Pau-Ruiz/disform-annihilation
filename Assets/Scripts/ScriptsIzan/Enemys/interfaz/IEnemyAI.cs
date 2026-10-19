using UnityEngine;

public interface IEnemyAI
{
    void ProcesarTriggerEnter(Collider other);
    void ProcesarTriggerExit(Collider other);
}
