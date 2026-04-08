using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;
    
    [SerializeField] float rotationSpeed;
    [SerializeField] float stopDistance;

    float currentSpeed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        agent.updatePosition = false;
        agent.updateRotation = false;
    }


    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        
        if (distanceToPlayer > stopDistance)
        {
            agent.SetDestination(player.position);
            animator.SetBool("Walking", true);
        }
        else
        {
            agent.ResetPath();
            animator.SetBool("Walking", false);
        }
        
        HandleRotation();
    }
    void HandleRotation()
    {
        Vector3 velocity = agent.desiredVelocity;

        if (velocity.sqrMagnitude < 0.1f) return;

        Quaternion targetRotation = Quaternion.LookRotation(velocity);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    // Function to make root motion sync with navmesh - Gets called automatically - Event Function
    void OnAnimatorMove()
    {
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            float navSpeed = agent.desiredVelocity.magnitude;
            float animSpeed = animator.deltaPosition.magnitude / Time.deltaTime;
            float scale = animSpeed > 0.001f ? navSpeed / animSpeed : 0f;

            transform.position += animator.deltaPosition * scale;
        }
        agent.nextPosition = transform.position;
    }
   
}
