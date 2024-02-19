using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    private static float Speed;

    private void Awake()
    {
        Speed = 4f;
    }

    private void Update()
    {
        //Environment is moving
        transform.position += Time.deltaTime * Speed * Vector3.right;
        Speed += GameManager.SpeedIncrement * Time.deltaTime;
    }

    public void TurnOffAndReturnChildrenToPull()
    {
        Transform parent = gameObject.transform.parent;
        if (parent.GetComponentInChildren<SpawnManager>() == null)
            return;
        parent.GetComponentInChildren<SpawnManager>().ReturnObjectsInEnvironmentToPool(gameObject);
        gameObject.SetActive(false);
    }

}
