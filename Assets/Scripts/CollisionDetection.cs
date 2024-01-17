using UnityEngine;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ObstacleTag))
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
        if (other.CompareTag(MushroomTag))
        {
            GameManager.Instance.HealthDecrease();
        }
        if (other.CompareTag(HealthPackTag))
        {
            other.gameObject.SetActive(false);
            GameManager.Instance.HealthIncrease();
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(GroundTag))
        {
            PlayerManager.Instance.Grounded();
        }
    }

    private void RollCollider()
    {
        PlayerManager.Instance.RollDown();
    }

    private void RollColliderUp()
    {
        PlayerManager.Instance.RollUp();
    }
}
