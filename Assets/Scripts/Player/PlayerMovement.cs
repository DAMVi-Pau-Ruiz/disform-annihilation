using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference slow;
    [SerializeField] private InputActionReference rotate;

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 10f;
    [SerializeField] private float slowSpeed = 5f;

    [Header("Rotation")]
    [SerializeField] private Camera playerCamera;

    private Rigidbody rb;
    private Vector2 moveDirection;
    private Quaternion targetRotation;
    private bool hasTargetRotation;
    private float moveSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moveSpeed = baseSpeed;
    }

    private void Update()
    {
        OnMove();
        OnRotate();
        OnSlow();
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(moveDirection.x, 0f, moveDirection.y);
        Vector3 deltaMovement = movement * moveSpeed * Time.fixedDeltaTime;

        if (rb != null)
        {
            rb.MovePosition(rb.position + deltaMovement);

            if (hasTargetRotation)
                rb.MoveRotation(targetRotation);
        }
        else
        {
            transform.position += deltaMovement;

            if (hasTargetRotation)
                transform.rotation = targetRotation;
        }
    }

    private void OnMove()
    {
        moveDirection = Vector2.zero;

        if (move != null && move.action != null && move.action.enabled)
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
    }

    private void OnRotate()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (Mouse.current == null || playerCamera == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray ray = playerCamera.ScreenPointToRay(mousePosition);

        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 targetPosition = ray.GetPoint(distance);
            Vector3 direction = targetPosition - transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                targetRotation = Quaternion.Euler(90f, angle, 0f);
                hasTargetRotation = true;
            }
        }
    }

    private void OnSlow()
    {
        if (slow == null || slow.action == null)
        {
            return;
        }

        if (slow.action.IsPressed())
        {
            moveSpeed = slowSpeed;
        }
        else
        {
            moveSpeed = baseSpeed;
        }
    }
}
