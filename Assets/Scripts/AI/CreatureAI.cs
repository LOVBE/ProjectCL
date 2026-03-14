using UnityEngine;
using UnityEngine.AI;

public enum CreatureState
{
    Wander,
    Curious,
    Alert,
    Flee
}

public abstract class CreatureAI : MonoBehaviour
{
    [Header("Trạng Thái")]
    public CreatureState CurrentState;

    protected NavMeshAgent agent;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Các hàm này sẽ implement chi tiết ở Phase 3
    public virtual void ChangeState(CreatureState newState)
    {
        CurrentState = newState;
    }
}
