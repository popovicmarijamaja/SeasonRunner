using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const string DeathParameter = "death";

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
        if (characterAnimator.GetBool(DeathParameter))
        {
            detector.enabled = false;
            CancelInvoke();
        }
    }
    private void Shooting()
    {
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
        characterAnimator.SetBool(DeathParameter, false);
        CharacterBoxCollider.enabled = true;
    }
}
