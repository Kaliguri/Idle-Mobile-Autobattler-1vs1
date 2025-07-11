using UnityEngine;

[CreateAssetMenu(fileName = "Unit Name", menuName = "Autobattler/Unit")]
public class UnitData : ScriptableObject
{
    [Header("Basic Info")]
    public string unitName;
    
    [Header("Stats")]
    public float maxHealth;
    public float damage;
    public float attackRange;
    public float movementSpeed;
    public float attackSpeed;
    
    [Header("Combat")]
    public AttackType attackType;
    
    [Header("Visual")]
    public GameObject unitVisualPrefab;
}

public enum AttackType
{
    Melee,
    Ranged
}
