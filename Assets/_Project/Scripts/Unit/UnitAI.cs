using Unity.Netcode;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    private Unit unit;
    private bool canAttack = true;

    public void Initialize(Unit unit)
    {
        this.unit = unit;
    }

    private void FixedUpdate()
    {
        // Работаем только если компонент включен и юнит инициализирован
        if (enabled && unit != null && unit.IsActive)
        {
            ProcessAI();
        }
    }

    private void ProcessAI()
    {
        if (unit == null) return;
        
        // ИИ юнитов работает только на сервере
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) return;
        
        // Проверяем, что BattleInfoSingleton существует
        if (BattleInfoSingleton.Instance == null) return;
        
        Unit nearestEnemy = BattleInfoSingleton.Instance.FindNearestEnemy(transform.position, unit.Team);
        bool isEnemyInRange = nearestEnemy != null && Vector3.Distance(transform.position, nearestEnemy.transform.position) <= unit.UnitData.AttackRange;
        
        if (isEnemyInRange)
        {
            if (canAttack) Attack(nearestEnemy);
        }
        else
        {
            Movement();
        }
    }

    private void Movement()
    {
        Vector3 targetPosition = GetTargetPosition();
        Vector3 direction = (targetPosition - transform.position).normalized;
        
        transform.position += direction * unit.UnitData.MovementSpeed * Time.fixedDeltaTime;
    }

    private Vector3 GetTargetPosition()
    {
        Unit nearestEnemy = BattleInfoSingleton.Instance.FindNearestEnemy(transform.position, unit.Team);
        
        if (nearestEnemy != null)
        {
            Debug.Log($"[{unit.name}] Найден ближайший враг: {nearestEnemy.name} на позиции {nearestEnemy.transform.position}");
            return nearestEnemy.transform.position;
        }
        else
        {
            Debug.Log($"[{unit.name}] Врагов не найдено, ищем WinPoint для команды {unit.Team}");
            WinPoint winPoint = BattleInfoSingleton.Instance.GetWinPointForTeam(unit.Team);
            if (winPoint != null)
            {
                Debug.Log($"[{unit.name}] Найден WinPoint: {winPoint.name} на позиции {winPoint.transform.position}");
                return winPoint.transform.position;
            }
            else
            {
                Debug.LogWarning($"[{unit.name}] WinPoint для команды {unit.Team} не найден!");
            }
        }
        
        Debug.Log($"[{unit.name}] Возвращаем текущую позицию: {transform.position}");
        return transform.position;
    }

    private void Attack(Unit target)
    {
        HP targetHP = target.GetComponent<HP>();
        if (targetHP != null)
        {
            // Вызываем атаку через ServerRpc
            targetHP.TakeDamageServerRpc(unit.UnitData.Damage);
        }
        
        canAttack = false;
        float attackCooldown = 1f / unit.UnitData.AttackSpeed;
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void ResetAttack()
    {
        canAttack = true;
    }
} 