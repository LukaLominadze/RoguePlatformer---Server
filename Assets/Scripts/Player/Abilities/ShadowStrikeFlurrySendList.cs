using Riptide;
using UnityEngine;

public class ShadowStrikeFlurrySendList : MonoBehaviour
{
    [SerializeField] AbilityController abilityController;
    [SerializeField] ShadowStrikeFlurryList list;
    [SerializeField] ShadowStrikeFlurryAttack attackScript;
    [Space(8)]
    [SerializeField] Transform player;
    [SerializeField] Rigidbody2D playerBody;

    [SerializeField] private float firstIterationTime;
    [SerializeField] private float secondIterationTime;

    int i = 0;

    public Vector2 startPosition;

    private void OnEnable()
    {
        startPosition = player.position;

        i = 0;

        list.changeList = false;

        playerBody.gravityScale = 0;

        EnemyMovement(false);

        SendMessage();
    }

    private void FixedUpdate()
    {
        if (i >= list.enemyList.Count)
        {
            attackScript.Attack();
            return;
        }
        else
        {
            firstIterationTime -= Time.fixedDeltaTime;
            if (firstIterationTime <= 0)
            {
                firstIterationTime = secondIterationTime;
                SendMessage();
            }
        }
    }

    private void OnDisable()
    {
        EnemyMovement(true);
    }

    private void SendMessage()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientID.abilityEvent);
        message.AddVector2(list.enemyList[i].position);
        NetworkManager.Singleton.Server.SendToAll(message);
        i++;
    }

    public void ResetIndex()
    {
        i = 0;
    }

    private void EnemyMovement(bool value)
    {
        foreach (Transform enemy in list.enemyList)
        {
            EnemyController controller = enemy.gameObject.GetComponent<EnemyController>();
            controller.enabled = value;
        }
    }
}
