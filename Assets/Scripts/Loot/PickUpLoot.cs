using UnityEngine;

public class PickUpLoot : MonoBehaviour
{
    [SerializeField] GameObject itemPrefab;

    const string PLAYER = "PLAYER";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(PLAYER))
        {
            //instantiate prefab
        }
    }
}
