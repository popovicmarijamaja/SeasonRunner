using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyData enemyData;

    private Animator animator;
    private BoxCollider boxCollider;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemyData.FireTag))
        {
            animator.SetBool(enemyData.AliveParameter, false);
            other.gameObject.SetActive(false);
            boxCollider.enabled = false;
        }
    }
    private void OnEnable()
    {
        animator.SetBool(enemyData.AliveParameter, true);
        boxCollider.enabled = true;
    }
}
