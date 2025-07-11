using UnityEngine;

[CreateAssetMenu(fileName = "Building Name", menuName = "Autobattler/Building")]
public class BuildingData : ScriptableObject
{
    [Header("Basic Info")]
    [SerializeField] private string buildingName;
    
    [Header("Spawn Settings")]
    [SerializeField] private bool canSpawnUnits;
    [SerializeField] private UnitData unitToSpawn;
    [SerializeField] private float spawnInterval = 5f;

    [Header("Shooting Settings")]
    [SerializeField] private bool canShoot;
    
    [Header("Visual")]
    [SerializeField] private GameObject buildingVisualPrefab;
    
    // Properties
    public string BuildingName => buildingName;

    public bool CanShoot => canShoot;
    public bool CanSpawnUnits => canSpawnUnits;
    public UnitData UnitToSpawn => unitToSpawn;

    public float SpawnInterval => spawnInterval;
    
    public GameObject BuildingVisualPrefab => buildingVisualPrefab;
} 