
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Camera")]
    [SerializeField] private float cameraAngle = 55f;
    [SerializeField] private float cameraDistance = 25f;
    [SerializeField] private float fieldOfView = 45f;

    [Header("Follow")]
    [SerializeField] private float followSmoothTime = 0.15f;

    [Header("Dead Zone")]
    [Range(0f, 0.45f)]
    [SerializeField] private float deadZoneHorizontal = 0.2f;

    [Range(0f, 0.45f)]
    [SerializeField] private float deadZoneVertical = 0.2f;

    [Header("Player Visibility")]
    [SerializeField] private float playerScreenMargin = 0.08f;
    [SerializeField] private Renderer playerRenderer;

    private Camera cam;
    private RoomInfo currentRoom;

    private Quaternion cameraRotation;
    private Vector3 focusPosition;
    private Vector3 focusVelocity;

    private bool initialized;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        cam.orthographic = false;
        cam.fieldOfView = fieldOfView;

        cameraRotation = Quaternion.Euler(cameraAngle, 0f, 0f);
        transform.rotation = cameraRotation;
    }

    private void LateUpdate()
    {
        if (player == null)
            return;

        DetectRoom();

        if (!initialized)
        {
            focusPosition = player.position;
            focusVelocity = Vector3.zero;
            initialized = true;
        }

        transform.rotation = cameraRotation;

        UpdateCameraPosition();

        Vector3 viewportPosition =
            cam.WorldToViewportPoint(player.position);

        Vector3 desiredFocus = focusPosition;

        float leftLimit = 0.5f - deadZoneHorizontal;
        float rightLimit = 0.5f + deadZoneHorizontal;

        float bottomLimit = 0.5f - deadZoneVertical;
        float topLimit = 0.5f + deadZoneVertical;

        if (viewportPosition.z > 0f)
        {
            float halfHeight =
                viewportPosition.z *
                Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

            float halfWidth = halfHeight * cam.aspect;

            // Seguimiento horizontal.
            if (viewportPosition.x < leftLimit)
            {
                desiredFocus.x +=
                    (viewportPosition.x - leftLimit) * 2f * halfWidth;
            }
            else if (viewportPosition.x > rightLimit)
            {
                desiredFocus.x +=
                    (viewportPosition.x - rightLimit) * 2f * halfWidth;
            }

            // Seguimiento vertical.
            float pitchSin =
                Mathf.Sin(cameraAngle * Mathf.Deg2Rad);

            if (viewportPosition.y < bottomLimit)
            {
                desiredFocus.z +=
                    (viewportPosition.y - bottomLimit) *
                    2f * halfHeight / pitchSin;
            }
            else if (viewportPosition.y > topLimit)
            {
                desiredFocus.z +=
                    (viewportPosition.y - topLimit) *
                    2f * halfHeight / pitchSin;
            }
        }

        focusPosition = Vector3.SmoothDamp(
            focusPosition,
            desiredFocus,
            ref focusVelocity,
            followSmoothTime
        );

        UpdateCameraPosition();

        KeepPlayerVisible();
    }

    private void UpdateCameraPosition()
    {
        Vector3 offset =
            cameraRotation * new Vector3(0f, 0f, -cameraDistance);

        transform.position = focusPosition + offset;
        transform.rotation = cameraRotation;
    }

    private void KeepPlayerVisible()
    {
        Bounds bounds = GetPlayerBounds();

        Vector3[] corners = new Vector3[8];

        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        int index = 0;

        for (int x = 0; x <= 1; x++)
        {
            for (int y = 0; y <= 1; y++)
            {
                for (int z = 0; z <= 1; z++)
                {
                    corners[index++] = new Vector3(
                        x == 0 ? min.x : max.x,
                        y == 0 ? min.y : max.y,
                        z == 0 ? min.z : max.z
                    );
                }
            }
        }

        Vector3 viewportMin = Vector3.one;
        Vector3 viewportMax = Vector3.zero;

        foreach (Vector3 corner in corners)
        {
            Vector3 point = cam.WorldToViewportPoint(corner);

            if (point.z <= cam.nearClipPlane)
                continue;

            viewportMin = Vector3.Min(viewportMin, point);
            viewportMax = Vector3.Max(viewportMax, point);
        }

        float margin = playerScreenMargin;

        float correctionX = 0f;
        float correctionY = 0f;

        if (viewportMin.x < margin)
            correctionX = viewportMin.x - margin;
        else if (viewportMax.x > 1f - margin)
            correctionX = viewportMax.x - (1f - margin);

        if (viewportMin.y < margin)
            correctionY = viewportMin.y - margin;
        else if (viewportMax.y > 1f - margin)
            correctionY = viewportMax.y - (1f - margin);

        if (Mathf.Abs(correctionX) < 0.0001f &&
            Mathf.Abs(correctionY) < 0.0001f)
            return;

        float depth = Vector3.Distance(transform.position, bounds.center);

        float halfHeight =
            depth * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

        float halfWidth = halfHeight * cam.aspect;

        float pitchSin =
            Mathf.Sin(cameraAngle * Mathf.Deg2Rad);

        focusPosition.x += correctionX * 2f * halfWidth;
        focusPosition.z += correctionY * 2f * halfHeight / pitchSin;

        focusVelocity = Vector3.zero;

        UpdateCameraPosition();
    }

    private Bounds GetPlayerBounds()
    {
        if (playerRenderer != null)
            return playerRenderer.bounds;

        Collider playerCollider =
            player.GetComponentInChildren<Collider>();

        if (playerCollider != null)
            return playerCollider.bounds;

        return new Bounds(
            player.position,
            new Vector3(1f, 2f, 1f)
        );
    }

    private void DetectRoom()
    {
        RoomInfo[] rooms = FindObjectsByType<RoomInfo>(
            FindObjectsSortMode.None
        );

        foreach (RoomInfo room in rooms)
        {
            if (room == null || !room.Contains(player.position))
                continue;

            if (room != currentRoom)
            {
                currentRoom = room;
                RoomFog roomFog = currentRoom.GetComponent<RoomFog>();

                if (roomFog != null)
                {
                    roomFog.RevealRoom();
                }

                Debug.Log("Jugador ha entrado en: " + currentRoom.gameObject.name);

                // Iniciar el combate de esta habitación.
                RoomCombatController combatController =
                    currentRoom.GetComponent<RoomCombatController>();

                if (combatController != null)
                {
                    combatController.StartCombat();
                }
                else
                {
                    // Compatibilidad con habitaciones que todavía
                    // no tengan RoomCombatController.
                    EnemySpawner[] spawners =
                        currentRoom.GetComponentsInChildren<EnemySpawner>(true);

                    foreach (EnemySpawner spawner in spawners)
                    {
                        spawner.SpawnEnemies();
                    }
                }

                // Mantener el sistema de generación de objetos.
                if (currentRoom.gameObject.name == "ItemRoom")
                {
                    ItemSpawner[] itemSpawners =
                        currentRoom.GetComponentsInChildren<ItemSpawner>(true);

                    foreach (ItemSpawner itemSpawner in itemSpawners)
                    {
                        itemSpawner.GenerarItemsEnSpawnPoints();
                    }
                }
            }

            return;
        }
    }
}
