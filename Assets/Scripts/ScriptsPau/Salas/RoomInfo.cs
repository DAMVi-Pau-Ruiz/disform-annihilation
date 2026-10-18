
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    [Header("Room Dimensions")]
    [SerializeField] private Vector2 roomSize = new Vector2(30f, 30f);

    public Vector3 Center => transform.position;
    public Vector2 RoomSize => roomSize;

    public bool Contains(Vector3 position)
    {
        Vector3 localPosition = transform.InverseTransformPoint(position);

        return Mathf.Abs(localPosition.x) <= roomSize.x / 2f &&
               Mathf.Abs(localPosition.z) <= roomSize.y / 2f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireCube(
            Vector3.zero,
            new Vector3(roomSize.x, 0.1f, roomSize.y)
        );
    }
}
