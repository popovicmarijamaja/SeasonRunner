using UnityEngine;
using System.Collections;

public class CollisionDetection : MonoBehaviour // CR: PlayerCollision je preciznije ime za ovu klasu
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
    private const float ShieldLasting = 5f; // CR: ShieldDuration
    private const float GunLasting = 5f; // CR: GunDuration

    private bool isShieldActive = false; // CR: Primeti da ti je ovaj bool ovde, a isMagnetActive u GameManageru. Trebalo bi da budu na istom mestu. Kada zavrsis ostale stvari, razmisli o resenju za ovo pa cemo da diskutujemo.
    private PlayerManager playerManager;

    private void Awake()
    {
        playerManager = GetComponentInParent<PlayerManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
		// Ovu logiku mozes da razbijes u vise pomocnih funkcija da bi bilo lakse za citanje i potencijalne izmene.
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

    private IEnumerator Shield() // CR: ActivateShield
    {
        isShieldActive = true;
        yield return new WaitForSeconds(ShieldLasting);
        isShieldActive = false;
    }
    private IEnumerator Gun() // CR: ActivateGun
    {
        playerManager.hasGun = true;
        yield return new WaitForSeconds(GunLasting);
        playerManager.hasGun = false;
    }

    private void RollCollider() // CR: StartRoll
    {
        playerManager.RollDown();
    }

    private void RollColliderUp() // CR: EndRoll
    {
        playerManager.RollUp();
    }
}
