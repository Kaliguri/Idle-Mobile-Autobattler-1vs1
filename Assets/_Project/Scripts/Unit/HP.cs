using Unity.Netcode;
using UnityEngine;

public class HP : NetworkBehaviour
{
    private NetworkVariable<float> maxHealth = new NetworkVariable<float>(100f);
    private NetworkVariable<float> currentHealth = new NetworkVariable<float>();

    public float MaxHealth => maxHealth.Value;
    public float CurrentHealth => currentHealth.Value;
    public bool IsDead => currentHealth.Value <= 0;

    [ServerRpc(RequireOwnership = false)]
    public void InitializeServerRpc(float maxHP)
    {
        if (IsServer)
        {
            maxHealth.Value = maxHP;
            currentHealth.Value = maxHP;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeHealthServerRpc(float amount)
    {
        if (!IsServer) return;
        
        currentHealth.Value += amount;
        
        if (currentHealth.Value > maxHealth.Value)
        {
            currentHealth.Value = maxHealth.Value;
        }
        
        if (currentHealth.Value <= 0)
        {
            currentHealth.Value = 0;
            DieClientRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        if (!IsServer) return;
        
        ChangeHealthServerRpc(-damage);
    }

    [ServerRpc(RequireOwnership = false)]
    public void HealServerRpc(float healAmount)
    {
        if (!IsServer) return;
        
        ChangeHealthServerRpc(healAmount);
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        Unit unit = GetComponent<Unit>();
        
        if (BattleInfoSingleton.Instance != null)
        {
            BattleInfoSingleton.Instance.UnregisterUnit(unit);
        }
        
        if (IsServer)
        {
            NetworkObject.Despawn(true);
        }
    }
} 
