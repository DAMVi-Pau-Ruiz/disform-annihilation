
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

        Vector2 size = currentRoom.RoomSize;

        // Si la dimensión más pequeña es pequeña,
        // usamos el zoom de sala pequeña.
        float smallestDimension = Mathf.Min(size.x, size.y);

        float targetZoom = smallestDimension <= smallRoomSize
            ? smallRoomZoom
            : largeRoomZoom;

        // Evitar que la cámara muestre el exterior de la sala.
        // En perspectiva ortográfica, el ancho visible depende
        // del aspect ratio de la cámara.
        float maxZoomByWidth = (size.x / 2f) / cam.aspect;
        float maxZoomByDepth = size.y / 2f;

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

        // Posición objetivo.
        Vector3 targetPosition = new Vector3(
            player.position.x,
            player.position.y + cameraHeight,
            player.position.z
        );

        // Límites horizontales según el ancho X.
        float halfWidthRoom = size.x / 2f;
        float halfHeightRoom = size.y / 2f;

        float halfHeightCamera = cam.orthographicSize;
        float halfWidthCamera = halfHeightCamera * cam.aspect;

        float minX = currentRoom.Center.x - halfWidthRoom + halfWidthCamera;
        float maxX = currentRoom.Center.x + halfWidthRoom - halfWidthCamera;

        float minZ = currentRoom.Center.z - halfHeightRoom + halfHeightCamera;
        float maxZ = currentRoom.Center.z + halfHeightRoom - halfHeightCamera;

        targetPosition.x = minX > maxX
            ? currentRoom.Center.x
            : Mathf.Clamp(targetPosition.x, minX, maxX);

        targetPosition.z = minZ > maxZ
            ? currentRoom.Center.z
            : Mathf.Clamp(targetPosition.z, minZ, maxZ);

        // Seguimiento directo para evitar el temblor.
        transform.position = targetPosition;

        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
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
                // Solo actuamos cuando entramos en una sala diferente.
                if (room != currentRoom)
                {
                    currentRoom = room;

                    // Buscar los spawners que pertenecen a esta sala.
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
