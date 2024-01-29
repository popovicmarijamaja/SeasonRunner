using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const string AliveParameter = "alive";
    private const string ShootParameter = "shoot";

    [SerializeField] private Transform firePoint;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private BoxCollider CharacterBoxCollider;
    private BoxCollider detector;

    private void Awake()
    {
        detector = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (!characterAnimator.GetBool(AliveParameter))
        {
            detector.enabled = false;
            CancelInvoke();
        }
    }

    private void Shooting()
    {
        characterAnimator.SetBool(ShootParameter, true);
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

    private void OnEnable()
    {
        detector.enabled = true;
        characterAnimator.SetBool(ShootParameter, false);
    }
}
