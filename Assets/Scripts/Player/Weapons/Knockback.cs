using UnityEngine;

public class Knockback : MonoBehaviour
{
    [SerializeField] Transform player;

    [SerializeField] private float knockbackForce;

    const string ENEMY = "Enemy";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(ENEMY))
        {
            if(collision.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D rbody))
            {
                rbody.AddForce(Vector2.right * player.localScale.x * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}
