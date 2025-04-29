using UnityEngine;

public class NPCIdleState : State
{
    private NPC _npc;
    private float followDistance = 5f;

    public NPCIdleState(NPC npc)
    {
        _npc = npc;
    }

    public override void OnEnter()
    {
        Debug.Log($"[NPC {_npc.name}] Entre a Idle");
    }

    public override void OnUpdate()
    {
        Debug.Log($"[NPC {_npc.name}] Estoy en Idle");
        
        if (_npc.leaderToFollow == null) return;
        if (_npc.hp <= _npc.maxHp/2) return; //no salgo de idle

        float distanceToLeader = Vector3.Distance(_npc.transform.position, _npc.leaderToFollow.transform.position);
        
        if (distanceToLeader >= followDistance) fsm.ChangeState(NPCStates.Move);
    }

    public override void OnExit()
    {
        Debug.Log($"[NPC {_npc.name}] Sali de Idle");
    }
}
