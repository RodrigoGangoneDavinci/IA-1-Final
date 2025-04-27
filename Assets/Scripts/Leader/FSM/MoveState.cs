using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    private Leader _leader;
    
    private Vector3 _destination;
    private List<Vector3> _path = new();
    private int _currentPathIndex;

    public MoveState(Leader leader)
    {
        _leader = leader;
    }

    public override void OnEnter()
    {
        _destination = _leader.targetPosition;

        if (HasLineOfSight(_destination))
        {
            Debug.Log($"[Leader {_leader.name}] Usando LINE OF SIGHT hacia {_destination}");
            _path.Clear();
            _path.Add(_destination);
            _currentPathIndex = 0;
        }
        else
        {
            Debug.Log($"[Leader {_leader.name}] Usando THETA* hacia {_destination}");
            _path = ThetaStarPathfinding.GetPath(_leader.transform.position, _destination);
            _currentPathIndex = 0;
        }
    }

    public override void OnUpdate()
    {
        if (_path == null || _path.Count == 0) return;

        Vector3 target = _path[_currentPathIndex];
        Vector3 direction = (target - _leader.transform.position);
        Vector3 flatDir = new Vector3(direction.x, 0, direction.z);
        float distance = flatDir.magnitude;

        if (distance < 0.1f)
        {
            _currentPathIndex++;
            if (_currentPathIndex >= _path.Count)
            {
                fsm.ChangeState(LeaderStates.Idle);
            }
        }
        else
        {
            // Movimiento
            Vector3 moveDir = flatDir.normalized;
            _leader.transform.position += moveDir * (_leader.speed * Time.deltaTime);

            // Rotación suave hacia el objetivo
            if (moveDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                _leader.transform.rotation = Quaternion.Slerp(_leader.transform.rotation, targetRot, _leader.rotation * Time.deltaTime);
            }
        }
    }

    public override void OnExit()
    {
        _path.Clear();
    }

    //TODO: hacer este metodo parte de LEADER para usar en MoveState y en ScapeState
    bool HasLineOfSight(Vector3 target)
    {
        Vector3 origin = _leader.transform.position + Vector3.up * 0.5f;
        Vector3 dir = (target - origin).normalized;
        float dist = Vector3.Distance(origin, target);
        
        return !Physics.Raycast(origin, dir, dist, LayerMask.GetMask("Wall"));
    }
}