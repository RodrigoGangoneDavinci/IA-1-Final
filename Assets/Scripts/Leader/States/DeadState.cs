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
    }

    public override void OnUpdate()
    {
    }

    public override void OnExit()
    {
    }
}
