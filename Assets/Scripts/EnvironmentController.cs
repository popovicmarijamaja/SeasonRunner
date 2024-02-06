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

}
