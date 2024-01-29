using UnityEngine;

public class Fireball : MonoBehaviour
{
    private const float Speed = 18f;

    private void OnEnable()
    {
        Invoke(nameof(Deactivate), 2f);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(Time.deltaTime * Speed * Vector3.left);
    }

}
