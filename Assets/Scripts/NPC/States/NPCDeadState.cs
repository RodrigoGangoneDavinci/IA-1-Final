using UnityEngine;

public class NPCDeadState : State
{
    private NPC _npc;

    public NPCDeadState(NPC npc)
    {
        _npc = npc;
    }

    public override void OnEnter()
    {
        Debug.Log($"[NPC {_npc.name}] Entre a Dead");
        GameManager.instance.UnregisterNPC(_npc);
        GameObject.Destroy(_npc.gameObject);
    }

    public override void OnUpdate()
    {
        Debug.Log($"[NPC {_npc.name}] Estoy en Dead");
    }

    public override void OnExit()
    {
        Debug.Log($"[NPC {_npc.name}] Sali de Dead");
    }
}