using UnityEngine;

public class ShootingEnemy : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const string AliveParameter = "alive";
    private const string ShootParameter = "shoot";
    private const string FirePoint = "FirePoint";

    private Transform firePoint;
    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private BoxCollider detector;
    private Transform parent;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        detector = GetComponent<BoxCollider>();
        parent = transform.parent;
        firePoint = parent.Find(FirePoint);
    }

    private void Start()
    {
        SetShootingToDefault();
    }

    private void SetShootingToDefault()
    {
        detector.enabled = true;
        animator.SetBool(ShootParameter, false);
        transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    private void Shooting()
    {
        animator.SetBool(ShootParameter, true);
        GameObject bullet = ObjectPool.Instance.GetBullet();

        if (bullet != null)
        {
            bullet.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
            bullet.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            InvokeRepeating(nameof(Shooting), 0f, 1.2f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            CancelInvoke();
        }
    }
}
