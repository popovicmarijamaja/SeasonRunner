using UnityEngine;

public class EnvironmentCollision : MonoBehaviour
{
    private const string TurnEnvironmentOffTag = "turnEnvironmentOff";

    private EnvironmentController controller;

    private void Awake()
    {
        controller = GetComponent<EnvironmentController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TurnEnvironmentOffTag))
        {
            controller.TurnOffAndReturnChildrenToPull();
        }
    }
}
