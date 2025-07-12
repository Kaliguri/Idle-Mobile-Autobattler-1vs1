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

    public void SetBuildingData(BuildingData data)
    {
        buildingData = data;
    }

    [ServerRpc(RequireOwnership = false)]
    public void InitializeServerRpc(Team buildingTeam)
    {
        if (!IsServer) return;
        
        team.Value = buildingTeam;
        isActive.Value = true;

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

    [ServerRpc(RequireOwnership = false)]
    public void SetActiveServerRpc(bool active)
    {
        if (IsServer)
        {
            isActive.Value = active;
        }
    }
} 