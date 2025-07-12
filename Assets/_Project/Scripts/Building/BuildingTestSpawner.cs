using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class BuildingTestSpawner : NetworkBehaviour
{
    [Header("Building Data")]
    [SerializeField] private BuildingData buildingData1;
    [SerializeField] private BuildingData buildingData2;

    private Camera mainCamera;
    private const int MAX_BUILDINGS = 2;
    
    // Сервер отслеживает количество зданий для каждого игрока
    private Dictionary<ulong, int> playerBuildingCount = new Dictionary<ulong, int>();

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Проверяем клики мыши (для ПК) и касания (для мобильных)
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        // Получаем позицию клика
        Vector3 clickPosition = GetClickWorldPosition();
        
        // Отправляем запрос на сервер (сервер проверит лимит)
        RequestSpawnBuildingServerRpc(clickPosition, NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnBuildingServerRpc(Vector3 position, ulong clientId)
    {
        if (!IsServer) return;

        // Проверяем лимит зданий для этого игрока
        if (!playerBuildingCount.ContainsKey(clientId))
        {
            playerBuildingCount[clientId] = 0;
        }

        if (playerBuildingCount[clientId] >= MAX_BUILDINGS)
        {
            // Debug.Log($"Сервер: Игрок {clientId} достиг лимита зданий: {playerBuildingCount[clientId]}/{MAX_BUILDINGS}");
            NotifyPlayerLimitReachedClientRpc(clientId);
            return;
        }

        // Определяем команду по ClientId
        Team playerTeam = clientId == 0 ? Team.Player1 : Team.Player2;
        
        // Определяем какое здание спавнить
        BuildingData buildingData = playerBuildingCount[clientId] == 0 ? buildingData1 : buildingData2;
        
        if (buildingData == null)
        {
            Debug.LogError("BuildingData is null!");
            return;
        }

        // Увеличиваем счетчик для игрока
        playerBuildingCount[clientId]++;

        // Debug.Log($"Сервер: Создаем здание {buildingData.BuildingName} для клиента {clientId} (команда {playerTeam}) на позиции {position}. Здание {playerBuildingCount[clientId]}/{MAX_BUILDINGS}");
        BuildingFactory.CreateBuilding(buildingData, position, playerTeam);
    }

    [ClientRpc]
    private void NotifyPlayerLimitReachedClientRpc(ulong targetClientId)
    {
        // Уведомляем только конкретного игрока
        if (NetworkManager.Singleton.LocalClientId == targetClientId)
        {
            // Debug.Log($"Достигнут лимит зданий: {MAX_BUILDINGS}/{MAX_BUILDINGS}");
        }
    }

    private Vector3 GetClickWorldPosition()
    {
        Vector3 screenPosition;
        
        // Определяем позицию в зависимости от устройства
        if (Input.touchCount > 0)
        {
            // Мобильное устройство - используем касание
            screenPosition = Input.GetTouch(0).position;
        }
        else
        {
            // ПК - используем мышь
            screenPosition = Input.mousePosition;
        }
        
        screenPosition.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(screenPosition);
    }
} 