using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private static GameLogic _singleton;
    public static GameLogic Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
                if (_singleton != value)
                {
                    Destroy(value);
                }
            }
        }
    }

    public GameObject PlayerPrefab => playerPrefab;
    public GameObject EnemyPrefab => enemyPrefab;

    [Header("Prefabs")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject enemyPrefab;

    private void Awake()
    {
        Singleton = this;
    }
}
