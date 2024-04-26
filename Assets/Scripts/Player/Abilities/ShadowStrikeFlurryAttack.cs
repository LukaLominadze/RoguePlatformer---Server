using Riptide;
using UnityEngine;

public class ShadowStrikeFlurryAttack : MonoBehaviour
{
    [SerializeField] AbilityController abilityController;
    [SerializeField] ShadowStrikeFlurryList list;
    [SerializeField] ShadowStrikeFlurrySendList sendList;
    [SerializeField] Transform player;
    [SerializeField] Rigidbody2D playerBody;
    [SerializeField] WeaponScript melee;

    [SerializeField] private float teleportationWindowTime;

    private int i = 0;
    private int direction = 1;

    private float originalTeleportationWindowTime;

    private void Start()
    {
        originalTeleportationWindowTime = teleportationWindowTime;
    }

    public void Attack()
    {
        teleportationWindowTime -= Time.fixedDeltaTime;

        if (i == list.enemyList.Count)
        {
            player.position = sendList.startPosition;
            playerBody.gravityScale = 1;
            i = 0;
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientID.destroyIndicators);
            NetworkManager.Singleton.Server.SendToAll(message);
            abilityController.abilityState = AbilityState.cooldown;
            return;
        }
        if (teleportationWindowTime <= 0)
        {
            melee.SendMessage(ServerToClientID.gunAttack);
            teleportationWindowTime = originalTeleportationWindowTime;
            player.position = (Vector2)list.enemyList[i].position + direction * Vector2.right;
            player.localScale = new Vector3(-direction, 1, 1);
            direction *= -1;
            i++;
            //play attack animation
        }
    }
}
