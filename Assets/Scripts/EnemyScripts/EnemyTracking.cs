using UnityEngine;
using UnityEngine.AI;

public class EnemyTracking : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Chase
    }

    private enum AnimState
    {
        Idle,
        Walk,
        Run
    }

    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public Transform eyes;

    [Header("Animation")]
    public Animator animator;

    [Header("Patrol nastavení")]
    public float patrolRadius = 10f;
    public float patrolPointTolerance = 0.5f;
    public float idleTimeAtPoint = 1f;

    [Header("Movement speeds")]
    public float patrolSpeed = 3.5f;
    public float chaseSpeed = 6f;

    [Header("Cone view (zorný kužel)")]
    public float viewRadius = 12f;
    [Range(0, 360)]
    public float viewAngle = 60f;
    public float loseSightTime = 3f;

    [Tooltip("Jak daleko minimálně má být nový patrol bod od aktuální pozice enemáka.")]
    public float minPatrolDistance = 8f;

    [Header("Debug")]
    public bool debugLogState = false;

    private State currentState = State.Patrol;
    private AnimState currentAnimState = AnimState.Idle;

    private Vector3 patrolOrigin;
    private float waitTimer = 0f;
    private float lastTimeSawPlayer = Mathf.NegativeInfinity;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (eyes == null)
            eyes = transform;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        patrolOrigin = transform.position;

        if (patrolSpeed > 0f)
            agent.speed = patrolSpeed;

        SetNewPatrolDestination();

        SetAnimState(AnimState.Walk);
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                PatrolUpdate();
                break;
            case State.Chase:
                ChaseUpdate();
                break;
        }

        UpdateAnimationFromMovement();

        if (debugLogState)
            Debug.Log($"State: {currentState}, CanSeePlayer: {CanSeePlayer()}");
    }

    private void PatrolUpdate()
    {
        if (patrolSpeed > 0f && agent.speed != patrolSpeed)
            agent.speed = patrolSpeed;

        if (CanSeePlayer())
        {
            lastTimeSawPlayer = Time.time;
            currentState = State.Chase;
            agent.ResetPath();
            return;
        }

        bool atPatrolPoint = !agent.pathPending && agent.remainingDistance <= patrolPointTolerance;

        if (atPatrolPoint)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= idleTimeAtPoint)
            {
                SetNewPatrolDestination();
                waitTimer = 0f;
            }
        }
        else
        {
            if (!agent.hasPath)
                SetNewPatrolDestination();
        }
    }

    private void ChaseUpdate()
    {
        if (player == null)
            return;

        if (chaseSpeed > 0f && agent.speed != chaseSpeed)
            agent.speed = chaseSpeed;

        agent.SetDestination(player.position);

        if (CanSeePlayer())
        {
            lastTimeSawPlayer = Time.time;
        }

        if (Time.time - lastTimeSawPlayer > loseSightTime)
        {
            currentState = State.Patrol;
            waitTimer = 0f;
            agent.ResetPath();
            SetNewPatrolDestination();
        }
    }

private void SetNewPatrolDestination()
{
    for (int i = 0; i < 30; i++)
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += patrolOrigin;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            float sqrDistFromCurrent = (hit.position - transform.position).sqrMagnitude;
            if (sqrDistFromCurrent < minPatrolDistance * minPatrolDistance)
                continue;

            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(hit.position, path);

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetDestination(hit.position);
                return;
            }
        }
    }

}


    private void UpdateAnimationFromMovement()
    {
        if (animator == null || agent == null)
            return;

        float speed = agent.velocity.magnitude;

        if (speed < 0.1f)
        {
            SetAnimState(AnimState.Idle);
        }
        else
        {
            if (currentState == State.Chase)
                SetAnimState(AnimState.Run);
            else
                SetAnimState(AnimState.Walk);
        }
    }

    private void SetAnimState(AnimState newState)
    {
        if (animator == null || currentAnimState == newState)
            return;

        currentAnimState = newState;

        animator.SetBool("Idle", newState == AnimState.Idle);
        animator.SetBool("Walk", newState == AnimState.Walk);
        animator.SetBool("Run",  newState == AnimState.Run);
    }

    private bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector3 toPlayer = player.position - eyes.position;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer > viewRadius)
            return false;

        Vector3 dirToPlayer = toPlayer.normalized;
        float angleToPlayer = Vector3.Angle(eyes.forward, dirToPlayer);
        if (angleToPlayer > viewAngle * 0.5f)
            return false;

        if (Physics.Raycast(eyes.position, dirToPlayer, out RaycastHit hit, viewRadius))
        {
            Transform root = hit.collider.transform.root;
            if (!root.CompareTag("Player"))
                return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Transform refTransform = eyes != null ? eyes : transform;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(refTransform.position, viewRadius);

        Vector3 leftBoundary = DirectionFromAngle(-viewAngle / 2f, refTransform);
        Vector3 rightBoundary = DirectionFromAngle(viewAngle / 2f, refTransform);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(refTransform.position,
                        refTransform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(refTransform.position,
                        refTransform.position + rightBoundary * viewRadius);

        Gizmos.color = Color.cyan;
        Vector3 origin = Application.isPlaying ? patrolOrigin : transform.position;
        Gizmos.DrawWireSphere(origin, patrolRadius);
    }

    private Vector3 DirectionFromAngle(float angleInDegrees, Transform reference)
    {
        float angleRad = (angleInDegrees + reference.eulerAngles.y) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angleRad), 0f, Mathf.Cos(angleRad));
    }


    
    [ContextMenu("Test path to center of map")]
    private void TestPathToOrigin()
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(patrolOrigin, path);
        Debug.Log($"TestPath: status = {path.status}, corners = {path.corners.Length}");
    }
}
