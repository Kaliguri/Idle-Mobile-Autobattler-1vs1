using System.Collections.Generic;
using UnityEngine;

public class BattleInfoSingleton : MonoBehaviour
{
    private static BattleInfoSingleton instance;

    private List<Unit> allUnits = new List<Unit>();
    private List<WinPoint> allWinPoints = new List<WinPoint>();

    public static BattleInfoSingleton Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Автоматически находим и регистрируем все WinPoint на сцене
        FindAndRegisterAllWinPoints();
    }

    private void FindAndRegisterAllWinPoints()
    {
        WinPoint[] winPoints = FindObjectsOfType<WinPoint>();
        //Debug.Log($"Найдено {winPoints.Length} WinPoint объектов на сцене");
        
        foreach (WinPoint winPoint in winPoints)
        {
            if (winPoint != null && winPoint.Team != Team.Neutral)
            {
                RegisterWinPoint(winPoint);
                //Debug.Log($"Автоматически зарегистрирован WinPoint для команды {winPoint.Team}: {winPoint.name}");
            }
        }
    }

    public void RegisterUnit(Unit unit)
    {
        if (!allUnits.Contains(unit))
        {
            allUnits.Add(unit);
        }
    }

    public void UnregisterUnit(Unit unit)
    {
        allUnits.Remove(unit);
    }

    public void RegisterWinPoint(WinPoint winPoint)
    {
        if (!allWinPoints.Contains(winPoint))
        {
            allWinPoints.Add(winPoint);
        }
    }

    public Unit FindNearestEnemy(Vector3 position, Team team)
    {
        Unit nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (Unit unit in allUnits)
        {
            if (unit == null) continue;

            if (IsEnemy(unit, team))
            {
                float distance = Vector3.Distance(position, unit.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = unit;
                }
            }
        }

        return nearestEnemy;
    }

    public WinPoint GetWinPointForTeam(Team team)
    {
        foreach (WinPoint winPoint in allWinPoints)
        {
            if (winPoint != null && winPoint.Team == team)
            {
                return winPoint;
            }
        }
        return null;
    }

    private bool IsEnemy(Unit targetUnit, Team searcherTeam)
    {
        if (searcherTeam == Team.Neutral)
        {
            return targetUnit.Team != Team.Neutral;
        }

        return targetUnit.Team != searcherTeam && targetUnit.Team != Team.Neutral;
    }

    public List<Unit> GetAllUnitsInTeam(Team team)
    {
        List<Unit> teamUnits = new List<Unit>();
        foreach (Unit unit in allUnits)
        {
            if (unit != null && unit.Team == team)
            {
                teamUnits.Add(unit);
            }
        }
        return teamUnits;
    }

    public int GetUnitCountInTeam(Team team)
    {
        int count = 0;
        foreach (Unit unit in allUnits)
        {
            if (unit != null && unit.Team == team)
            {
                count++;
            }
        }
        return count;
    }
} 