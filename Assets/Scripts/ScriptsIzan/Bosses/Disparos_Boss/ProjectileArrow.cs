
using UnityEngine;

public class ProjectileArrow : ProjectileBase
{
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
