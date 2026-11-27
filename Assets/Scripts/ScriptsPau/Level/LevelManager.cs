using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level Exit")]
    [SerializeField] private GameObject exitPrefab;
    [SerializeField] private Transform exitPosition;

    private int totalBosses;
    private int defeatedBosses;

    private void Start()
    {
        
    }
}
