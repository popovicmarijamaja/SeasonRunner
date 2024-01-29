using UnityEngine;

public class EnemyWalking : MonoBehaviour
{
    public Transform PointA;
    public Transform PointB;

    private Transform destination;
    private Animator animator;
    private EnemyController enemyController;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyController = GetComponent<EnemyController>();
    }
    private void Start()
    {
        destination = PointA;
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
            destination = (destination == PointA) ? PointB : PointA;
            ChangeDestination();
        }
    }
    private void ChangeDestination()
    {
        // Determine the target rotation based on the new destination
        Quaternion targetRotation = (destination == PointB) ? Quaternion.Euler(0f, 0f, 0f) : Quaternion.Euler(0f, 180f, 0f);
        transform.rotation = targetRotation;
    }
}