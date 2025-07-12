using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;

public class NetworkConnectionManager : MonoBehaviour
{
    [Header("Relay Settings")]
    private const int MAX_PLAYERS = 2;
    private string currentJoinCode = "";
    
    private async void Start()
    {
        // Проверяем, что NetworkManager существует
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager не найден на сцене!");
            return;
        }
        
        // Инициализируем Unity Gaming Services
        await InitializeUnityServices();
    }
    
    private async Task InitializeUnityServices()
    {
        try
        {
            // Инициализация Unity Gaming Services
            await UnityServices.InitializeAsync();
            
            // Анонимная аутентификация
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            
            Debug.Log("Unity Gaming Services инициализированы");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка инициализации Unity Gaming Services: {e.Message}");
        }
    }
    
    /// <summary>
    /// Отключается от сервера
    /// </summary>
    public void Disconnect()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager не найден!");
            return;
        }
        
        if (NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
            // Debug.Log("✅ Отключено от сервера");
        }
        else
        {
            Debug.LogWarning("Не подключен к серверу!");
        }
    }
    
    /// <summary>
    /// Проверяет, подключен ли к серверу
    /// </summary>
    public bool IsConnected()
    {
        return NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;
    }
    
    /// <summary>
    /// Проверяет, является ли хостом
    /// </summary>
    public bool IsHost()
    {
        return NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost;
    }
    
    /// <summary>
    /// Проверяет, является ли сервером
    /// </summary>
    public bool IsServer()
    {
        return NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer;
    }
    
    /// <summary>
    /// Проверяет, является ли клиентом
    /// </summary>
    public bool IsClient()
    {
        return NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient;
    }
    
    /// <summary>
    /// Создает игру через Relay и возвращает код для подключения
    /// </summary>
    public async void CreateGameWithRelay()
    {
        try
        {
            // Создаем Relay allocation
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MAX_PLAYERS);
            
            // Получаем Join Code
            currentJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            
            // Отображаем код в UI через MainMenuUIManager
            if (MainMenuUIManager.Instance != null)
            {
                MainMenuUIManager.Instance.DisplayCode(currentJoinCode);
            }
            
            // Настраиваем транспорт для Relay
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                allocation.RelayServer.IpV4, 
                (ushort)allocation.RelayServer.Port, 
                allocation.AllocationIdBytes, 
                allocation.Key, 
                allocation.ConnectionData
            );
            
            // Запускаем хост
            NetworkManager.Singleton.StartHost();
            
            Debug.Log($"Игра создана через Relay! Код: {currentJoinCode}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка создания игры через Relay: {e.Message}");
        }
    }
    
    /// <summary>
    /// Подключается к игре по коду через Relay
    /// </summary>
    public async void JoinGameWithRelay()
    {
        try
        {
            string joinCode = "";
            
            // Получаем код из MainMenuUIManager
            if (MainMenuUIManager.Instance != null)
            {
                joinCode = MainMenuUIManager.Instance.GetInputCode();
            }
            
            if (string.IsNullOrEmpty(joinCode))
            {
                Debug.LogError("Введите код для подключения!");
                return;
            }
            
            // Подключаемся к Relay
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            
            // Настраиваем транспорт для Relay
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                allocation.RelayServer.IpV4, 
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes, 
                allocation.Key, 
                allocation.ConnectionData, 
                allocation.HostConnectionData
            );
            
            // Подключаемся как клиент
            NetworkManager.Singleton.StartClient();
            
            Debug.Log($"Подключение к игре через Relay с кодом: {joinCode}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка подключения через Relay: {e.Message}");
        }
    }
    
    /// <summary>
    /// Получает текущий код подключения
    /// </summary>
    public string GetCurrentJoinCode()
    {
        return currentJoinCode;
    }
} 