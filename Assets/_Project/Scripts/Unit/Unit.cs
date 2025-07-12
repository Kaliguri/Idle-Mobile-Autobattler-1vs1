using Unity.Netcode;
using UnityEngine;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnitData unitData;
    private NetworkVariable<Team> team = new NetworkVariable<Team>();
    private NetworkVariable<bool> isActive = new NetworkVariable<bool>(true);

    public UnitData UnitData => unitData;
    public Team Team => team.Value;
    public bool IsActive => isActive.Value;

    public void SetUnitData(UnitData data)
    {
        unitData = data;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        // Регистрируем юнит в BattleInfo при спавне
        if (BattleInfoSingleton.Instance != null)
        {
            BattleInfoSingleton.Instance.RegisterUnit(this);
        }
    }

    public override void OnNetworkDespawn()
    {
        // Удаляем юнит из BattleInfo при деспавне
        if (BattleInfoSingleton.Instance != null)
        {
            BattleInfoSingleton.Instance.UnregisterUnit(this);
        }
        
        base.OnNetworkDespawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void InitializeServerRpc(Team unitTeam)
    {
        team.Value = unitTeam;
        isActive.Value = true;
        
        // Включаем и инициализируем UnitAI только на сервере
        if (IsServer)
        {
            UnitAI unitAI = GetComponent<UnitAI>();
            if (unitAI != null)
            {
                unitAI.enabled = true;
                unitAI.Initialize(this);
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

