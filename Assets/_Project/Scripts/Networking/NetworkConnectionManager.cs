using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkConnectionManager : MonoBehaviour
{
    [Header("Connection Settings")]
    [SerializeField] private string serverIP = "127.0.0.1";
    [SerializeField] private ushort serverPort = 7777;
    
    private void Start()
    {
        // Проверяем, что NetworkManager существует
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager не найден на сцене!");
        }
    }
    
    /// <summary>
    /// Запускает хост (сервер + клиент на одном устройстве)
    /// Навесить на кнопку Host
    /// </summary>
    public void StartHost()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager не найден!");
            return;
        }
        
        if (NetworkManager.Singleton.IsListening)
        {
            Debug.LogWarning("Уже подключен к серверу!");
            return;
        }
        
        NetworkManager.Singleton.StartHost();
        Debug.Log("✅ Запущен хост (сервер + клиент)");
    }
    
    /// <summary>
    /// Запускает сервер (только сервер, без клиента)
    /// Навесить на кнопку Server
    /// </summary>
    public void StartServer()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager не найден!");
            return;
        }
        
        if (NetworkManager.Singleton.IsListening)
        {
            Debug.LogWarning("Уже запущен сервер!");
            return;
        }
        
        NetworkManager.Singleton.StartServer();
        Debug.Log("✅ Запущен сервер");
    }
    
    /// <summary>
    /// Подключается к серверу как клиент
    /// </summary>
    public void StartClient()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager не найден!");
            return;
        }
        
        if (NetworkManager.Singleton.IsListening)
        {
            Debug.LogWarning("Уже подключен к серверу!");
            return;
        }
        
        // Настраиваем подключение к серверу
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null)
        {
            transport.SetConnectionData(serverIP, serverPort);
        }
        
        NetworkManager.Singleton.StartClient();
        Debug.Log($"✅ Подключение к серверу {serverIP}:{serverPort}");
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
            Debug.Log("✅ Отключено от сервера");
        }
        else
        {
            Debug.LogWarning("Не подключен к серверу!");
        }
    }
    
    /// <summary>
    /// Устанавливает IP адрес сервера
    /// </summary>
    public void SetServerIP(string ip)
    {
        serverIP = ip;
        Debug.Log($"IP сервера изменен на: {ip}");
    }
    
    /// <summary>
    /// Устанавливает порт сервера
    /// </summary>
    public void SetServerPort(ushort port)
    {
        serverPort = port;
        Debug.Log($"Порт сервера изменен на: {port}");
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
} 