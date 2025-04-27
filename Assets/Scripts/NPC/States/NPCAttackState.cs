using UnityEngine;

public class NPCAttackState : State
{
    private NPC _npc;

    public NPCAttackState(NPC npc)
    {
        _npc = npc;
    }

    public override void OnEnter()
    {
        Debug.Log($"[NPC {_npc.name}] Entre a Attack");
    }

    public override void OnUpdate()
    {
        Debug.Log($"[NPC {_npc.name}] Estoy en Attack");
    }

    public override void OnExit()
    {
        Debug.Log($"[NPC {_npc.name}] Sali de Attack");
    }
}