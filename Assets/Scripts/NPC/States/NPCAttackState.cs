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
        _target = _npc.GetNearestEnemyInFOV();
    }

    public override void OnUpdate()
    {
        Debug.Log($"[NPC {_npc.name}] Estoy en Attack");

        if (_npc == null || _target == null)
            _npc._fsm.ChangeState(NPCStates.Move); // No hay enemigos a la vista


        if (_npc.IsInFieldOfView(_target.transform.position))
        {
            float distance = Vector3.Distance(_npc.transform.position, _target.transform.position);

            if (distance <= _npc.rangeToAttack)
            {
                //Atacar
                _target.TakeDamage(_npc.attack);
            }
            else
            {
                //Moverse hacia el enemigo
                _npc.AddForce(_npc.Arrive(_target.transform.position));
                _npc.Move();
            }
        }
        else
        {
            // Buscar otro objetivo visible
            var newTarget = _npc.GetNearestEnemyInFOV();

            if (newTarget != null)
                _target = newTarget; // Nuevo objetivo
            else
                _npc._fsm.ChangeState(NPCStates.Move); // No hay enemigos a la vista
        }
    }

    public override void OnExit()
    {
        Debug.Log($"[NPC {_npc.name}] Sali de Attack");
    }
}