using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;

    [SerializeField] private float linecastOffsetY;
    [SerializeField] private float linecastLenght;

    private bool onGround;

    public bool OnGround()
    {
        onGround = Physics2D.BoxCast(transform.position, new Vector2(linecastLenght, 0.4f), 0, Vector2.down, 0.5f, groundLayer);
        return onGround;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(transform.position - Vector3.up * linecastOffsetY, new Vector2(linecastLenght, 0.4f));
    }
}
