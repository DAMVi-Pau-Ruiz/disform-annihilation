
using UnityEngine;

public class ProjectileArrow : ProjectileBase
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rb == null || rb.linearVelocity.sqrMagnitude < 0.01f)
            return;

        transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
    }
}
