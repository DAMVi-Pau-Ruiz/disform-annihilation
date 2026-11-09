
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RoomInfo))]
public class RoomDoorSettings : MonoBehaviour
{
    private const float SEGMENT_SIZE = 30f;

    [Header("Room Type")]
    [SerializeField] private bool isCorridor;

    [Header("Door Configuration")]
    [SerializeField] private bool[] northDoors = new bool[1];
    [SerializeField] private bool[] southDoors = new bool[1];
    [SerializeField] private bool[] eastDoors = new bool[1];
    [SerializeField] private bool[] westDoors = new bool[1];

    private RoomInfo roomInfo;

    public bool IsCorridor => isCorridor;

    public bool[] NorthDoors => northDoors;
    public bool[] SouthDoors => southDoors;
    public bool[] EastDoors => eastDoors;
    public bool[] WestDoors => westDoors;

    private void OnEnable()
    {
        UpdateArrays();
    }

    private void OnValidate()
    {
        UpdateArrays();
    }

    private void UpdateArrays()
    {
        if (roomInfo == null)
            roomInfo = GetComponent<RoomInfo>();

        if (roomInfo == null)
            return;

        Vector2 size = roomInfo.RoomSize;

        int horizontalSegments =
            Mathf.Max(1, Mathf.RoundToInt(size.x / SEGMENT_SIZE));

        int verticalSegments =
            Mathf.Max(1, Mathf.RoundToInt(size.y / SEGMENT_SIZE));

        northDoors = ResizeArray(northDoors, horizontalSegments);
        southDoors = ResizeArray(southDoors, horizontalSegments);

        eastDoors = ResizeArray(eastDoors, verticalSegments);
        westDoors = ResizeArray(westDoors, verticalSegments);
    }

    private bool[] ResizeArray(bool[] original, int newSize)
    {
        if (original == null)
            original = new bool[0];

        if (original.Length == newSize)
            return original;

        bool[] resized = new bool[newSize];

        for (int i = 0; i < Mathf.Min(original.Length, newSize); i++)
        {
            resized[i] = original[i];
        }

        return resized;
    }
}
