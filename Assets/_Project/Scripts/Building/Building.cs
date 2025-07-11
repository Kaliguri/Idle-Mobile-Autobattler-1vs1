using UnityEngine;

public class Building : MonoBehaviour
{
    protected BuildingData buildingData;
    protected Team team;
    protected bool isActive = true;

    public BuildingData BuildingData => buildingData;
    public Team Team => team;
    public bool IsActive => isActive;

    public void Initialize(BuildingData data, Team team)
    {
        buildingData = data;
        this.team = team;

        if (data.CanSpawnUnits)
        {
            UnitSpawner spawner = GetComponent<UnitSpawner>();
            spawner.Initialize(this);
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }
} 