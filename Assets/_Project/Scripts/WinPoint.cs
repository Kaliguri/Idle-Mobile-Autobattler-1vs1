using Unity.Netcode;
using UnityEngine;

public class WinPoint : NetworkBehaviour
{
    [SerializeField] private Team initialTeam = Team.Player1;
    private NetworkVariable<Team> team = new NetworkVariable<Team>();

    public Team Team => team.Value;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        Debug.Log($"[WinPoint {name}] OnNetworkSpawn. IsServer: {IsServer}, initialTeam: {initialTeam}");
        
        // Инициализируем команду из значения в инспекторе
        if (IsServer)
        {
            team.Value = initialTeam;
            Debug.Log($"[WinPoint {name}] Сервер установил команду: {team.Value}");
        }
        
        // Добавляем 2D коллайдер для обнаружения юнитов, если его нет
        if (GetComponent<Collider2D>() == null)
        {
            var collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }
        
        // Подписываемся на изменение команды
        team.OnValueChanged += OnTeamChanged;
        
        // НА СЕРВЕРЕ: регистрируем сразу с initialTeam (не ждем синхронизации NetworkVariable)
        if (IsServer)
        {
            Debug.Log($"[WinPoint {name}] Сервер: пытаемся зарегистрировать с initialTeam: {initialTeam}");
            if (initialTeam != Team.Neutral)
            {
                RegisterSelf();
            }
            else
            {
                Debug.LogWarning($"[WinPoint {name}] Сервер: initialTeam = Neutral, не регистрируем");
            }
        }
        
        // НА КЛИЕНТЕ: ждем синхронизации через OnTeamChanged
        Debug.Log($"[WinPoint {name}] Текущее значение team.Value: {team.Value}");
    }
    
    private void OnTeamChanged(Team previousValue, Team newValue)
    {
        Debug.Log($"[WinPoint {name}] OnTeamChanged: {previousValue} → {newValue}. IsServer: {IsServer}");
        
        // Регистрируем WinPoint когда команда установлена (только на клиенте)
        if (!IsServer && newValue != Team.Neutral)
        {
            Debug.Log($"[WinPoint {name}] Клиент: регистрируем после синхронизации");
            RegisterSelf();
        }
    }
    
    public override void OnNetworkDespawn()
    {
        // Отписываемся от события
        team.OnValueChanged -= OnTeamChanged;
        base.OnNetworkDespawn();
    }
    
    private void RegisterSelf()
    {
        if (BattleInfoSingleton.Instance != null)
        {
            BattleInfoSingleton.Instance.RegisterWinPoint(this);
            Debug.Log($"[WinPoint {name}] ✅ ЗАРЕГИСТРИРОВАН для команды {Team}. IsServer: {IsServer}");
        }
        else
        {
            Debug.LogError($"[WinPoint {name}] ❌ BattleInfoSingleton.Instance == null!");
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;
        
        Unit unit = other.GetComponentInParent<Unit>();
        if (unit != null && unit.Team != Team && unit.Team != Team.Neutral)
        {
            // Юнит противника достиг WinPoint
            OnWinPointReachedClientRpc(unit.Team);
            
            // Уведомляем BattleInfoSingleton о достижении WinPoint
            if (BattleInfoSingleton.Instance != null)
            {
                BattleInfoSingleton.Instance.CheckWinConditionServerRpc(unit.Team);
            }
        }
    }

    [ClientRpc]
    private void OnWinPointReachedClientRpc(Team reachedByTeam)
    {
        Debug.Log($"WinPoint команды {Team} достигнут командой {reachedByTeam}!");
        
        // Здесь можно добавить визуальные эффекты или звуки
    }
} 