using UnityEngine;

public class EnvironmentMoving : MonoBehaviour // CR: Ovo ti je zapravo EnvironmentController
{
    private const string TurnEnvironmentOffTag = "turnEnvironmentOff";

    private static float Speed;

    private void Awake()
    {
        Speed = 4f;
    }

    private void Update()
    {
        //Environment is moving
        transform.position += Time.deltaTime * Speed * Vector3.right;
        Speed += GameManager.SpeedIncreasing * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other) // CR: Zasto je ova logika ovde kada imas dedicated EnvironmentCollision skriptu na istom objektu? Ovo pripada tamo.
    {
        if (other.gameObject.CompareTag(TurnEnvironmentOffTag))
        {
            gameObject.SetActive(false);
        }
    }
}
