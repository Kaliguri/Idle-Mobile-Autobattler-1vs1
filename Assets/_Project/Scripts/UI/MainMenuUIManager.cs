using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    private static MainMenuUIManager instance;
    public static MainMenuUIManager Instance => instance;

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject lobbyUI;
    
    [Header("Current UI")]
    [SerializeField] private GameObject currentUI;
    
    [Header("Lobby UI")]
    [SerializeField] private TMPro.TextMeshProUGUI statusText;
    [SerializeField] private Button exitButton;
    
    [SerializeField] private string readyText = "Starting the match...";
    [SerializeField] private string waitingText = "Waiting for the opponent...";
    
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
        // Инициализация UI - начинаем с главного меню
        currentUI = mainMenuUI;
        ShowMainMenu();
        
        // Подписываемся на сетевые события
        SubscribeToNetworkEvents();
    }
    
    private void OnDestroy()
    {
        // Отписываемся от событий
        UnsubscribeFromNetworkEvents();
    }
    
    private void SubscribeToNetworkEvents()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        }
    }
    
    private void UnsubscribeFromNetworkEvents()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        }
    }
    
    private void OnClientConnected(ulong clientId)
    {
        // Проверяем, подключились ли именно мы
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            ShowLobby();
        }
        // Если подключился другой игрок - остаемся в лобби
        UpdateExitButtonState();
    }
    
    private void OnClientDisconnected(ulong clientId)
    {
        // Проверяем, отключились ли именно мы
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            ShowMainMenu();
        }
        // Если отключился другой игрок - остаемся в лобби
        UpdateExitButtonState();
    }
    
    private void OnServerStarted()
    {
        // Проверяем, запустили ли сервер именно мы
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            ShowLobby();
        }
    }
    
    /// <summary>
    /// Показывает главное меню
    /// </summary>
    public void ShowMainMenu()
    {
        if (currentUI != null)
        {
            currentUI.SetActive(false);
        }
        
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(true);
            currentUI = mainMenuUI;
        }
    }
    
    /// <summary>
    /// Показывает лобби
    /// </summary>
    public void ShowLobby()
    {
        if (currentUI != null)
        {
            currentUI.SetActive(false);
        }
        
        if (lobbyUI != null)
        {
            lobbyUI.SetActive(true);
            currentUI = lobbyUI;
            UpdateStatusText(waitingText);
            UpdateExitButtonState();
        }
    }
    
    /// <summary>
    /// Вызывается когда игра готова к запуску
    /// </summary>
    public void OnGameReady()
    {
        UpdateStatusText(readyText);
    }
    
    /// <summary>
    /// Обновляет текст статуса
    /// </summary>
    private void UpdateStatusText(string text)
    {
        if (statusText != null)
        {
            statusText.text = text;
        }
    }
    
    /// <summary>
    /// Обновляет состояние кнопки Exit в зависимости от количества игроков
    /// </summary>
    private void UpdateExitButtonState()
    {
        if (exitButton != null && NetworkManager.Singleton != null)
        {
            int playerCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
            bool shouldDisableExit = playerCount >= 2; // Деактивируем когда 2 или больше игроков
            
            exitButton.gameObject.SetActive(!shouldDisableExit);
        }
    }
} 