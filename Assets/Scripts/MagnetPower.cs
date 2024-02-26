using UnityEngine;

public class MagnetPower : MonoBehaviour
{
    private const float CoinSpeed = 20f;
    private const float CoinDefaultYPos = 0.5f;
    private const string PlayerRootTag = "PlayerRoot";
    private const string PlayerTag = "Player";

    private GameObject player;
    private bool isAttracted;


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
            
            GameObject[] players = GameObject.FindGameObjectsWithTag(PlayerTag);

            foreach (GameObject obj in players)
            {
                if (obj.transform.IsChildOf(other.transform))
                {
                    player = obj;
                }
            }
        }
        if (other.CompareTag(PlayerTag))
        {
            isAttracted = false;
            gameObject.SetActive(false);
        }
    }

    private void SetToDefault()
    {
        transform.position = new Vector3(transform.position.x, CoinDefaultYPos, transform.position.z);
    }

    private void OnDisable()
    {
        SetToDefault();
    }
}
