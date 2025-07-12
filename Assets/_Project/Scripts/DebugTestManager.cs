using Unity.Netcode;
using UnityEngine;

public class DebugTestManager : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private KeyCode testKey = KeyCode.T;
    
    private void Update()
    {
        if (enableDebugLogs && Input.GetKeyDown(testKey))
        {
            RunDebugTest();
        }
    }
    
    private void RunDebugTest()
    {
        // Debug.Log("=== НАЧАЛО ОТЛАДОЧНОГО ТЕСТА ===");
        
        // Проверяем BattleInfoSingleton
        if (BattleInfoSingleton.Instance != null)
        {
            // Debug.Log("✅ BattleInfoSingleton найден");
            
            // Проверяем WinPoint
            WinPoint player1WinPoint = BattleInfoSingleton.Instance.GetWinPointForTeam(Team.Player1);
            WinPoint player2WinPoint = BattleInfoSingleton.Instance.GetWinPointForTeam(Team.Player2);
            
            // Debug.Log($"WinPoint для Player1: {(player1WinPoint != null ? player1WinPoint.name : "НЕ НАЙДЕН")}");
            // Debug.Log($"WinPoint для Player2: {(player2WinPoint != null ? player2WinPoint.name : "НЕ НАЙДЕН")}");
            
            // Проверяем юниты
            var player1Units = BattleInfoSingleton.Instance.GetAllUnitsInTeam(Team.Player1);
            var player2Units = BattleInfoSingleton.Instance.GetAllUnitsInTeam(Team.Player2);
            
            // Debug.Log($"Юниты Player1: {player1Units.Count}");
            // Debug.Log($"Юниты Player2: {player2Units.Count}");
            
            // Проверяем каждый юнит
            foreach (var unit in player1Units)
            {
                if (unit != null)
                {
                    // Debug.Log($"Юнит Player1: {unit.name}, Позиция: {unit.transform.position}, Активен: {unit.IsActive}");
                }
            }
            
            foreach (var unit in player2Units)
            {
                if (unit != null)
                {
                    // Debug.Log($"Юнит Player2: {unit.name}, Позиция: {unit.transform.position}, Активен: {unit.IsActive}");
                }
            }
        }
        else
        {
            // Debug.LogError("❌ BattleInfoSingleton НЕ НАЙДЕН");
        }
        
        // Проверяем NetworkManager
        if (NetworkManager.Singleton != null)
        {
            // Debug.Log($"✅ NetworkManager найден. IsServer: {NetworkManager.Singleton.IsServer}, IsClient: {NetworkManager.Singleton.IsClient}");
        }
        else
        {
            // Debug.LogError("❌ NetworkManager НЕ НАЙДЕН");
        }
        
        // Debug.Log("=== КОНЕЦ ОТЛАДОЧНОГО ТЕСТА ===");
    }
} 