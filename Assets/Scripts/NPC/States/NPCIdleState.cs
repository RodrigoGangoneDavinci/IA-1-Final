using UnityEngine;

public class NPCIdleState : State
{
    private NPC _npc;

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
    }

    public override void OnExit()
    {
        Debug.Log($"[NPC {_npc.name}] Sali de Idle");
    }
}
