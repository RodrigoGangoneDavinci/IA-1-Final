using UnityEngine;

public class NPCScapeState : State
{
    private NPC _npc;

    public NPCScapeState(NPC npc)
    {
        _npc = npc;
    }

    public override void OnEnter()
    {
        Debug.Log($"[NPC {_npc.name}] Entre a Scape");
    }

    public override void OnUpdate()
    {
        Debug.Log($"[NPC {_npc.name}] Estoy en Scape");
    }

    public override void OnExit()
    {
        Debug.Log($"[NPC {_npc.name}] Sali de Scape");
    }
}
