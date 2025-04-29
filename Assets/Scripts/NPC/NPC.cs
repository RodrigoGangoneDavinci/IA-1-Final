using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class NPC : MonoBehaviour
{
    public FiniteStateMachine _fsm; //✅

    [Header("Basics")]
    [SerializeField] public LeaderTeam myTeam; //✅
    [SerializeField] public Leader leaderToFollow; //✅
    [SerializeField] Transform safeNode; //✅
    public Vector3 SafeNodePosition => safeNode.position;
    
    [Header("Stats")] 
    public float attack = 10f; //✅
    public float maxHp = 100f; //✅
    public float hp; //✅
    [SerializeField] protected float _maxSpeed = 5f; //✅
    [SerializeField] protected float _maxForce = 5f; //✅

    [Header("Flocking Settings")] 
    [SerializeField, Range(0f, 2f)] float _alignmentWeight = 1f; //✅
    [SerializeField, Range(0f, 2f)] float _separationWeight = 1.75f; //✅
    [SerializeField, Range(0f, 2f)] float _cohesionWeight = 1f; //✅
    private Vector3 _velocity; //Direccion en la que tiene que moverse

    [Header("FOV Settings")]
    [SerializeField] float _viewRadius = 2f; //✅
    [SerializeField, Range(0, 180)] float viewAngle = 120f; //✅
    [SerializeField] LayerMask _obstacles; //✅

    [Header("Enemies")] 
    [SerializeField] List<NPC> allEnemies = new(); //✅ // se llena automáticamente desde un GameManager
    [SerializeField] public float rangeToAttack = 1f; //✅ //Rango para atacar

    public Vector3 TargetPosition => leaderToFollow != null ? leaderToFollow.transform.position : transform.position;

    private void Awake()
    {
        hp = maxHp;
        rangeToAttack = _viewRadius / 2;
    }

    void Start()
    {
        GameManager.instance.RegisterNPC(this);

        _fsm = new FiniteStateMachine();
        _fsm.AddState(NPCStates.Scape, new NPCScapeState(this)); //1- Escapar si tengo poca vida
        _fsm.AddState(NPCStates.Idle, new NPCIdleState(this)); //2-Verifico si estoy lejos del "nanana Lider"
        _fsm.AddState(NPCStates.Move, new NPCMoveState(this)); //3- Aplico (floking [arrive + separation])
        _fsm.AddState(NPCStates.Attack, new NPCAttackState(this)); // 4-Atacar a NPCs contrarios si estan en field o view
        _fsm.AddState(NPCStates.Dead, new NPCDeadState(this)); //Muerte si mi vida llega a 0

        _fsm.ChangeState(NPCStates.Idle);
    }

    void Update()
    {
        _fsm.Update();

        //Prioridades
        // 1- Escapar si tengo poca vida
        //if (hp <= maxHp / 2) _fsm.ChangeState(NPCStates.Scape);
        
        //TODO: DEBUG -> Daño a todos los NPCs sin importar el LEADER TEAM
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(25f);
        }
    }

    public void Flocking()
    {
        //Obtengo todos los NPCs de mi equipo
        var allNPCs = GameManager.instance.GetAllNPCsFromTeam(myTeam);

        AddForce(Alignment(allNPCs) * _alignmentWeight);
        AddForce(Separation(allNPCs) * _separationWeight);
        AddForce(Cohesion(allNPCs) * _cohesionWeight);
    }

    //FOV
    public bool IsInFieldOfView(Vector3 position)
    {
        Vector3 dir = position - transform.position;
        if (dir.magnitude > _viewRadius) return false;
        if (Vector3.Angle(transform.forward, dir) > viewAngle / 2f) return false;
        if (!HasLineOfSight(transform.position, position)) return false;
        return true;
    }

    //Line Of Sight
    public bool HasLineOfSight(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;
        return !Physics.Raycast(start, dir.normalized, dir.magnitude, _obstacles);
    }
    
    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
            _fsm.ChangeState(NPCStates.Dead);
        else if (hp <= maxHp / 2)
            _fsm.ChangeState(NPCStates.Scape);
    }
    
    //Creo un angulo
    Vector3 GetAngleFromDir(float angleInDegrees)
    {
        return new Vector3(
            Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),
            0,
            Mathf.Cos(angleInDegrees * Mathf.Deg2Rad)
        );
    }

    //Metodo para visualizar el angulo creado
    private void OnDrawGizmosSelected()
    {
        #region ShowAngle

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);

        Vector3 DirA = GetAngleFromDir(viewAngle / 2 + transform.eulerAngles.y);
        Vector3 DirB = GetAngleFromDir(-viewAngle / 2 + transform.eulerAngles.y);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + DirA.normalized * _viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + DirB.normalized * _viewRadius);

        #endregion

        #region ShowObstacleAvoidance

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);

        Gizmos.color = Color.green;

        Vector3 leftRayPos = transform.position + transform.right * 0.5f;
        Vector3 rightRayPos = transform.position - transform.right * 0.5f;

        Gizmos.DrawLine(leftRayPos, leftRayPos + transform.forward * _viewRadius);
        Gizmos.DrawLine(rightRayPos, rightRayPos + transform.forward * _viewRadius);

        #endregion
    }

    #region SteeringAgent Methods

    public void Move()
    {
        transform.position += _velocity * Time.deltaTime;
        if (_velocity != Vector3.zero) transform.forward = _velocity;
    }

    //Chequea si hay obstáculos usando ObstacleAvoidance()
    public bool HastToUseObstacleAvoidance()
    {
        Vector3 avoidanceObs = ObstacleAvoidance();
        AddForce(avoidanceObs);
        return avoidanceObs != Vector3.zero;
    }

    //Calcula una fuerza de steering para moverse directamente hacia una posición objetivo (targetPos) usando la velocidad máxima (_maxSpeed).
    public Vector3 Seek(Vector3 targetPos)
    {
        return Seek(targetPos, _maxSpeed);
    }

    //Lo mismo que el anterior per permite poner velocidad deseada Y limita la fuerza para no excederse
    public Vector3 Seek(Vector3 targetPos, float speed)
    {
        Vector3 desired = (targetPos - transform.position).normalized * speed;

        Vector3 steering = desired - _velocity;

        steering = Vector3.ClampMagnitude(steering, _maxForce * Time.deltaTime);

        return steering;
    }

    public Vector3 Flee(Vector3 targetPos) => -Seek(targetPos);

    //Variante de Seek donde el agente frena gradualmente al acercarse al objetivo
    //Si está lejos, se mueve a máxima velocidad.
    //Si está cerca (dentro de viewRadius), la velocidad disminuye proporcionalmente a la distancia.
    public Vector3 Arrive(Vector3 targetPos)
    {
        float dist = Vector3.Distance(transform.position, targetPos);
        if (dist > _viewRadius) return Seek(targetPos);

        return Seek(targetPos, _maxSpeed * (dist / _viewRadius));
    }

    /*
     Detecta obstáculos usando dos raycasts (uno hacia arriba y otro hacia abajo desde el centro).

    Si detecta algo, genera una fuerza para desviarse hacia arriba o hacia abajo.

    Si no detecta obstáculos, retorna Vector3.zero (sin fuerza extra
     */
    public Vector3 ObstacleAvoidance()
    {
        if (Physics.Raycast(
                transform.position + transform.right * 0.5f,
                transform.forward,
                _viewRadius,
                _obstacles))
            return Seek(transform.position - transform.right);
        
        if (Physics.Raycast(
                transform.position - transform.right * 0.5f,
                transform.forward,
                _viewRadius,
                _obstacles))
            return Seek(transform.position + transform.right);
        
        return Vector3.zero;
    }

    /*
     Persigue a otro agente (targetAgent)
     prediciendo su próxima posición (asume que el otro sigue moviéndose con su velocity actual
     */
    //TODO: ver si tiene que perseguir al LEADER en vez de a otros NPCs
    public Vector3 Pursuit(NPC targetAgent)
    {
        Vector3 futurePos = targetAgent.transform.position + targetAgent._velocity;
        Debug.DrawLine(transform.position, futurePos, Color.cyan);
        return Seek(futurePos);
    }

    /*
     * Hace lo opuesto de Pursuit: huye de la posición futura del objetivo.
     */
    //TODO: creo que esta bien que deba evadir a NPCs enemigos, solo cuando esta escapando
    public Vector3 Evade(NPC targetAgent)
    {
        return -Pursuit(targetAgent);
    }

    /*
     * Resetea la posición del agente a (0,0,0).
     */
    public void ResetPosition()
    {
        transform.position = Vector3.zero;
    }

    /*
     * Calcula una fuerza para alinearse con la velocidad promedio de los otros agentes cercanos (dentro de viewRadius).
        El agente trata de apuntar y moverse en la misma dirección que sus vecinos.
     */
    public Vector3 Alignment(List<NPC> agents)
    {
        Vector3 desired = Vector3.zero;
        int boidsCount = 0;

        foreach (var item in agents)
        {
            if (Vector3.Distance(item.transform.position, transform.position) > _viewRadius) continue;

            desired += item._velocity;
            boidsCount++;
        }

        desired /= boidsCount;

        return CalculateSteering(desired.normalized * _maxSpeed);
    }

    /*
     Calcula una fuerza para separarse de los otros agentes que estén demasiado cerca (dentro de viewRadius).
    Si está demasiado cerca de alguien, empuja en la dirección opuesta.
    */
    public Vector3 Separation(List<NPC> agents)
    {
        Vector3 desired = Vector3.zero;

        foreach (var item in agents)
        {
            if (item == this) continue; //Ignorar mi propio calculo

            Vector3 dist = item.transform.position - transform.position;

            if (dist.sqrMagnitude > _viewRadius * _viewRadius) continue;

            desired += dist;
        }

        if (desired == Vector3.zero) return Vector3.zero;
        desired *= -1;
        return CalculateSteering(desired.normalized * _maxSpeed);
    }

    /*
     Calcula una fuerza para acercarse al centro de los vecinos (cohesión).
    Es como una atracción para mantener el grupo junto, sin dispersarse.
     */
    public Vector3 Cohesion(List<NPC> agents)
    {
        Vector3 desired = Vector3.zero;
        int boidsCount = 0;

        foreach (var item in agents)
        {
            if (item == this) continue; //Ignorar mi propio calculo

            Vector3 dist = item.transform.position - transform.position;

            if (dist.sqrMagnitude > _viewRadius * _viewRadius) continue;

            //Promedio = Suma / Cantidad
            desired += item.transform.position;
            boidsCount++;
        }

        if (boidsCount == 0) return Vector3.zero; //Si no hay agentes

        desired /= boidsCount;

        return Seek(desired);
    }


/*
 * Dado un vector deseado de movimiento, calcula el steering force respetando el límite de fuerza máxima (_maxForce).
 */
    public Vector3 CalculateSteering(Vector3 desired)
    {
        return Vector3.ClampMagnitude(desired - _velocity, _maxForce * Time.deltaTime);
    }

    /*
     * Suma una fuerza a la velocidad actual,
     * limitando la nueva velocidad para que no supere la velocidad máxima (_maxSpeed).
     */
    public void AddForce(Vector3 force)
    {
        _velocity = Vector3.ClampMagnitude(_velocity + force, _maxSpeed);
    }

    #endregion
}

public enum NPCStates
{
    Idle,
    Move,
    Attack,
    Scape,
    Dead
}

public enum LeaderTeam
{
    LeaderOne,
    LeaderTwo
}