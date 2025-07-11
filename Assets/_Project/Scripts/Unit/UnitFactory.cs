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

        // Создаем GameObject для юнита
        GameObject unitGO = new GameObject($"Unit_{data.UnitName}_{team}");
        unitGO.transform.position = position;
        // Добавляем компоненты
        Unit unit = AddComponents(unitGO, data);
        // Создаем визуальную часть
        CreateVisualPrefab(unitGO, data);
        // Инициализируем юнит с данными и командой
        unit.Initialize(data, team);
        
        // Регистрируем юнит в BattleInfo
        BattleInfoSingleton.Instance.RegisterUnit(unit);
        
        return unit;
    }

    private static Unit AddComponents(GameObject unitGO, UnitData data)
    {
        Unit unit = unitGO.AddComponent<Unit>();
        UnitAI ai = unitGO.AddComponent<UnitAI>();
        HP health = unitGO.AddComponent<HP>();
        
        // Инициализируем HP с данными из UnitData
        health.Initialize(data.MaxHealth);
        
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