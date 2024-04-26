using Riptide;
using UnityEngine;

public class RangedKnife : MonoBehaviour
{
    [SerializeField] Player playerScript;
    [SerializeField] Transform player;

    [SerializeField] private float movementSpeed;

    [SerializeField] private float offset;

    private float currentSpeed;

    private void Start()
    {
        currentSpeed = movementSpeed;
        transform.SetParent(null);
    }

    private void OnEnable()
    {
        transform.position = new Vector2(player.position.x + offset * player.localScale.x, player.position.y);
        currentSpeed = movementSpeed * player.localScale.x;
        SendMessage(true);
    }

    private void FixedUpdate()
    {
        transform.position += Vector3.right * currentSpeed;
        SendMessage(true);
    }

    private void OnDisable()
    {
        SendMessage(false);
    }

    private void SendMessage(bool value)
    {
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientID.rangedEvent);
        message.AddUShort(playerScript.Id);
        message.AddVector2(transform.position);
        message.AddBool(value);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
}
