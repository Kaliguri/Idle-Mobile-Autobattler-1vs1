using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BattleInfoSingleton : NetworkBehaviour
{
    private static BattleInfoSingleton instance;

    private List<Unit> allUnits = new List<Unit>();
    private List<WinPoint> allWinPoints = new List<WinPoint>();
    private NetworkVariable<float> gameTime = new NetworkVariable<float>(0f);

    public static BattleInfoSingleton Instance => instance;
    public float GameTime => gameTime.Value;

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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        // WinPoint объекты будут регистрировать себя сами в своем OnNetworkSpawn()
        // Это более надежный способ, чем FindObjectsOfType с задержкой
        Debug.Log("BattleInfoSingleton инициализирован");
    }

    private void Update()
    {
        if (IsServer)
        {
            gameTime.Value += Time.deltaTime;
        }
    }

    /// <summary>
    /// Ручная регистрация всех WinPoint на сцене (для отладки)
    /// </summary>
    [ContextMenu("Найти и зарегистрировать все WinPoint")]
    public void FindAndRegisterAllWinPoints()
    {
        WinPoint[] winPoints = FindObjectsOfType<WinPoint>();
        Debug.Log($"Найдено {winPoints.Length} WinPoint объектов на сцене");
        
        foreach (WinPoint winPoint in winPoints)
        {
            if (winPoint != null)
            {
                Debug.Log($"Проверяем WinPoint: {winPoint.name}, Team: {winPoint.Team}");
                if (winPoint.Team != Team.Neutral)
                {
                    RegisterWinPoint(winPoint);
                    Debug.Log($"Вручную зарегистрирован WinPoint для команды {winPoint.Team}: {winPoint.name}");
                }
                else
                {
                    Debug.LogWarning($"WinPoint {winPoint.name} имеет команду Neutral, пропускаем");
                }
            }
        }
        
        Debug.Log($"Всего зарегистрировано WinPoint: {allWinPoints.Count}");
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
            Debug.Log($"[BattleInfoSingleton] ✅ Зарегистрирован WinPoint: {winPoint.name}, команда: {winPoint.Team}. Всего: {allWinPoints.Count}");
        }
        else
        {
            Debug.LogWarning($"[BattleInfoSingleton] ⚠️ WinPoint {winPoint.name} уже зарегистрирован!");
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
        Debug.Log($"Ищем WinPoint для команды {team}. Всего зарегистрировано WinPoint: {allWinPoints.Count}");
        
        foreach (WinPoint winPoint in allWinPoints)
        {
            if (winPoint != null)
            {
                Debug.Log($"Проверяем WinPoint: {winPoint.name}, Team: {winPoint.Team}");
                if (winPoint.Team == team)
                {
                    Debug.Log($"Найден WinPoint для команды {team}: {winPoint.name}");
                    return winPoint;
                }
            }
        }
        
        Debug.LogWarning($"WinPoint для команды {team} не найден!");
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

    [ClientRpc]
    public void WinPointReachedClientRpc(Team team)
    {
        Debug.Log($"WinPoint достигнут командой {team}!");
        // Здесь можно добавить логику обработки достижения WinPoint
    }

    [ClientRpc]
    public void GameOverClientRpc(Team winnerTeam)
    {
        Debug.Log($"Игра окончена! Победила команда {winnerTeam}!");
        // Здесь можно добавить логику окончания игры
    }

    [ServerRpc(RequireOwnership = false)]
    public void CheckWinConditionServerRpc(Team team)
    {
        if (!IsServer) return;

        // Проверяем условие победы
        WinPoint winPoint = GetWinPointForTeam(team);
        if (winPoint != null)
        {
            // Логика проверки достижения WinPoint
            WinPointReachedClientRpc(team);
            
            // Если условие победы выполнено, объявляем победителя
            GameOverClientRpc(team);
        }
    }
} 