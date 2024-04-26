using Riptide;
using Riptide.Utils;
using UnityEngine;

public enum ServerToClientID : ushort
{
    sync,
    playerSpawned = 1,
    playerMovement,
    serverEvent,
    weaponState,
    meleeAttack,
    gunAngle,
    gunAttack,
    spawnBullet,
    rangedEvent,
    enemySpawned,
    enemyDied,
    enemyMovement,
    abilityEvent,
    destroyIndicators,
    droppedItems,
    pickUpMelee,
}

public enum ClientToServerID : ushort
{
    name = 1,
    input,
    enemySpawned,
    weaponState,
    meleeAttack,
    gunAttack,
    gunAngle,
}

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _singleton;
    public static NetworkManager Singleton
    {
        get => _singleton;
        private set
        {
            if(_singleton == null)
            {
                _singleton = value;
                if(_singleton != value)
                {
                    Destroy(value);
                }
            }
        }
    }

    public Server Server { get; private set; }
    public ushort CurrentTick { get; private set; } = 0;

    [SerializeField] private ushort port;
    [SerializeField] private ushort maxClientNumber;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, true);

        Server = new Server();
        Server.Start(port, maxClientNumber);
        Server.ClientConnected += PlayerConnected;
        Server.ClientDisconnected += PlayerDisconnected;
    }

    private void FixedUpdate()
    {
        Server.Update();

        if(CurrentTick % 250 == 0)
        {
            if(CurrentTick % 6500 == 0)
            {
                CurrentTick = 0;
            }
            //send tick to the client
            Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientID.sync);
            message.AddUShort(CurrentTick);

            Server.SendToAll(message);
        }

        CurrentTick++;
    }

    private void PlayerConnected(object sender, ServerConnectedEventArgs e)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientID.sync);
        message.AddShort((short)CurrentTick);
        Server.SendToAll(message);
    }

    private void PlayerDisconnected(object sender, ServerDisconnectedEventArgs e)
    {
        Destroy(Player.list[e.Client.Id].gameObject);
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientID.serverEvent);
        message.AddUShort(e.Client.Id);
        Server.SendToAll(message);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }
}
