using Riptide;
using UnityEngine;

public enum Weapon { melee, ranged }

public class WeaponController : MonoBehaviour
{
    [SerializeField] Player player;

    public GameObject melee;
    public GameObject ranged;

    private ushort id;
    private ushort currentId;

    private void Awake()
    {
        id = player.Id;
        currentId = (ushort)Weapon.melee;
    }

    private void WeaponState()
    {
        switch (currentId)
        {
            case (ushort)Weapon.melee:
                melee.SetActive(true);
                ranged.SetActive(false);
                SendMessage();
                break;
            case (ushort)Weapon.ranged:
                melee.SetActive(false);
                ranged.SetActive(true);
                SendMessage();
                break;
        }
    }

    public void SetCurrentId(ushort newId)
    {
        currentId = newId;
        Debug.Log(currentId);
        WeaponState();
    }

    public void SetMelee(GameObject newMelee)
    {
        melee = newMelee;
    }

    public void SetRanged(GameObject newRanged)
    {
        ranged = newRanged;
    }

    private void SendMessage()
    {
        Message message = Message.Create(MessageSendMode.Reliable, ServerToClientID.weaponState);
        message.AddUShort(player.Id);
        message.AddUShort(currentId);
        NetworkManager.Singleton.Server.SendToAll(message);
        Debug.Log("sentSwitch");
    }
}
