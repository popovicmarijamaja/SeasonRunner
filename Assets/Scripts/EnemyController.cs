using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private const float Speed = 2f;
    private const string AliveParametar = "alive"; // CR: Na engleskom je Parameter
    private const string FireTag = "fire";

    public Transform PointA;
    public Transform PointB;

    private Transform target; // CR: Ovo je samo po sebi nejasno ime. movementTarget je bolje. destination je jos bolje.
    private Animator anim; // CR: Slobodno celo ime, animator

    private void Start()
    {
        anim = GetComponent<Animator>();
        target = PointA;
    }

    private void Update()
    {
        if (anim.GetBool(AliveParametar))
        {
            MoveTowards();
        }
    }

    private void MoveTowards() // CR: MoveTowardsDestination, u zavisnosti od toga kako prekrstis target
    {
        float step = Speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            // If it has reached the target, change the destination
            target = (target == PointA) ? PointB : PointA; // CR: Ovu liniju bih ubacio u RotateEnemy funkciju, koju bih preimenoveo (vidi ispod)
            RotateEnemy();
        }
    }
    private void RotateEnemy() // CR: Posto smo u EnemyControlleru, rec Enemy se podrazumeva. Umesto Rotate, ja bih ubacio i prethodnu liniju i nazvao ovo ChangeDestination
    {
        // Determine the target rotation based on the new destination
        Quaternion targetRotation = (target == PointB) ? Quaternion.Euler(0f, 0f, 0f) : Quaternion.Euler(0f, 180f, 0f);
        transform.rotation = targetRotation;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(FireTag))
        {
            anim.SetBool(AliveParametar, false);
            other.gameObject.SetActive(false);
        }
    }
}
