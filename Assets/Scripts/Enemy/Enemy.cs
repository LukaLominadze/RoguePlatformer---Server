using Riptide;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Dictionary<ushort, Enemy> enemyList = new Dictionary<ushort, Enemy>();

    public ushort Id => id;
    [SerializeField] private ushort id;

    private void Start()
    {
        enemyList.Add(Id, this);
    }

    private void OnDestroy()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientID.enemyDied);
        message.Add(Id);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    private static void CreateEnemy(ushort toClientId)
    {
        for(ushort i = 0;  i < enemyList.Count; i++)
        {
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientID.enemySpawned);
            message.AddUShort(i); //id
            message.AddVector2(enemyList[i].transform.position);
            Debug.Log(message.GetVector2());
            NetworkManager.Singleton.Server.Send(message, toClientId);
        }
    }

    [MessageHandler((ushort)ClientToServerID.enemySpawned)]
    private static void SendEnemyInfo(ushort fromClientId, Message message)
    {
        CreateEnemy(fromClientId);
    }
}
