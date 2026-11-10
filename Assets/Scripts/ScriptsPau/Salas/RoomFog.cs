
using UnityEngine;

[RequireComponent(typeof(RoomInfo))]
public class RoomFog : MonoBehaviour
{
    [Header("Fog Prefab")]
    [SerializeField] private GameObject fogPrefab;

    [Header("Fog Dimensions")]
    [SerializeField] private float fogThickness = 0.5f;
    [SerializeField] private float heightOffset = 8f;

    private GameObject fogInstance;
    private bool hasBeenVisited = false;

    private RoomInfo roomInfo;

    private void Awake()
    {
        roomInfo = GetComponent<RoomInfo>();
        CreateFog();
    }

    private void CreateFog()
    {
        if (fogPrefab == null)
        {
            Debug.LogWarning("No hay Fog Prefab asignado en " + gameObject.name);
            return;
        }

        Vector2 roomSize = roomInfo.RoomSize;

        // Crear la niebla como hija de la habitación
        fogInstance = Instantiate(fogPrefab, transform);

        fogInstance.name = "FogOfWar";

        // Adaptar posición y escala
        fogInstance.transform.localPosition = new Vector3(
            0f,
            heightOffset,
            0f
        );

        fogInstance.transform.localRotation = Quaternion.identity;

        fogInstance.transform.localScale = new Vector3(
            roomSize.x,
            fogThickness,
            roomSize.y
        );
    }

    public void RevealRoom()
    {
        if (hasBeenVisited)
            return;

        hasBeenVisited = true;

        if (fogInstance != null)
            fogInstance.SetActive(false);
    }
}
