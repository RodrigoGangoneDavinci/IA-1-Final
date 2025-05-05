using UnityEngine;

public class NPCMoveState : State
{
    private NPC _npc;

    public NPCMoveState(NPC npc)
    {
        _npc = npc;
    }

    public override void OnEnter()
    {
        Debug.Log($"[NPC {_npc.name}] Entre a Move");
    }

    public override void OnUpdate()
    {
        Debug.Log($"[NPC {_npc.name}] Estoy en Move");
        
        // 0- Chequeo si hay enemigo en FOV
        NPC target = _npc.GetNearestEnemyInFOV();
        if (target != null) _npc._fsm.ChangeState(NPCStates.Attack);

        // 1- Chequeo si no hay obstaculos
        if (!_npc.HastToUseObstacleAvoidance())
        {
            var team = GameManager.instance.GetAllNPCsFromTeam(_npc.myTeam);
            _npc.AddForce(_npc.Arrive(_npc.TargetPosition));
            _npc.AddForce(_npc.Separation(team));
        }

        _npc.Move();
    }

    public override void OnExit()
    {
        Debug.Log($"[NPC {_npc.name}] Sali de Move");
    }
}
