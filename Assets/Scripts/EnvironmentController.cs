using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    private static float Speed;
    private const string TurnEnvironmentOffTag = "turnEnvironmentOff";

    private void Awake()
    {
        Speed = 4f;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.position += Time.deltaTime * Speed * Vector3.right;
        Speed += GameManager.SpeedIncrement * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TurnEnvironmentOffTag))
        {
            TurnOffAndReturnChildrenToPool();
        }
    }

    public void TurnOffAndReturnChildrenToPool()
    {
        Transform parent = gameObject.transform.parent;
        if (parent.GetComponentInChildren<SpawnManager>() == null)
            return;
        parent.GetComponentInChildren<SpawnManager>().ReturnObjectsInEnvironmentToPool(gameObject); // CR: Je l' ovo bila jedna od stvari koje ti se "ne svidja" kako si resila?
        gameObject.SetActive(false);
    }

}
