using System.Collections.Generic;
using UnityEngine;

public class LootPrefabList : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs = new List<GameObject>();

    public static Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    private void Start()
    {
        for (ushort i = 0; i < prefabs.Count; i++)
        {
            prefabDictionary.Add(prefabs[i].name, prefabs[i]);
        }
    }
}
