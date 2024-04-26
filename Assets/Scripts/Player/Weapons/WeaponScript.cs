using Riptide;
using System.Collections;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] PolygonCollider2D polygonCollider;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] GameObject bulletPrefab;

    [SerializeField] private float minimumDamage;
    [SerializeField] private float maximumDamage;
    [SerializeField] public float attackTime;
    [Space(4)]
    [Header("0 -> Melee; 1 -> Ragned")]
    [SerializeField] private ushort weaponId;

    private float angle;

    private bool attackInput;
    private bool isAttacking = false;
    private bool meleeDestroyed;

    const string ENEMY = "Enemy";

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void GetInputs(bool attackInput)
    {
        this.attackInput = attackInput;
    }

    private void FixedUpdate()
    {
        if (attackInput && !isAttacking)
        {
            switch (weaponId)
            {
                case (ushort)Weapon.melee:
                    SendMessage(ServerToClientID.meleeAttack);
                    StartCoroutine(MeleeAttack(attackTime));
                    break;
                case (ushort)Weapon.ranged:
                    SendMessage(ServerToClientID.gunAttack);
                    GunAttack();
                    break;
            }
        }
    }

    IEnumerator MeleeAttack(float time)
    {
        polygonCollider.enabled = true;
        isAttacking = true;
        sprite.color = Color.yellow;
        yield return new WaitForSeconds(time);
        polygonCollider.enabled = false;
        isAttacking = false;
        sprite.color = Color.white;
    }

    private void GunAttack()
    {
        Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle));
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientID.gunAttack);
        message.AddUShort(player.Id);
        NetworkManager.Singleton.Server.SendToAll(message);
        isAttacking = true;
        Invoke("AttackEnd", attackTime);
    }

    private void AttackEnd()
    {
        isAttacking = false;
    }

    public void SetAngle(float angle)
    {
        this.angle = angle;
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientID.gunAngle);
        message.AddUShort(player.Id);
        message.AddFloat(this.angle);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(ENEMY))
        {
            Health health = collision.gameObject.GetComponent<Health>();
            health.health -= Random.Range(minimumDamage, maximumDamage);
        }
    }

    public void SendMessage(ServerToClientID id)
    {
        Message message = Message.Create(MessageSendMode.Reliable, id);
        message.AddUShort(player.Id);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
}