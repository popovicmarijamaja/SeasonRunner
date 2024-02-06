using UnityEngine;

public class EnvironmentCollision : MonoBehaviour
{
    private const string TurnEnvironmentOffTag = "turnEnvironmentOff";

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TurnEnvironmentOffTag))
        {
            gameObject.SetActive(false);
            SpawnManager.Instance.ReturnObjectsInEnvironmentToPool(gameObject);
        }
    }
}
