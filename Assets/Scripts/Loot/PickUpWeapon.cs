using Riptide;
using UnityEngine;

public class PickUpWeapon : MonoBehaviour
{
    [SerializeField] GameObject weaponPrefab;

    public ushort id;
    public string weaponName;

    const string PLAYER = "Player";

    [SerializeField] private Weapon weaponId;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (CharValues.GetValueOfString(player.meleeScript.gameObject.name) == CharValues.GetValueOfString(weaponPrefab.name)) return;

            if (weaponId == Weapon.melee)
            {
                Destroy(player.meleeScript.gameObject);
            }
            else
            {
                Destroy(player.gunScript.gameObject);
            }

            GameObject newWeapon = Instantiate(weaponPrefab, collision.transform.position, Quaternion.identity, collision.transform);
            player.SetWeapon(newWeapon, newWeapon.GetComponent<WeaponScript>(), weaponName, weaponId, id);

            LootBag.droppedItems.Remove(id);
            Destroy(this.gameObject);
        }
    }
}
