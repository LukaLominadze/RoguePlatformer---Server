using Riptide;
using System.Collections.Generic;
using UnityEngine;

public class LootBag : MonoBehaviour
{
    public static Dictionary<ushort, GameObject> droppedItems = new Dictionary<ushort, GameObject>();

    private string lootName;

    [SerializeField] GameObject lootPrefab;
    [Space(2)]
    [Header(
            @"1-100
            Above 80 - Common
            Above 70 - Uncommon
            Above 40 - Rare
            Above 20 - Epic
            Above 0 - Legendary")]

    [SerializeField] private List<Loot> lootList = new List<Loot>();

    private int randomItem;

    Loot GetDroppedItem()
    {
        int randomNumber = Random.Range(1, 101);
        List<Loot> possibleItems = new List<Loot>();
        for(int i = 0; i < lootList.Count; i++)
        {
            if(randomNumber <= lootList[i].dropChance)
            {
                possibleItems.Add(lootList[i]);
            }
        }
        if (possibleItems.Count > 0)
        {
            randomItem = Random.Range(0, possibleItems.Count);
            Loot droppedItem = possibleItems[randomItem];
            lootName = possibleItems[randomItem].lootName;
            return droppedItem;
        }
        return null;
    }

    public void InstantiateLoot(Vector2 spawnPosition)
    {
        Loot droppedItem = GetDroppedItem();
        if (droppedItem != null)
        {
            Message message = Message.Create(MessageSendMode.Reliable, ServerToClientID.droppedItems);
            message.AddUShort(CharValues.GetValueOfString(lootName));
            message.AddVector2(spawnPosition);
            GameObject lootGameObject = Instantiate(LootPrefabList.prefabDictionary[lootName], spawnPosition, Quaternion.identity);
            ushort id = (ushort)droppedItems.Count;
            droppedItems.Add(id, lootGameObject);
            message.AddUShort(id);
            lootGameObject.GetComponent<SpriteRenderer>().sprite = droppedItem.lootSprite;
            lootGameObject.GetComponent<PickUpWeapon>().id = id;

            NetworkManager.Singleton.Server.SendToAll(message);

            LootBag.droppedItems.Remove(id);
            Destroy(this.gameObject);
        }
    }
}