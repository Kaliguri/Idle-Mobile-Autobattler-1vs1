using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class UnitSpawner : NetworkBehaviour
{
    private Building building;
    private Coroutine spawnCoroutine;

    public void Initialize(Building buildingComponent)
    {
        building = buildingComponent;
        
        // Спавн работает только на сервере и когда компонент включен
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer && enabled)
        {
            StartSpawning();
        }
    }

    private void StartSpawning()
    {
        if (!NetworkManager.Singleton.IsServer || building == null || !enabled) return;
        
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (building != null && enabled)
        {
            yield return new WaitForSeconds(building.BuildingData.SpawnInterval);
            
            if (building.IsActive && NetworkManager.Singleton.IsServer)
            {
                SpawnUnit();
            }
        }
    }

    private void SpawnUnit()
    {
        if (!NetworkManager.Singleton.IsServer || building == null || !enabled) return;
        
        if (building.BuildingData != null && building.BuildingData.CanSpawnUnits && building.BuildingData.UnitToSpawn != null)
        {
            Vector3 spawnPosition = transform.position;
            UnitFactory.CreateUnit(building.BuildingData.UnitToSpawn, spawnPosition, building.Team);
        }
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    public override void OnNetworkDespawn()
    {
        // Очищаем корутины при отключении от сети
        StopSpawning();
        base.OnNetworkDespawn();
    }
} 