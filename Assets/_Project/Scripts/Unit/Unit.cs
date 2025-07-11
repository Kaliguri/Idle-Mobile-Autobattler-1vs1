using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitData unitData;
    public Team team;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(UnitData data, Team unitTeam)
    {
        unitData = data;
        team = unitTeam;
    }
}
