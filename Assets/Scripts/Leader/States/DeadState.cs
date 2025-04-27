using UnityEngine;

public class DeadState : State
{
    private Leader _leader;

    public DeadState(Leader l)
    {
        _leader = l;
    }
    
    public override void OnEnter()
    {
        Debug.Log($"[Leader {_leader.name}] Entre a Dead");
    }

    public override void OnUpdate()
    {
        //Si pierde toda la vida
        Debug.Log($"[Leader {_leader.name}] Estoy en Dead");
    }

    public override void OnExit()
    {
        Debug.Log($"[Leader {_leader.name}] Sali de Dead");
    }
}
