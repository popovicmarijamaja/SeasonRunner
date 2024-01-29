using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private const float Speed = 2f;
    private const string AliveParameter = "alive";
    private const string FireTag = "fire";

    public Transform PointA;
    public Transform PointB;

    private Transform destination;
    private Animator animator;
    private BoxCollider boxCollider;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
    }
    private void Start()
    {
        destination = PointA;
    }

    private void Update()
    {
        if (animator.GetBool(AliveParameter))
        {
            MoveTowardsDestination();
        }
    }

    private void MoveTowardsDestination()
    {
        float step = Speed * Time.deltaTime;
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(FireTag))
        {
            animator.SetBool(AliveParameter, false);
            other.gameObject.SetActive(false);
            boxCollider.enabled = false;
        }
    }
    private void OnEnable()
    {
        animator.SetBool(AliveParameter, true);
        boxCollider.enabled = true;
    }
}
