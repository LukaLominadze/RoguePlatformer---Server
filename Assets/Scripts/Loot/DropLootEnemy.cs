using UnityEngine;

public class DropLootEnemy : MonoBehaviour
{
    [SerializeField] LootBag lootBag;
    [SerializeField] Health health;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(health.health <= 0)
        {
            lootBag.InstantiateLoot(transform.position);
            Destroy(gameObject);
        }
    }
}
