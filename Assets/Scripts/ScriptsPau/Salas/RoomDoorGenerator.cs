
using UnityEngine;

[RequireComponent(typeof(RoomInfo))]
[RequireComponent(typeof(RoomDoorSettings))]
public class RoomDoorGenerator : MonoBehaviour
{
    private const float SEGMENT_SIZE = 30f;

    [Header("Door Prefab")]
    [SerializeField] private GameObject doorPrefab;

    [Header("Door Position")]
    [SerializeField] private float doorHeight = 0f;

    [Header("Generation")]
    [SerializeField] private bool generateOnStart = true;

    private RoomInfo roomInfo;
    private RoomDoorSettings doorSettings;

    private Transform generatedDoors;

    private void Awake()
    {
        roomInfo = GetComponent<RoomInfo>();
        doorSettings = GetComponent<RoomDoorSettings>();
    }

    private void Start()
    {
        if (generateOnStart)
            GenerateDoors();
    }

    [ContextMenu("Generate Doors")]
    public void GenerateDoors()
    {
        if (roomInfo == null)
            roomInfo = GetComponent<RoomInfo>();

        if (doorSettings == null)
            doorSettings = GetComponent<RoomDoorSettings>();

        if (doorPrefab == null)
        {
            Debug.LogWarning(
                "No hay Door Prefab asignado en " + gameObject.name,
                this
            );
            return;
        }

        ClearGeneratedDoors();

        GameObject container = new GameObject("GeneratedDoors");
        container.transform.SetParent(transform, false);

        generatedDoors = container.transform;

        Vector2 roomSize = roomInfo.RoomSize;

        int horizontalSegments =
            Mathf.RoundToInt(roomSize.x / SEGMENT_SIZE);

        int verticalSegments =
            Mathf.RoundToInt(roomSize.y / SEGMENT_SIZE);

        // Norte y sur: puertas colocadas a lo largo del eje X.
        // En pasillos solo se permiten si son los extremos.
        if (!doorSettings.IsCorridor || roomSize.x <= roomSize.y)
        {
            GenerateHorizontalSide(
                doorSettings.NorthDoors,
                horizontalSegments,
                roomSize.y / 2f,
                "North"
            );

            GenerateHorizontalSide(
                doorSettings.SouthDoors,
                horizontalSegments,
                -roomSize.y / 2f,
                "South"
            );
        }

        // Este y oeste: puertas colocadas a lo largo del eje Z.
        // En pasillos solo se permiten si son los extremos.
        if (!doorSettings.IsCorridor || roomSize.x >= roomSize.y)
        {
            GenerateVerticalSide(
                doorSettings.EastDoors,
                verticalSegments,
                roomSize.x / 2f,
                "East"
            );

            GenerateVerticalSide(
                doorSettings.WestDoors,
                verticalSegments,
                -roomSize.x / 2f,
                "West"
            );
        }
    }

    private void GenerateHorizontalSide(
        bool[] doors,
        int segmentCount,
        float zPosition,
        string sideName)
    {
        if (doors == null)
            return;

        for (int i = 0; i < segmentCount; i++)
        {
            if (i >= doors.Length || !doors[i])
                continue;

            float xPosition =
                -roomInfo.RoomSize.x / 2f +
                (i + 0.5f) * SEGMENT_SIZE;

            Vector3 localPosition = new Vector3(
                xPosition,
                doorHeight,
                zPosition
            );

            CreateDoor(
                localPosition,
                Quaternion.identity,
                sideName + "_" + i
            );
        }
    }

    private void GenerateVerticalSide(
        bool[] doors,
        int segmentCount,
        float xPosition,
        string sideName)
    {
        if (doors == null)
            return;

        for (int i = 0; i < segmentCount; i++)
        {
            if (i >= doors.Length || !doors[i])
                continue;

            float zPosition =
                -roomInfo.RoomSize.y / 2f +
                (i + 0.5f) * SEGMENT_SIZE;

            Vector3 localPosition = new Vector3(
                xPosition,
                doorHeight,
                zPosition
            );

            CreateDoor(
                localPosition,
                Quaternion.Euler(0f, 90f, 0f),
                sideName + "_" + i
            );
        }
    }

    private void CreateDoor(
        Vector3 localPosition,
        Quaternion localRotation,
        string doorName)
    {
        GameObject door = Instantiate(
            doorPrefab,
            generatedDoors,
            false
        );

        door.name = doorName;
        door.transform.localPosition = localPosition;
        door.transform.localRotation = localRotation;
    }

    private void ClearGeneratedDoors()
    {
        Transform previousContainer = transform.Find("GeneratedDoors");

        if (previousContainer == null)
            return;

        if (Application.isPlaying)
            Destroy(previousContainer.gameObject);
        else
            DestroyImmediate(previousContainer.gameObject);
    }
}
