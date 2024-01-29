using UnityEngine;

public class MagnetPower : MonoBehaviour
{
    private const float CoinSpeed = 20f;
    private const string PlayerRootTag = "PlayerRoot";
    private const string PlayerTag = "Player";

    private GameObject player;
    private bool isAttracted;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag(PlayerTag);
    }

    private void Update()
    {
        if (isAttracted)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, CoinSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerRootTag))
        {
            isAttracted = true;
        }
        if (other.CompareTag(PlayerTag))
        {
            isAttracted = false;
            gameObject.SetActive(false);
        }
    }
    
}
