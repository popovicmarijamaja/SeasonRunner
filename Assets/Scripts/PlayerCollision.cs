using UnityEngine;

public class PlayerCollision : MonoBehaviour
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
    private const string BulletTag = "bullet";

    private PlayerManager playerManager;
    [SerializeField] private Transform environmentSpawnPos;
    [SerializeField] private SpawnManager spawnManager;

    private void Awake()
    {
        playerManager = GetComponentInParent<PlayerManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!playerManager.IsShieldActive)
        {
            HandleObstacleCollision(other);
            HandleMushroomsCollision(other);
            HandleEnemyCollision(other);
            HandleBulletCollision(other);
        }

        HandleCoinCollision(other);
        HandleNewSectionCollision(other);
        HandleMagnetCollision(other);
        HandleStarCollision(other);
        HandleHealthPackCollision(other);
        HandleShieldCollision(other);
        HandleGunCollision(other);
    }

    private void HandleObstacleCollision(Collider other)
    {
        if (other.CompareTag(ObstacleTag))
        {
            GameManager.Instance.SetDeathByObstacleHit(playerManager);
        }
    }

    private void HandleMushroomsCollision(Collider other)
    {
        if (other.CompareTag(MushroomTag))
        {
            GameManager.Instance.DamagePlayer(playerManager);
        }
    }

    private void HandleEnemyCollision(Collider other)
    {
        if (other.CompareTag(EnemyTag) && other is CapsuleCollider)
        {
            GameManager.Instance.SetDeathByObstacleHit(playerManager);
        }
    }

    private void HandleBulletCollision(Collider other)
    {
        if (other.CompareTag(BulletTag))
        {
            GameManager.Instance.DamagePlayer(playerManager);
            other.gameObject.SetActive(false);
        }
    }

    private void HandleCoinCollision(Collider other)
    {
        if (other.CompareTag(CoinTag))
        {
            GameManager.Instance.CollectCoin(playerManager);
        }
    }

    private void HandleNewSectionCollision(Collider other)
    {
        if (other.CompareTag(NewSectionTag))
        {
            spawnManager.SpawnNewSection(environmentSpawnPos);
        }
    }

    private void HandleMagnetCollision(Collider other)
    {
        if (other.CompareTag(MagnetTag))
        {
            other.gameObject.SetActive(false);
            playerManager.ActivateMagnet();
            GameManager.Instance.PlayPowerUpSoundAndParticle(playerManager);
        }
    }

    private void HandleStarCollision(Collider other)
    {
        if (other.CompareTag(StarTag))
        {
            other.gameObject.SetActive(false);
            GameManager.Instance.CollectStar(playerManager);
        }
    }

    private void HandleHealthPackCollision(Collider other)
    {
        if (other.CompareTag(HealthPackTag))
        {
            other.gameObject.SetActive(false);
            GameManager.Instance.CollectHeart(playerManager);
        }
    }

    private void HandleShieldCollision(Collider other)
    {
        if (other.CompareTag(ShieldTag))
        {
            other.gameObject.SetActive(false);
            playerManager.ActivateShield();
            GameManager.Instance.PlayPowerUpSoundAndParticle(playerManager);
        }
    }

    private void HandleGunCollision(Collider other)
    {
        if (other.CompareTag(GunTag))
        {
            other.gameObject.SetActive(false);
            playerManager.ActivateGun();
            GameManager.Instance.PlayPowerUpSoundAndParticle(playerManager);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(GroundTag))
        {
            playerManager.IsGrounded = true;
        }
    }

    public void StartRoll()
    {
        playerManager.RollDown();
    }

    public void EndRoll()
    {
        playerManager.RollUp();
    }
}
