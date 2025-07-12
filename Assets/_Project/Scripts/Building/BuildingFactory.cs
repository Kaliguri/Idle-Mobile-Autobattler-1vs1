using Unity.Netcode;
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

        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("CreateBuilding can only be called on server!");
            return null;
        }

        // Создаем здание из префаба
        GameObject buildingGO = Object.Instantiate(NetworkDBSingleton.Instance.BuildingNetworkPrefab);
        buildingGO.name = $"Building_{data.BuildingName}_{team}";
        buildingGO.transform.position = position;
        
        // Получаем компоненты из префаба
        NetworkObject networkObject = buildingGO.GetComponent<NetworkObject>();
        Building building = buildingGO.GetComponent<Building>();
        
        // Устанавливаем BuildingData
        building.SetBuildingData(data);
        
        // Создаем визуальную часть
        CreateVisualPrefab(buildingGO, data);
        
        // Спавним в сети
        networkObject.Spawn();
        
        // Инициализируем здание через ServerRpc
        building.InitializeServerRpc(team);
        
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