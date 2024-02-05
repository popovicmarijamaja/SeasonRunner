using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyData enemyData;

    private const string FireTag = "fire";

    private Animator animator;
    private CapsuleCollider capsuleCollider;
    private BoxCollider detector;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        detector = GetComponent<BoxCollider>();
    }

    public void SetEnemy()
    {
        AddBehaviourScripts();
        SetComponentsToDefault();
    }

    private void AddBehaviourScripts()
    {
        if (enemyData.Speed > 0)
        {
            if (GetComponent<WalkingEnemy>() == null)
            {
                gameObject.AddComponent<WalkingEnemy>();
            }
        }

        if (enemyData.isShooting)
        {
            if (GetComponent<ShootingEnemy>() == null)
            {
                gameObject.AddComponent<ShootingEnemy>();
            }
        }
    }

    private void SetComponentsToDefault()
    {
        animator.runtimeAnimatorController = enemyData.AnimatorController;
        animator.SetBool(enemyData.AliveParameter, true);
        capsuleCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(FireTag))
        {
            animator.SetBool(enemyData.AliveParameter, false);
            other.gameObject.SetActive(false);
            capsuleCollider.enabled = false;
            detector.enabled = false;
            if(GetComponent<ShootingEnemy>() != null)
            {
                GetComponent<ShootingEnemy>().CancelInvoke();
            }
        }
    }

    private void DestroyBehaviourScripts()
    {
        var walkingSoldierScript = GetComponent<WalkingEnemy>();
        var shootingSoldierScript = GetComponent<ShootingEnemy>();

        if (walkingSoldierScript != null)
            Destroy(walkingSoldierScript);

        if (shootingSoldierScript != null)
            Destroy(shootingSoldierScript);
    }

    private void OnDisable()
    {
        DestroyBehaviourScripts();
    }

}
