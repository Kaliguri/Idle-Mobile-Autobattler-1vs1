using UnityEngine;

public class HP : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;

    public void Initialize(float maxHP)
    {
        maxHealth = maxHP;
        currentHealth = maxHealth;
    }

    public void ChangeHealth(float amount)
    {
        currentHealth += amount;
        
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void TakeDamage(float damage)
    {
        ChangeHealth(-damage);
    }

    public void Heal(float healAmount)
    {
        ChangeHealth(healAmount);
    }

    private void Die()
    {
        Unit unit = GetComponent<Unit>();
        
        if (BattleInfoSingleton.Instance != null)
        {
            BattleInfoSingleton.Instance.UnregisterUnit(unit);
        }
        
        Destroy(gameObject);
    }
} 