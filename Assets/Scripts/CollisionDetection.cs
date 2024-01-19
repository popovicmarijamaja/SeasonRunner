using UnityEngine;
using System.Collections;

public class CollisionDetection : MonoBehaviour
{
    private const string ObstacleTag = "obstacle";
    private const string CoinTag = "coin";
    private const string NewSectionTag = "newSection";
    private const string MagnetTag = "magnet";
    private const string GroundTag = "Ground";
    private const string StarTag = "star";
    private const string MushroomTag = "mushrooms";
    private const string HealthPackTag = "healthPack";
    private const string ShieldTag = "shield";
    private const string GunTag = "gun";
    private const string EnemyTag = "enemy";
    private const float ShieldLasting = 5f;
    private const float GunLasting = 5f;

    private bool isShieldActive = false;
    private PlayerManager playerManager;

    private void Awake()
    {
        playerManager = GetComponentInParent<PlayerManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ObstacleTag) && !isShieldActive)
        {
            GameManager.Instance.GameOver();
        }
        if (other.CompareTag(CoinTag))
        {
            GameManager.Instance.CoinCollect();
        }
        if (other.CompareTag(NewSectionTag))
        {
            GameManager.Instance.SpawnNewSection();
        }
        if (other.CompareTag(MagnetTag))
        {
            other.gameObject.SetActive(false);
            GameManager.Instance.MagnetPower();
        }
        if (other.CompareTag(StarTag))
        {
            other.gameObject.SetActive(false);
            GameManager.Instance.StarCollect();
        }
        if (other.CompareTag(MushroomTag) && !isShieldActive)
        {
            GameManager.Instance.HealthDecrease();
        }
        if (other.CompareTag(HealthPackTag))
        {
            other.gameObject.SetActive(false);
            GameManager.Instance.HealthIncrease();
        }
        if (other.CompareTag(ShieldTag))
        {
            other.gameObject.SetActive(false);
            StartCoroutine(Shield());
        }
        if (other.CompareTag(GunTag))
        {
            other.gameObject.SetActive(false);
            StartCoroutine(Gun());
        }
        if (other.CompareTag(EnemyTag) && !isShieldActive)
        {
            GameManager.Instance.GameOver();
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(GroundTag))
        {
            playerManager.Grounded();
        }
    }

    private IEnumerator Shield()
    {
        isShieldActive = true;
        yield return new WaitForSeconds(ShieldLasting);
        isShieldActive = false;
    }
    private IEnumerator Gun()
    {
        playerManager.hasGun = true;
        yield return new WaitForSeconds(GunLasting);
        playerManager.hasGun = false;
    }

    private void RollCollider()
    {
        playerManager.RollDown();
    }

    private void RollColliderUp()
    {
        playerManager.RollUp();
    }
}
