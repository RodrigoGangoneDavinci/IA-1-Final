using System.Collections.Generic;
using UnityEngine;

public class NPCScapeState : State
{
    private NPC _npc;
    
    Vector3 _destination;
    private List<Vector3> _path;
    private int _currentWaypointIndex;
    
    private const float _waypointTolerance = 0.2f;

    public NPCScapeState(NPC npc)
    {
        _npc = npc;
    }

    public override void OnEnter()
    {
        _path = new();
        _currentWaypointIndex = 0;

        // Verificar si tiene línea directa al nodo seguro
        if (_npc.HasLineOfSight(_npc.transform.position, _npc.SafeNodePosition))
        {
            Debug.Log($"[NPC {_npc.name}] Entre a Scape -> HasLineOfSight");
            _path.Add(_npc.SafeNodePosition); //  ir directo
        }
        else
        {
            Debug.Log($"[NPC {_npc.name}] Entre a Scape -> Theta*");
            _path = ThetaStarPathfinding.GetPath(_npc.transform.position, _npc.SafeNodePosition);
        }
    }

    public override void OnUpdate()
    {
        Debug.Log($"[NPC {_npc.name}] Estoy en Scape");
        
        if (_npc == null || _path == null || _path.Count == 0) return;
        
        // Evitar obstaculos en tiempo real
        if (_npc.HastToUseObstacleAvoidance())
        {
            _npc.Move();
            return;
        }

        Vector3 target = _path[_currentWaypointIndex];
        _npc.AddForce(_npc.Arrive(target));
        _npc.Move();

        // Si llegó al punto actual, avanzar al siguiente
        if (Vector3.Distance(_npc.transform.position, target) < _waypointTolerance)
        {
            _currentWaypointIndex++;

            if (_currentWaypointIndex >= _path.Count)
            {
                // Llegó al destino
                _npc._fsm.ChangeState(NPCStates.Idle);
            }
        }
    }

    public override void OnExit()
    {
        Debug.Log($"[NPC {_npc.name}] Sali de Scape");
        _path.Clear();
    }
}
