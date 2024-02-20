using UnityEngine;

public class EnvironmentCollision : MonoBehaviour
{
    private const string TurnEnvironmentOffTag = "turnEnvironmentOff";

    private EnvironmentController controller;

    private void Awake()
    {
        controller = GetComponent<EnvironmentController>(); // CR: Kada ovako getujes komponentu, to je znak da su te dve komponente cvrsto uparene. Posto ova cela klasa ima samo OnTriggerEnter u kojem se poziva logika iz EnvironmentControllera, imalo bi smisla da ta logika zapravo bude u EnvironmentController-u. Ova klasa ti i nije potrebna jer vestacki odvaja stvari koje bi trebalo da su jedan entitet.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TurnEnvironmentOffTag))
        {
            controller.TurnOffAndReturnChildrenToPull(); // CR: Pool, ne Pull :)
        }
    }
}
