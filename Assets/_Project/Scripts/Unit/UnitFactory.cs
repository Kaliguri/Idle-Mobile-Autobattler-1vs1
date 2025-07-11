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
        GameObject unitGO = new GameObject($"Unit_{data.unitName}_{team}");
        unitGO.transform.position = position;
        // Добавляем компоненты
        Unit unit = AddComponents(unitGO);
        // Создаем визуальную часть
        CreateVisualPrefab(unitGO, data);
        // Инициализируем юнит
        unit.Initialize();
        
        return unit;
    }

    private static Unit AddComponents(GameObject unitGO)
    {
        Unit unit = unitGO.AddComponent<Unit>();
        UnitAI ai = unitGO.AddComponent<UnitAI>();
        HP health = unitGO.AddComponent<HP>();
        
        return unit;
    }

    private static void CreateVisualPrefab(GameObject unitGO, UnitData data)
    {
        if (data.unitVisualPrefab != null)
        {
            GameObject visual = GameObject.Instantiate(data.unitVisualPrefab);
            visual.transform.SetParent(unitGO.transform);
            visual.transform.localPosition = Vector3.zero;
        }
    }
}

public enum Team
{
    Player,
    Enemy
} 