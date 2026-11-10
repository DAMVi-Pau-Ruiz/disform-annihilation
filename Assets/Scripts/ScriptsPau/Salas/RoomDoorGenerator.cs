
using UnityEngine;

[RequireComponent(typeof(RoomInfo))]
[RequireComponent(typeof(RoomDoorSettings))]
public class RoomDoorGenerator : MonoBehaviour
{
    private const float SEGMENT_SIZE = 30f;

    [Header("Prefabs")]
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject energyBarrierPrefab;

    [Header("Position")]
    [SerializeField] private float wallHeight = 0f;

    [Header("Generation")]
    [SerializeField] private bool generateOnStart = true;

    private RoomInfo roomInfo;
    private RoomDoorSettings doorSettings;
    private Transform generatedSegments;

    private void Start()
    {
        if (generateOnStart)
            GenerateRoomBorders();
    }

    [ContextMenu("Generate Room Borders")]
    public void GenerateRoomBorders()
    {
        roomInfo = GetComponent<RoomInfo>();
        doorSettings = GetComponent<RoomDoorSettings>();

        if (wallPrefab == null)
        {
            Debug.LogWarning(
                "No hay Wall Prefab asignado en " + gameObject.name,
                this
            );
            return;
        }

        ClearGeneratedSegments();

        GameObject container = new GameObject("GeneratedBorders");
        container.transform.SetParent(transform, false);

        generatedSegments = container.transform;

        Vector2 roomSize = roomInfo.RoomSize;

        int horizontalSegments =
            Mathf.RoundToInt(roomSize.x / SEGMENT_SIZE);

        int verticalSegments =
            Mathf.RoundToInt(roomSize.y / SEGMENT_SIZE);

        bool allowHorizontalDoors =
            !doorSettings.IsCorridor || roomSize.x <= roomSize.y;

        bool allowVerticalDoors =
            !doorSettings.IsCorridor || roomSize.x >= roomSize.y;

        // NORTE
        GenerateHorizontalSide(
            doorSettings.NorthDoors,
            horizontalSegments,
            roomSize.y / 2f,
            "North",
            allowHorizontalDoors
        );

        // SUR
        GenerateHorizontalSide(
            doorSettings.SouthDoors,
            horizontalSegments,
            -roomSize.y / 2f,
            "South",
            allowHorizontalDoors
        );

        // ESTE
        GenerateVerticalSide(
            doorSettings.EastDoors,
            verticalSegments,
            roomSize.x / 2f,
            "East",
            allowVerticalDoors
        );

        // OESTE
        GenerateVerticalSide(
            doorSettings.WestDoors,
            verticalSegments,
            -roomSize.x / 2f,
            "West",
            allowVerticalDoors
        );

        if (energyBarrierPrefab == null)
        {
            Debug.LogWarning(
                "No has asignado Energy Barrier Prefab en " + gameObject.name,
                this
            );
        }
    }

    private void GenerateHorizontalSide(
        bool[] doors,
        int segmentCount,
        float zPosition,
        string sideName,
        bool allowDoors)
    {
        for (int i = 0; i < segmentCount; i++)
        {
            float xPosition =
                -roomInfo.RoomSize.x / 2f +
                (i + 0.5f) * SEGMENT_SIZE;

            Vector3 localPosition = new Vector3(
                xPosition,
                wallHeight,
                zPosition
            );

            bool hasDoor =
                allowDoors &&
                doors != null &&
                i < doors.Length &&
                doors[i];

            CreateSegment(
                localPosition,
                Quaternion.identity,
                hasDoor,
                sideName + "_" + i
            );
        }
    }

    private void GenerateVerticalSide(
        bool[] doors,
        int segmentCount,
        float xPosition,
        string sideName,
        bool allowDoors)
    {
        for (int i = 0; i < segmentCount; i++)
        {
            float zPosition =
                -roomInfo.RoomSize.y / 2f +
                (i + 0.5f) * SEGMENT_SIZE;

            Vector3 localPosition = new Vector3(
                xPosition,
                wallHeight,
                zPosition
            );

            bool hasDoor =
                allowDoors &&
                doors != null &&
                i < doors.Length &&
                doors[i];

            CreateSegment(
                localPosition,
                Quaternion.Euler(0f, 90f, 0f),
                hasDoor,
                sideName + "_" + i
            );
        }
    }

    private void CreateSegment(
        Vector3 localPosition,
        Quaternion localRotation,
        bool hasDoor,
        string segmentName)
    {
        GameObject prefab = hasDoor ? doorPrefab : wallPrefab;

        if (prefab == null)
            prefab = wallPrefab;

        GameObject segment = Instantiate(
            prefab,
            generatedSegments,
            false
        );

        segment.name = (hasDoor ? "Door_" : "Wall_") + segmentName;

        segment.transform.localPosition = localPosition;
        segment.transform.localRotation = localRotation;

        // Las barreras solamente se generan en las puertas.
        if (hasDoor && energyBarrierPrefab != null)
        {
            GameObject barrier = Instantiate(
                energyBarrierPrefab,
                segment.transform,
                false
            );

            barrier.name = "EnergyBarrier";
            barrier.transform.localPosition = Vector3.zero;
            barrier.transform.localRotation = Quaternion.identity;

            // Empieza desactivada hasta que comience el combate.
            barrier.SetActive(false);
        }
    }

    public void SetBarriersActive(bool active)
    {
        if (generatedSegments == null)
        {
            Transform existing = transform.Find("GeneratedBorders");

            if (existing != null)
                generatedSegments = existing;
        }

        if (generatedSegments == null)
            return;

        foreach (Transform segment in generatedSegments)
        {
            Transform barrier = segment.Find("EnergyBarrier");

            if (barrier != null)
                barrier.gameObject.SetActive(active);
        }
    }

    private void ClearGeneratedSegments()
    {
        Transform previousContainer =
            transform.Find("GeneratedBorders");

        if (previousContainer == null)
            return;

        if (Application.isPlaying)
            Destroy(previousContainer.gameObject);
        else
            DestroyImmediate(previousContainer.gameObject);
    }
}

