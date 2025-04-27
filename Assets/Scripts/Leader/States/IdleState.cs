using UnityEngine;

public class IdleState : State
{
    private Leader _leader;
    
    public IdleState(Leader l)
    {
        _leader = l;
    }
    
    public override void OnEnter()
    {
        Debug.Log($"[Leader {_leader.name}] Entre a Idle");
    }

    public override void OnUpdate()
    {
        Debug.Log($"[Leader {_leader.name}] Estoy en Idle");
    }

    public override void OnExit()
    {
        Debug.Log($"[Leader {_leader.name}] Sali de Idle");
    }
}
