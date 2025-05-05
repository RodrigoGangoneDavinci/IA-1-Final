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
        
        if (_npc.hp <= _npc.maxHp/2) return; //no salgo de idle
        
        // 0- Chequeo si hay enemigo en FOV
        if (_npc.GetNearestEnemyInFOV() != null)
        {
            _npc._fsm.ChangeState(NPCStates.Attack);
            return;
        }

        // 1- Me muevo buscando al leader
        float distanceToLeader = Vector3.Distance(_npc.transform.position, _npc.leaderToFollow.transform.position);
        if (distanceToLeader >= followDistance) fsm.ChangeState(NPCStates.Move);
    }

    public override void OnExit()
    {
        Debug.Log($"[NPC {_npc.name}] Sali de Idle");
    }
}
