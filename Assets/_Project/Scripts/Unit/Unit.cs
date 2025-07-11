using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitData unitData;
    [SerializeField] private Team team;

    public UnitData UnitData => unitData;
    public Team Team => team;

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
