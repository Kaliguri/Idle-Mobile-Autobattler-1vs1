using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerSingleton : MonoBehaviour
{
    private static SceneManagerSingleton instance;
    public static SceneManagerSingleton Instance => instance;

    [Header("Scene Settings")]
    [SerializeField] private string lobbySceneName = "Main Menu";
    [SerializeField] private string gameSceneName = "Gameplay";
    
    [Header("Player Count")]
    [SerializeField] private int requiredPlayers = 2;
    
    [Header("UI Settings")]
    [SerializeField] private float sceneLoadDelay = 1f;
    
    private int currentPlayerCount = 0;
    private bool gameStarted = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SubscribeToNetworkEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromNetworkEvents();
    }

    private void SubscribeToNetworkEvents()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.OnClientStarted += OnClientStarted;
        }
    }

    private void UnsubscribeFromNetworkEvents()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            NetworkManager.Singleton.OnClientStarted -= OnClientStarted;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        currentPlayerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
        Debug.Log($"Игрок подключился! ID: {clientId}, Всего игроков: {currentPlayerCount}");
        
        // Если подключился другой игрок и мы сервер, проверяем условия
        if (NetworkManager.Singleton.IsServer && !gameStarted)
        {
            CheckAndStartGame();
        }
        
        // Если мы клиент и подключились, проверяем, готова ли уже игра
        if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            // Небольшая задержка, чтобы убедиться, что все синхронизировалось
            StartCoroutine(CheckGameReadyAfterConnection());
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        currentPlayerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
        Debug.Log($"Игрок отключился! ID: {clientId}, Всего игроков: {currentPlayerCount}");
    }

    private void OnServerStarted()
    {
        // Debug.Log("Сервер запущен!");
        currentPlayerCount = 1; // Сервер = 1 игрок
    }

    private void OnClientStarted()
    {
        // Debug.Log("Клиент запущен!");
        // Обновляем количество игроков
        if (NetworkManager.Singleton != null)
        {
            currentPlayerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
        }
    }

    private void CheckAndStartGame()
    {
        // Проверяем, что мы в лобби и достаточно игроков
        if (SceneManager.GetActiveScene().name == lobbySceneName && currentPlayerCount >= requiredPlayers && !gameStarted)
        {
            // Debug.Log($"Достаточно игроков ({currentPlayerCount}/{requiredPlayers})! Запускаем игру...");
            gameStarted = true;
            
            // Уведомляем всех клиентов о готовности игры
            if (NetworkManager.Singleton.IsServer)
            {
                GameReadyClientRpc();
            }
            
            StartGame();
        }
        else
        {
            // Debug.Log($"Недостаточно игроков для старта: {currentPlayerCount}/{requiredPlayers} или не в лобби или игра уже началась");
        }
    }

    [ClientRpc]
    private void GameReadyClientRpc()
    {
        // Уведомляем MainMenuUIManager о готовности
        if (MainMenuUIManager.Instance != null)
        {
            MainMenuUIManager.Instance.OnGameReady();
        }
    }

    private void StartGame()
    {
        // Запускаем игру на всех клиентах через Netcode SceneManager
        if (NetworkManager.Singleton.IsServer)
        {
            StartCoroutine(StartGameWithDelay());
        }
    }

    private System.Collections.IEnumerator StartGameWithDelay()
    {
        // Debug.Log($"Запускаем игру через {sceneLoadDelay} секунд...");
        yield return new WaitForSeconds(sceneLoadDelay);
        NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    
    private System.Collections.IEnumerator LoadGameSceneWithDelay()
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        // Debug.Log("Загружаем игровую сцену...");
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Устанавливает количество игроков (для отладки)
    /// </summary>
    public void SetPlayerCount(int count)
    {
        currentPlayerCount = count;
        CheckAndStartGame();
    }

    /// <summary>
    /// Возвращает текущее количество игроков
    /// </summary>
    public int GetPlayerCount()
    {
        return currentPlayerCount;
    }

    private System.Collections.IEnumerator CheckGameReadyAfterConnection()
    {
        yield return new WaitForSeconds(0.5f); // Ждем полсекунды
        
        if (currentPlayerCount >= requiredPlayers && !gameStarted)
        {
            // Уведомляем UI о готовности
            if (MainMenuUIManager.Instance != null)
            {
                MainMenuUIManager.Instance.OnGameReady();
            }
        }
    }
} 