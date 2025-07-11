using UnityEngine;

public class WinPoint : MonoBehaviour
{
    [SerializeField] private Team team;

    public Team Team => team;

    private void Awake()
    {
        // BattleInfoSingleton сам найдет и зарегистрирует этот WinPoint
    }

    public void Initialize(Team winPointTeam)
    {
        team = winPointTeam;
    }
} 