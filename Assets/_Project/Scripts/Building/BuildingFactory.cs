using UnityEngine;

public static class BuildingFactory
{
    public static Building CreateBuilding(BuildingData data, Vector3 position, Team team)
    {
        if (data == null)
        {
            Debug.LogError("BuildingData is null!");
            return null;
        }

        // Создаем GameObject для здания
        GameObject buildingGO = new GameObject($"Building_{data.BuildingName}_{team}");
        buildingGO.transform.position = position;
        // Добавляем компоненты
        Building building = AddComponents(buildingGO, data);
        // Создаем визуальную часть
        CreateVisualPrefab(buildingGO, data);
        // Инициализируем здание с данными и командой
        building.Initialize(data, team);
        
        return building;
    }

    private static Building AddComponents(GameObject buildingGO, BuildingData data)
    {
        Building building = buildingGO.AddComponent<Building>();
        
        if (data.CanSpawnUnits)
        {
            buildingGO.AddComponent<UnitSpawner>();
        }
        
        return building;
    }

    private static void CreateVisualPrefab(GameObject buildingGO, BuildingData data)
    {
        if (data.BuildingVisualPrefab != null)
        {
            GameObject visual = GameObject.Instantiate(data.BuildingVisualPrefab);
            visual.transform.SetParent(buildingGO.transform);
            visual.transform.localPosition = Vector3.zero;
        }
    }
} 