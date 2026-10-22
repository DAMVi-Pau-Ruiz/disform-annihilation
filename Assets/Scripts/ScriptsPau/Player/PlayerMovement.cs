
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference slow;

    [Header("Rotation")]
    [SerializeField] private Camera playerCamera;

    private Rigidbody rb;
    private PlayerStats stats;
    private Vector2 moveDirection;
    private Quaternion targetRotation;
    private bool hasTargetRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        OnMove();
        OnRotate();
    }

    private void FixedUpdate()
    {
        if (stats == null)
        {
            return;
        }

        float moveSpeed = stats.GetBaseSpeed();

        if (slow != null && slow.action != null &&
            slow.action.IsPressed())
        {
            moveSpeed = stats.GetSlowSpeed();
        }

        Vector3 movement = new Vector3(
            moveDirection.x, 0f, moveDirection.y
        );

        Vector3 deltaMovement =
            movement * moveSpeed * Time.fixedDeltaTime;

        if (rb != null)
        {
            if (moveDirection == Vector2.zero)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            rb.MovePosition(rb.position + deltaMovement);

            if (hasTargetRotation)
            {
                rb.MoveRotation(targetRotation);
            }
        }
        else
        {
            transform.position += deltaMovement;

            if (hasTargetRotation)
            {
                transform.rotation = targetRotation;
            }
        }
    }

    private void OnMove()
    {
        moveDirection = Vector2.zero;

        if (move != null && move.action != null &&
            move.action.enabled)
        {
            moveDirection = move.action.ReadValue<Vector2>();
        }
        else if (Keyboard.current != null)
        {
            float x = 0f;
            float y = 0f;

            if (Keyboard.current.wKey.isPressed) y += 1f;
            if (Keyboard.current.sKey.isPressed) y -= 1f;
            if (Keyboard.current.dKey.isPressed) x += 1f;
            if (Keyboard.current.aKey.isPressed) x -= 1f;

            moveDirection = new Vector2(x, y).normalized;
        }
        
        if (moveDirection.sqrMagnitude < 0.01f)
        {
            moveDirection = Vector2.zero;
        }
    }

    private void OnRotate()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (Mouse.current == null || playerCamera == null)
        {
            return;
        }

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Ray ray = playerCamera.ScreenPointToRay(mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 targetPosition = ray.GetPoint(distance);
            Vector3 direction = targetPosition - transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                float angle = Mathf.Atan2(
                    direction.x, direction.z
                ) * Mathf.Rad2Deg;

                targetRotation = Quaternion.Euler(90f, angle, 0f);
                hasTargetRotation = true;
            }
        }
    }
}
