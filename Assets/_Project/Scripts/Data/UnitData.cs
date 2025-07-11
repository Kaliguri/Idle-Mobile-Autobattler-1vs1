using UnityEngine;

[CreateAssetMenu(fileName = "Unit Name", menuName = "Autobattler/Unit")]
public class UnitData : ScriptableObject
{
    [Header("Basic Info")]
    [SerializeField] private string unitName;
    
    [Header("Stats")]
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float damage = 10;
    [SerializeField] private float attackRange = 10;
    [SerializeField] private float movementSpeed = 10;
    [SerializeField] private float attackSpeed = 1;
    
    [Header("Combat")]
    [SerializeField] private AttackType attackType;
    
    [Header("Visual")]
    [SerializeField] private GameObject unitVisualPrefab;

    // Public properties for accessing the data
    public string UnitName => unitName;

    public float MaxHealth => maxHealth;
    public float Damage => damage;
    public float AttackRange => attackRange;
    public float MovementSpeed => movementSpeed;
    public float AttackSpeed => attackSpeed;

    public AttackType AttackType => attackType;
    public GameObject UnitVisualPrefab => unitVisualPrefab;
}

public enum AttackType
{
    Melee,
    Ranged
}
