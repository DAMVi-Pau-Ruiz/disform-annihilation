
using UnityEngine;

public class ProjectileArrow : ProjectileBase
{
    protected override void Awake()
    {
        base.Awake();
    }

    private void FixedUpdate()
    {
        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(
                rb.linearVelocity.normalized
            );
        }
    }
}
