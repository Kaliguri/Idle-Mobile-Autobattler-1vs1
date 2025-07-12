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
        
        // Спавним в сети
        networkObject.Spawn();
        
        // Инициализируем юнит через ServerRpc (отправляет данные всем клиентам)
        unit.InitializeServerRpc(team, data.UnitName, data.MaxHealth);
        
        return unit;
    }
}

public enum Team
{
    Player1,
    Player2,
    Neutral
} 