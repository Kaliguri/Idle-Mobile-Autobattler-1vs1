using Unity.Netcode;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField] protected BuildingData buildingData;
    private NetworkVariable<Team> team = new NetworkVariable<Team>();
    private NetworkVariable<bool> isActive = new NetworkVariable<bool>(true);

    public BuildingData BuildingData => buildingData;
    public Team Team => team.Value;
    public bool IsActive => isActive.Value;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public void SetBuildingData(BuildingData data)
    {
        buildingData = data;
    }

    [ServerRpc(RequireOwnership = false)]
    public void InitializeServerRpc(Team buildingTeam, string buildingName)
    {
        if (!IsServer) return;
        
        team.Value = buildingTeam;
        isActive.Value = true;

        // Отправляем всем клиентам имя здания для создания визуала
        CreateVisualClientRpc(buildingName);

        if (buildingData != null && buildingData.CanSpawnUnits)
        {
            // Включаем и инициализируем UnitSpawner после установки данных
            UnitSpawner spawner = GetComponent<UnitSpawner>();
            if (spawner != null)
            {
                spawner.enabled = true;
                spawner.Initialize(this);
            }
        }
    }

    [ClientRpc]
    private void CreateVisualClientRpc(string buildingName)
    {
        // Находим BuildingData по имени
        BuildingData data = FindBuildingDataByName(buildingName);
        
        // Устанавливаем BuildingData на всех клиентах
        buildingData = data;
        
        // Создаем визуальную часть
        if (data != null && data.BuildingVisualPrefab != null)
        {
            GameObject visual = GameObject.Instantiate(data.BuildingVisualPrefab);
            visual.transform.SetParent(transform);
            visual.transform.localPosition = Vector3.zero;
        }
    }
    
    private BuildingData FindBuildingDataByName(string buildingName)
    {
        // Ищем BuildingData в ресурсах по имени
        BuildingData[] allBuildingData = Resources.FindObjectsOfTypeAll<BuildingData>();
        foreach (BuildingData data in allBuildingData)
        {
            if (data.BuildingName == buildingName)
            {
                return data;
            }
        }
        
        Debug.LogError($"BuildingData с именем '{buildingName}' не найден!");
        return null;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetActiveServerRpc(bool active)
    {
        if (IsServer)
        {
            isActive.Value = active;
        }
    }
} 