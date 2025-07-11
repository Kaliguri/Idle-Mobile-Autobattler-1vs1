using UnityEngine;
using System.Collections;

public class UnitSpawner : MonoBehaviour
{
    private Building building;
    private Coroutine spawnCoroutine;

    public void Initialize(Building buildingComponent)
    {
        building = buildingComponent;
        StartSpawning();
    }

    private void StartSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(building.BuildingData.SpawnInterval);
            
            if (building.IsActive)
            {
                SpawnUnit();
            }
        }
    }

    private void SpawnUnit()
    {
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
} 