using UnityEngine;

public class WalkingEnemy : MonoBehaviour
{
    private const string PositionA = "PositionA";
    private const string PositionB = "PositionB";

    private Transform pointA;
    private Transform pointB;
    private Transform parent;
    private Transform destination;
    private Animator animator;
    private EnemyController enemyController;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyController = GetComponent<EnemyController>();
        parent = transform.parent;
        pointA = parent.Find(PositionA);
        pointB = parent.Find(PositionB);
    }
    private void Start()
    {
        destination = pointA;
    }

    private void Update()
    {
        if (animator.GetBool(enemyController.enemyData.AliveParameter))
        {
            MoveTowardsDestination();
        }
    }

    private void MoveTowardsDestination()
    {
        float step = enemyController.enemyData.Speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, destination.position, step);
        if (Vector3.Distance(transform.position, destination.position) < 0.01f)
        {
            // If it has reached the target, change the destination
            destination = (destination == pointA) ? pointB : pointA;
            ChangeDestination();
        }
    }
    private void ChangeDestination()
    {
        // Determine the target rotation based on the new destination
        Quaternion targetRotation = (destination == pointB) ? Quaternion.Euler(0f, 0f, 0f) : Quaternion.Euler(0f, 180f, 0f);
        transform.rotation = targetRotation;
    }
}
