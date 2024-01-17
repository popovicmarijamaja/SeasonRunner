using UnityEngine;

public class EnvironmentMoving : MonoBehaviour
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TurnEnvironmentOffTag))
        {
            gameObject.SetActive(false);
        }
    }
}
