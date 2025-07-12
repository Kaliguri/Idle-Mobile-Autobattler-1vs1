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
    public void InitializeServerRpc(Team unitTeam, string unitName, float maxHealth)
    {
        team.Value = unitTeam;
        isActive.Value = true;
        
        // Отправляем всем клиентам данные для создания визуала
        CreateVisualClientRpc(unitName);
        
        // Инициализируем HP на сервере
        HP health = GetComponent<HP>();
        if (health != null)
        {
            health.InitializeServerRpc(maxHealth);
        }
        
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

    [ClientRpc]
    private void CreateVisualClientRpc(string unitName)
    {
        // Находим UnitData по имени
        UnitData data = FindUnitDataByName(unitName);
        
        // Устанавливаем UnitData на всех клиентах
        unitData = data;
        
        // Создаем визуальную часть
        if (data != null && data.UnitVisualPrefab != null)
        {
            GameObject visual = GameObject.Instantiate(data.UnitVisualPrefab);
            visual.transform.SetParent(transform);
            visual.transform.localPosition = Vector3.zero;
        }
    }
    
    private UnitData FindUnitDataByName(string unitName)
    {
        // Ищем UnitData в ресурсах по имени
        UnitData[] allUnitData = Resources.FindObjectsOfTypeAll<UnitData>();
        foreach (UnitData data in allUnitData)
        {
            if (data.UnitName == unitName)
            {
                return data;
            }
        }
        
        Debug.LogError($"UnitData с именем '{unitName}' не найден!");
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

