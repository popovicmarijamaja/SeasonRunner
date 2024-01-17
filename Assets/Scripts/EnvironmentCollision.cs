using UnityEngine;

public class EnvironmentCollision : MonoBehaviour
{
    private const string TurnEnvironmentOffTag = "turnEnvironmentOff";

    [SerializeField] private GameObject child;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TurnEnvironmentOffTag))
        {
            child.GetComponent<SpawnManager>().SpawnCoin();
            child.GetComponent<SpawnManager>().RespawnPower();
            child.GetComponent<SpawnManager>().RespawnObstacle();

        }
    }
}
