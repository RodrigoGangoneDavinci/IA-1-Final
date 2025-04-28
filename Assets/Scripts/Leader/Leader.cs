using UnityEngine;

public class Leader : MonoBehaviour
{
    protected FiniteStateMachine _fsm;
    public float hp;
    public float maxHp = 100f;
    public float speed = 5f;
    public float rotation = 10f;
    
    [SerializeField] Transform safeNode;
    public Vector3 SafeNodePosition => safeNode.position;
    
    public Vector3 targetPosition;

    void Start()
    {
        _fsm = new FiniteStateMachine(); 
        _fsm.AddState(LeaderStates.Idle, new IdleState(this)); //Si el usuario no clickeo nada o el Leader llego a destino 
        _fsm.AddState(LeaderStates.Move, new MoveState(this)); //Mover Por LineOfSight y si choca con pared usar Theta*
        _fsm.AddState(LeaderStates.Scape, new ScapeState(this)); //Si tengo poca vida entrar en state Scape
        _fsm.AddState(LeaderStates.Dead, new DeadState(this)); //Si pierde toda la vida

        _fsm.ChangeState(LeaderStates.Idle);
    }

    protected void Update()
    {
        _fsm.Update();
    }

    protected void MoveTo(Vector3 position)
    {
        targetPosition = position;
        _fsm.ChangeState(LeaderStates.Move);
    }
    
    protected void TrySetDestination()
    {
        if (Physics.Raycast(Camera.main!.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            Vector3 clickPosition = hit.point;
            Vector3 targetNode = NodeGrid.GetClosestNode(clickPosition).Position;
            MoveTo(targetNode);
        }
    }
    
    public bool HasLineOfSight(Vector3 target)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir = (target - origin).normalized;
        float dist = Vector3.Distance(origin, target);
        
        return !Physics.Raycast(origin, dir, dist, LayerMask.GetMask("Wall"));
    }
    
    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
            _fsm.ChangeState(LeaderStates.Dead);
        else if (hp <= maxHp / 2)
            _fsm.ChangeState(LeaderStates.Scape);
    }
}

public enum LeaderStates
{
    Idle,
    Move,
    Scape,
    Dead
}