using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;

    private void Start()
    {
        CreatePlayer();
    }

    public void CreatePlayer()
    {
        if (spawnPoint == null || playerPrefab == null)
        {
            return;
        }
        GameObject player = Instantiate(playerPrefab, spawnPoint);
        player.transform.SetParent(null);
    }
}
