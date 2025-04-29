using UnityEngine;

public class NPCAttackState : State
{
    private NPC _npc;
    private NPC _target;

    public NPCAttackState(NPC npc)
    {
        _npc = npc;
    }

    public override void OnEnter()
    {
        Debug.Log($"[NPC {_npc.name}] Entre a Attack");
        _target = FindNearestEnemyInFOV();
    }

    public override void OnUpdate()
    {
        Debug.Log($"[NPC {_npc.name}] Estoy en Attack");

        if (_npc == null || _target == null) return;

        if (_npc.IsInFieldOfView(_target.transform.position))
        {
            float distance = Vector3.Distance(_npc.transform.position, _target.transform.position);

            if (distance <= _npc.rangeToAttack)
            {
                // Aquí atacarías (restar vida, animaciones, etc.)
                _target.TakeDamage(_npc.attack);
            }
            else
            {
                // Si no está cerca todavía, moverse hacia el enemigo
                _npc.AddForce(_npc.Arrive(_target.transform.position));
                _npc.Move();
            }
        }
        else
        {
            // Si perdió de vista al enemigo, volver a moverse
            _npc._fsm.ChangeState(NPCStates.Move);
        }
    }

    public override void OnExit()
    {
        Debug.Log($"[NPC {_npc.name}] Sali de Attack");
    }

    private NPC FindNearestEnemyInFOV()
    {
        float minDistance = Mathf.Infinity;
        NPC closestEnemy = null;

        foreach (var enemy in GameManager.instance.GetAllNPCsFromCounterTeam(_npc.myTeam))
        {
            if (enemy == null) continue;
            if (!_npc.IsInFieldOfView(enemy.transform.position)) continue;

            float dist = Vector3.Distance(_npc.transform.position, enemy.transform.position);

            if (dist < minDistance)
            {
                minDistance = dist;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
}