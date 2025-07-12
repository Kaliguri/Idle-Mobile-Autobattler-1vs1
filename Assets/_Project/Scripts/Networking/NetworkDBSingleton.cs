using UnityEngine;

public class NetworkDBSingleton : MonoBehaviour
{
    private static NetworkDBSingleton instance;

    [Header("Network Prefabs")]
    [SerializeField] private GameObject unitNetworkPrefab;
    [SerializeField] private GameObject buildingNetworkPrefab;

    public static NetworkDBSingleton Instance => instance;
    
    // Публичные свойства для доступа к префабам
    public GameObject UnitNetworkPrefab => unitNetworkPrefab;
    public GameObject BuildingNetworkPrefab => buildingNetworkPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
} 