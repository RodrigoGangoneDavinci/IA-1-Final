using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private List<NPC> allNPCs = new();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void RegisterNPC(NPC npc)
    {
        if (!allNPCs.Contains(npc))
            allNPCs.Add(npc);
    }

    public void UnregisterNPC(NPC npc)
    {
        if (allNPCs.Contains(npc))
            allNPCs.Remove(npc);
    }

    public List<NPC> GetAllNPCsFromTeam(LeaderTeam team)
    {
        List<NPC> teamList = new();

        foreach (var npc in allNPCs)
        {
            if (npc.myTeam == team)
            {
                teamList.Add(npc);
            }
        }

        return teamList;
    }
}