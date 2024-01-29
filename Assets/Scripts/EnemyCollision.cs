using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    private const string FireTag = "fire";
    private const string DeathParameter = "death";

    private Animator anim;
    private BoxCollider bc;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        bc = GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(FireTag))
        {
            anim.SetBool(DeathParameter, true);
            other.gameObject.SetActive(false);
            bc.enabled = false;
        }
    }
}
