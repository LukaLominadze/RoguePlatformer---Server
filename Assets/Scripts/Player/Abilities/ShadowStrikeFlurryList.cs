using System.Collections.Generic;
using UnityEngine;

public class ShadowStrikeFlurryList : MonoBehaviour
{
    public List<Transform> enemyList = new List<Transform> ();

    const string ENEMY = "Enemy";

    public bool changeList = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!changeList) return;
        if (collision.CompareTag(ENEMY))
        {
            enemyList.Add(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!changeList) return;
        if (collision.CompareTag(ENEMY))
        {
            enemyList.Remove(collision.transform);
        }
    }

    public void ClearList()
    {
        for(int i = 0; i < enemyList.Count; i++)
        {
            enemyList.RemoveAt(0);
        }
        changeList = true;
    }
}
