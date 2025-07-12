using Unity.Netcode;
using UnityEngine;

public static class UnitFactory
{
    public static Unit CreateUnit(UnitData data, Vector3 position, Team team)
    {
        if (data == null)
        {
            Debug.LogError("UnitData is null!");
            return null;
        }

        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("CreateUnit can only be called on server!");
            return null;
        }

        // Создаем юнит из префаба
        GameObject unitGO = Object.Instantiate(NetworkDBSingleton.Instance.UnitNetworkPrefab);
        unitGO.name = $"Unit_{data.UnitName}_{team}";
        unitGO.transform.position = position;
        
        // Получаем компоненты из префаба
        NetworkObject networkObject = unitGO.GetComponent<NetworkObject>();
        Unit unit = unitGO.GetComponent<Unit>();
        
        // Устанавливаем UnitData
        unit.SetUnitData(data);
        
        // Создаем визуальную часть
        CreateVisualPrefab(unitGO, data);
        
        // Спавним в сети
        networkObject.Spawn();
        
        // Инициализируем юнит через ServerRpc
        unit.InitializeServerRpc(team);
        
        // Инициализируем HP через ServerRpc
        HP health = unit.GetComponent<HP>();
        if (health != null)
        {
            health.InitializeServerRpc(data.MaxHealth);
        }
        
        return unit;
    }

    private static void CreateVisualPrefab(GameObject unitGO, UnitData data)
    {
        if (data.UnitVisualPrefab != null)
        {
            GameObject visual = GameObject.Instantiate(data.UnitVisualPrefab);
            visual.transform.SetParent(unitGO.transform);
            visual.transform.localPosition = Vector3.zero;
        }
    }
}

public enum Team
{
    Player1,
    Player2,
    Neutral
} 