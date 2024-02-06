using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    public const int HealthMaxValue = 4;
    public const int HealthMinValue = 0;
    private const float TransitionDuration = 0.2f;
    private const float JumpForce = 5f;
    private const string JumpTriggerParameter = "jump";
    private const string RollTriggerParameter = "down";
    private const string RightBool = "right";
    private const string LeftBool = "left";
    private const string DeathAnimBool = "death";

    [SerializeField] private GameObject character;
    [SerializeField] private Transform leftPos;
    [SerializeField] private Transform centrePos;
    [SerializeField] private Transform rightPos;
    [SerializeField] private Transform firePoint;
    [SerializeField] private ParticleSystem collectibleParticle;
    [SerializeField] private ParticleSystem boomParticle;
    private PowerUpManager powerUpManager;
    public Animator CharacterAnimator;
    public BoxCollider CollectibleCollider;
    private Rigidbody characterRb;
    private BoxCollider characterBoxCollider;

    public bool IsShieldActive = false;
    public bool isMagnetActive = false;
    public bool isGunActive = false;
    private bool isTransitioning = false;
    public int health;

    private bool isGrounded;
    public bool IsGrounded
    {
        get { return isGrounded; }
        set { isGrounded = value; }
    }
    private void Awake()
    {
        CharacterAnimator = character.GetComponent<Animator>();
        CollectibleCollider = GetComponent<BoxCollider>();
        characterRb = character.GetComponent<Rigidbody>();
        characterBoxCollider = character.GetComponent<BoxCollider>();
        powerUpManager = GetComponent<PowerUpManager>();
    }
    private void Start()
    {
        health = HealthMaxValue;
    }

    private void Update() 
    {
        HandleMovementInput();
        HandleJumpInput();
        HandleRollInput();
        HandleShootInput();
    }

    private void HandleMovementInput()
    {
        if (!isTransitioning)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (transform.position == centrePos.position)
                {
                    SetCharacterAnimation(RightBool);
                    StartCoroutine(MoveToPosition(rightPos.position));
                }
                else if (transform.position == leftPos.position)
                {
                    SetCharacterAnimation(RightBool);
                    StartCoroutine(MoveToPosition(centrePos.position));
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (transform.position == centrePos.position)
                {
                    SetCharacterAnimation(LeftBool);
                    StartCoroutine(MoveToPosition(leftPos.position));
                }
                else if (transform.position == rightPos.position)
                {
                    SetCharacterAnimation(LeftBool);
                    StartCoroutine(MoveToPosition(centrePos.position));
                }
            }
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            AudioManager.Instance.PlayJumpSound();
            CharacterAnimator.SetTrigger(JumpTriggerParameter);
            characterRb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void HandleRollInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CharacterAnimator.SetTrigger(RollTriggerParameter);
        }
    }

    private void HandleShootInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGunActive)
        {
            ShootFireball();
        }
    }

    private void SetCharacterAnimation(string boolName)
    {
        CharacterAnimator.SetBool(boolName, true);
    }

    private void ShootFireball()
    {
        GameObject fireball = ObjectPool.Instance.GetFireball();

        if (fireball != null)
        {
            fireball.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
            fireball.SetActive(true);
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        isTransitioning = true;
        float elapsedTime = 0f;
        Vector3 startingPos = transform.position;

        while (elapsedTime < TransitionDuration)
        {
            transform.position = Vector3.Lerp(startingPos, targetPosition, elapsedTime / TransitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        CharacterAnimator.SetBool(RightBool, false);
        CharacterAnimator.SetBool(LeftBool, false);
        transform.position = targetPosition;
        isTransitioning = false;
    }

    public void SetPlayerState(GameState currentState)
    {
        switch (currentState)
        {
            case GameState.Playing:
                CharacterAnimator.enabled = true;
                break;
            case GameState.GameOver:
                CharacterAnimator.SetBool(DeathAnimBool, true);
                character.transform.position = new Vector3(character.transform.position.x, 0, character.transform.position.y);
                characterRb.constraints = RigidbodyConstraints.FreezeAll;
                break;
            default:
                CharacterAnimator.enabled = false;
                break;
        }
    }

    public void ActivateShield()
    {
        powerUpManager.ActivateShield();
    }

    public void ActivateGun()
    {
        powerUpManager.ActivateGun();
    }

    public void ActivateMagnet()
    {
        powerUpManager.ActivateMagnet();
    }

    public void DecreaseHealth()
    {
        health--;
    }

    public void IncreaseHealth()
    {
        health++;
    }

    public void RollDown()
    {
        characterBoxCollider.size = new Vector3(characterBoxCollider.size.x, 1.2f, characterBoxCollider.size.z);
        characterBoxCollider.center = new Vector3(characterBoxCollider.center.x, 0f, characterBoxCollider.center.z);
    }

    public void RollUp()
    {
        characterBoxCollider.size = new Vector3(characterBoxCollider.size.x, 2f, characterBoxCollider.size.z);
        characterBoxCollider.center = new Vector3(characterBoxCollider.center.x, 0.956f, characterBoxCollider.center.z);
    }

    public void PlayCollectionParticle()
    {
        collectibleParticle.Play();
    }
    public void PlayExplosionParticle()
    {
        boomParticle.Play();
    }
}
