using UnityEngine;

[CreateAssetMenu]
public class Loot : ScriptableObject
{
    public Sprite lootSprite;
    public string lootName;
    [Space(3)]
    [Header(
            @"1-100
            Above 80 - Common
            Above 70 - Uncommon
            Above 40 - Rare
            Above 20 - Epic
            Above 0 - Legendary")]
    public int dropChance;

    public Loot(Sprite lootSprite, int dropChance)
    {
        this.lootSprite = lootSprite;
        this.dropChance = dropChance;
    }
}
