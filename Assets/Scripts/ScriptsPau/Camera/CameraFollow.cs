
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float cameraHeight = 30f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 3f;
    [SerializeField] private float smallRoomZoom = 10f;
    [SerializeField] private float largeRoomZoom = 20f;

    [Header("Room Size")]
    [SerializeField] private float smallRoomSize = 30f;

    private Camera cam;
    private RoomInfo currentRoom;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
    }

    private void LateUpdate()
    {
        if (player == null)
            return;

        DetectRoom();

        if (currentRoom == null)
            return;

        // Obtener las dimensiones reales de la sala
        // teniendo en cuenta su rotación y escala.
        Vector2 halfExtents = GetWorldHalfExtents();

        float roomWidth = halfExtents.x * 2f;
        float roomDepth = halfExtents.y * 2f;

        // Elegir zoom según la dimensión más pequeña.
        float smallestDimension = Mathf.Min(roomWidth, roomDepth);

        float targetZoom = smallestDimension <= smallRoomSize
            ? smallRoomZoom
            : largeRoomZoom;

        // Evitar que la cámara muestre el exterior.
        float maxZoomByWidth = halfExtents.x / cam.aspect;
        float maxZoomByDepth = halfExtents.y;

        float maxAllowedZoom = Mathf.Min(
            maxZoomByWidth,
            maxZoomByDepth
        );

        targetZoom = Mathf.Min(targetZoom, maxAllowedZoom);

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetZoom,
            zoomSpeed * Time.deltaTime
        );

        // Posición objetivo siguiendo al jugador.
        Vector3 targetPosition = new Vector3(
            player.position.x,
            player.position.y + cameraHeight,
            player.position.z
        );

        // Calcular los márgenes visibles de la cámara.
        float halfHeightCamera = cam.orthographicSize;
        float halfWidthCamera = halfHeightCamera * cam.aspect;

        float minX = currentRoom.Center.x
                     - halfExtents.x + halfWidthCamera;

        float maxX = currentRoom.Center.x
                     + halfExtents.x - halfWidthCamera;

        float minZ = currentRoom.Center.z
                     - halfExtents.y + halfHeightCamera;

        float maxZ = currentRoom.Center.z
                     + halfExtents.y - halfHeightCamera;

        // Limitar el movimiento en X.
        targetPosition.x = minX > maxX
            ? currentRoom.Center.x
            : Mathf.Clamp(targetPosition.x, minX, maxX);

        // Limitar el movimiento en Z.
        targetPosition.z = minZ > maxZ
            ? currentRoom.Center.z
            : Mathf.Clamp(targetPosition.z, minZ, maxZ);

        // Seguimiento directo para evitar temblores.
        transform.position = targetPosition;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private Vector2 GetWorldHalfExtents()
    {
        Vector2 size = currentRoom.RoomSize;
        Transform roomTransform = currentRoom.transform;

        // Semiejes locales convertidos al espacio del mundo.
        Vector3 halfAxisX = roomTransform.TransformVector(
            new Vector3(size.x / 2f, 0f, 0f)
        );

        Vector3 halfAxisZ = roomTransform.TransformVector(
            new Vector3(0f, 0f, size.y / 2f)
        );

        // Calcular cuánto ocupa la sala en los ejes
        // globales X y Z, incluyendo su rotación.
        float halfWidth =
            Mathf.Abs(halfAxisX.x) + Mathf.Abs(halfAxisZ.x);

        float halfDepth =
            Mathf.Abs(halfAxisX.z) + Mathf.Abs(halfAxisZ.z);

        return new Vector2(halfWidth, halfDepth);
    }

    private void DetectRoom()
    {
        RoomInfo[] rooms = FindObjectsByType<RoomInfo>(
            FindObjectsSortMode.None
        );

        foreach (RoomInfo room in rooms)
        {
            if (room != null && room.Contains(player.position))
            {
                // Solo actuar al entrar en una sala diferente.
                if (room != currentRoom)
                {
                    currentRoom = room;

                    // Activar los spawners de esta sala.
                    EnemySpawner[] spawners =
                        currentRoom.GetComponentsInChildren<EnemySpawner>(true);

                    foreach (EnemySpawner spawner in spawners)
                    {
                        spawner.SpawnEnemies();
                    }
                }

                return;
            }
        }
    }
}
