
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] private string newLevelName;

    private bool playerEntered;

    private void OnTriggerEnter(Collider other)
    {
        if (playerEntered)
            return;

        if (other.CompareTag("Player"))
        {
            playerEntered = true;

            // Cargar el siguiente nivel.
            SceneManager.LoadScene(newLevelName);
        }
    }
}
