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
    }

    public override void OnUpdate()
    {
    }

    public override void OnExit()
    {
    }
}
