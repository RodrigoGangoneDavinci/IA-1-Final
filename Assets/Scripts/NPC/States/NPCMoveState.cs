using UnityEngine;

public class NPCMoveState : State
{
    private NPC _npc;
    
    //No se para que estan
    private float _arriveWeight = 1.0f;
    private float _obstacleAvoidanceWeight = 2.0f;

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
        
        // Si no tenemos líder asignado, no hacemos nada
        if (_npc.leaderToFollow == null)
        {
            fsm.ChangeState(NPCStates.Idle);
            return;
        }

        _npc.Flocking();

        /*Vector3 force = Vector3.zero;

        // 1. Llegar hacia el líder (Arrive)
        force += _npc.Arrive(_npc.TargetPosition) * _arriveWeight;

        // 2. Separación de otros NPCs aliados
        var allNPCs = GameManager.instance.GetAllNPCsFromTeam(_npc.myTeam); // necesitarás este método
        force += _npc.Separation(allNPCs) * _separationWeight;

        // 3. Evitar obstáculos
        if (_npc.HastToUseObstacleAvoidance())
        {
            force += _npc.ObstacleAvoidance() * _obstacleAvoidanceWeight;
        }

        _npc.AddForce(force);

        _npc.Move();

        // (Opcional: podríamos chequear distancia al líder y cambiar a Idle si estamos muy cerca)*/

    }

    public override void OnExit()
    {
        Debug.Log($"[NPC {_npc.name}] Sali de Move");
    }
}
