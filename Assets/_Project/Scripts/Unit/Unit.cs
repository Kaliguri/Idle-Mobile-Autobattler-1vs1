using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitData unitData;
    [SerializeField] private Team team;
    private bool isActive = true;

    public UnitData UnitData => unitData;
    public Team Team => team;
    public bool IsActive => isActive;

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
        
        UnitAI unitAI = GetComponent<UnitAI>();
        unitAI.Initialize(this);
        
        // Регистрируем юнит в BattleInfo при инициализации
        if (BattleInfoSingleton.Instance != null)
        {
            BattleInfoSingleton.Instance.RegisterUnit(this);
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    private void OnDestroy()
    {
        // Удаляем юнит из BattleInfo при уничтожении
        if (BattleInfoSingleton.Instance != null)
        {
            BattleInfoSingleton.Instance.UnregisterUnit(this);
        }
    }
}
