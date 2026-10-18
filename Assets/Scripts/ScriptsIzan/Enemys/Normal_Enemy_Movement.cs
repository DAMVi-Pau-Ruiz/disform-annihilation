using UnityEngine;
using UnityEngine.AI;

public class Normal_Enemy_Movement : MonoBehaviour
{
    private Transform playerTransform;
    private NavMeshAgent agent;

    private void Awake()
    {
        
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.Log("no se detecta nada ");
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        
        if (playerTransform != null && agent.isOnNavMesh)
        {
            agent.SetDestination(playerTransform.position);
            
        }
    }
}
