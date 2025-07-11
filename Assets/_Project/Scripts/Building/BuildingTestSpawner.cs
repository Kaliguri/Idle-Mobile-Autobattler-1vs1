using UnityEngine;

public class BuildingTestSpawner : MonoBehaviour
{
    [Header("Building Data")]
    [SerializeField] private BuildingData buildingData1;
    [SerializeField] private BuildingData buildingData2;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnBuilding(buildingData1, Team.Player1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnBuilding(buildingData2, Team.Player1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SpawnBuilding(buildingData1, Team.Player2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SpawnBuilding(buildingData2, Team.Player2);
        }
    }

    private void SpawnBuilding(BuildingData buildingData, Team team)
    {
        if (buildingData == null)
        {
            Debug.LogError("BuildingData is null!");
            return;
        }

        Debug.Log($"Должно заспавниться здание - {buildingData.BuildingName} для команды {team}");
        Vector3 spawnPosition = GetMouseWorldPosition();
        BuildingFactory.CreateBuilding(buildingData, spawnPosition, team);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
} 