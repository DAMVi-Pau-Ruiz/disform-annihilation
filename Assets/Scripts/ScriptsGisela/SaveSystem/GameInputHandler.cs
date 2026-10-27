using UnityEngine;
using UnityEngine.InputSystem; // <- Importante para usar el New Input System

public class GameInputHandler : MonoBehaviour
{
    private void Update()
    {
        // Verificamos que el teclado esté conectado y activo
        if (Keyboard.current == null) return;

        // Guardar partida al pulsar F5 usando el New Input System
        if (Keyboard.current.f5Key.wasPressedThisFrame)
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.CommitSave();
                Debug.Log("[Input] F5 pulsado (New Input System): Guardando partida...");
            }
            else
            {
                Debug.LogError("[Input] No se encontró el SaveManager en la escena.");
            }
        }

        // Cargar partida al pulsar F9 usando el New Input System
        if (Keyboard.current.f9Key.wasPressedThisFrame)
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.LoadGame();
                Debug.Log("[Input] F9 pulsado (New Input System): Cargando partida...");
            }
            else
            {
                Debug.LogError("[Input] No se encontró el SaveManager en la escena.");
            }
        }
    }
}