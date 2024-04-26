using Riptide;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id { get; private set; }
    public string Username { get; private set; }

    public WeaponScript meleeScript;
    public WeaponScript gunScript;
    public WeaponController weaponController;

    [SerializeField] PlayerMovement movement;
    [SerializeField] RangedWeapons rangedScript;
    [SerializeField] AbilityController abilityController;

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    public static void Spawn(ushort id, string username)
    {
        foreach (Player players in list.Values)
        {
            players.SendSpawned(id);
        }

        Player player = Instantiate(GameLogic.Singleton.PlayerPrefab, GameLogic.Singleton.PlayerPrefab.transform.position, Quaternion.identity).GetComponent<Player>();
        player.Id = id;
        player.name = $"Player {id} -> {(string.IsNullOrEmpty(username) ? $"Guest" : username)}";
        player.Username = string.IsNullOrEmpty(username) ? $"Guest {id}" : username;

        player.SendSpawned();
        list.Add(id, player);
    }

    private void SendSpawned()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientID.playerSpawned);
        AddSpawnData(message);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    private void SendSpawned(ushort toClientId)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientID.playerSpawned);
        AddSpawnData(message);
        NetworkManager.Singleton.Server.Send(message, toClientId);
    }

    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Id);
        message.AddString(Username);
        message.AddVector2(transform.position);
        return message;
    }

    public void SetWeapon(GameObject weapon, WeaponScript weaponScript, string weaponName, Weapon weaponId, ushort droppedItemId)
    {
        switch (weaponId)
        {
            case Weapon.melee:
                meleeScript = weaponScript;
                weaponController.melee = weapon;
                SendWeapon(weaponName, droppedItemId);
                break;
            case Weapon.ranged:
                gunScript = weaponScript;
                weaponController.ranged = weapon;
                SendWeapon(weaponName, droppedItemId);
                break;
        }
    }

    private void SendWeapon(string weaponName, ushort droppedItemId)
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientID.pickUpMelee);
        message.AddUShort(this.Id);
        message.AddUShort(CharValues.GetValueOfString(weaponName));
        message.AddUShort(droppedItemId);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    [MessageHandler((ushort)ClientToServerID.name)]
    private static void Name(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());
    }

    [MessageHandler((ushort)ClientToServerID.input)]
    private static void GetInputs(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
        {
            player.movement.GetInputs(message.GetShort(), message.GetBools());
            player.rangedScript.GetInputs(message.GetBool());
            player.abilityController.GetInputs(message.GetBool(), fromClientId);
        }
    }

    [MessageHandler((ushort)ClientToServerID.weaponState)]
    private static void GetWeaponState(ushort fromClientId, Message message)
    {
        ushort id = message.GetUShort();
        if (list.TryGetValue(id, out Player player))
        {
            player.weaponController.SetCurrentId(message.GetUShort());
        }
    }


    [MessageHandler((ushort)ClientToServerID.meleeAttack)]
    private static void GetMeleeInput(ushort fromClientId, Message message)
    {
        ushort id = message.GetUShort();
        if (list.TryGetValue(id, out Player player))
        {
            player.meleeScript.GetInputs(message.GetBool());
        }
    }

    [MessageHandler((ushort)ClientToServerID.gunAngle)]
    private static void GetAngle(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.gunScript.SetAngle(message.GetFloat());
        }
    }

    [MessageHandler((ushort)ClientToServerID.gunAttack)]
    private static void GetGunInput(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.gunScript.GetInputs(message.GetBool());
        }
    }
}
