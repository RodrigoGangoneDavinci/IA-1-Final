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
    }

    public override void OnExit()
    {
        Debug.Log($"[NPC {_npc.name}] Sali de Move");
    }
}
