using UnityEngine;

public class FireParticle : MonoBehaviour
{
    private const float ParticleSpeed = 18f;

    private void OnEnable()
    {
        Invoke("Deactivate", 2f);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        MoveParticle();
    }

    private void MoveParticle()
    {
        transform.Translate(Time.deltaTime * ParticleSpeed * Vector3.left);
    }

}
