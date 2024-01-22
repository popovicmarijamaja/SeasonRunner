using UnityEngine;

public class FireParticle : MonoBehaviour // CR: Ja bih ovo nazvao Fireball
{
    private const float ParticleSpeed = 18f; // CR: Samo speed je dovoljno, znamo da se odnosi na Fireball

    private void OnEnable()
    {
        Invoke("Deactivate", 2f); // CR: Umesto hardkodiranog stringa, ovde mozes da upotrebis nameof operator da bi odrzala referencu na metodu koju invoke-ujes. Proguglaj nameof pa ces da vidis
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        MoveParticle();
    }

    private void MoveParticle() // CR: Move je dovoljno
    {
        transform.Translate(Time.deltaTime * ParticleSpeed * Vector3.left);
    }

}
