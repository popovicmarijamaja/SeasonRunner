using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class PlayerManager : NetworkBehaviour
{
    public const int HealthMaxValue = 4;
    public const int HealthMinValue = 0;
    public const float SpeedIncrement = 0.03f;
    private const float TransitionDuration = 0.2f;
    private const float JumpForce = 5f;
    private const string JumpTriggerParameter = "jump";
    private const string RollTriggerParameter = "down";
    private const string RightBool = "right";
    private const string LeftBool = "left";
    private const string DeathAnimBool = "death";

    [SerializeField] private GameObject character;
    private readonly Vector3 leftPos = new (0, 0, -1);
    private readonly Vector3 centrePos = new (0, 0, 0);
    private readonly Vector3 rightPos= new (0, 0, 1);
    [SerializeField] private Transform firePoint;
    [SerializeField] private ParticleSystem collectibleParticle;
    [SerializeField] private ParticleSystem boomParticle;
    [SerializeField] private TextMeshProUGUI scoreText;
    public Slider HealthSlider;
    private PowerUpManager powerUpManager;
    public Animator CharacterAnimator;
    public BoxCollider CollectibleCollider;
    private Rigidbody characterRb;
    private BoxCollider characterBoxCollider;

    public int Health;
    public bool IsDead;
    public bool IsShieldActive = false;
    public bool IsMagnetActive = false;
    public bool IsGunActive = false;
    private bool isGrounded;
    public int TotalScore;
    public int CoinScore;
    private float runningScore;
    private float scoreIncrementPerSecond = 2f;
    public GameState CurrentState;


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
        Health = HealthMaxValue;
    }

    private void Update()
    {
        //UpdateScore();
    }
    

    private void UpdateScore()
    {
        TotalScore = Convert.ToInt32(CoinScore + runningScore);
        if (CurrentState == GameState.Playing)
        {
            runningScore += Time.deltaTime * scoreIncrementPerSecond;
        }
        scoreIncrementPerSecond += SpeedIncrement * Time.deltaTime;
        UIManager.Instance.UpdateScoreText(TotalScore, scoreText);
    }

    public void HandleMovementInput(InputAction.CallbackContext context)
    {
        float inputValue = context.ReadValue<float>();

        if (GameManager.Instance.CurrentState == GameState.Paused || IsDead || !context.performed)
            return;

        if (inputValue > 0)
        {
            if (transform.position == centrePos)
            {
                NetworkManager.bufferedInput.IsMoveRight = true;
            }
            else if (transform.position == leftPos)
            {
                NetworkManager.bufferedInput.IsMoveCentre = true;
            }
        }
        else if (inputValue < 0)
        {
            if (transform.position == centrePos)
            {
                NetworkManager.bufferedInput.IsMoveLeft = true;
            }
            else if (transform.position == rightPos)
            {
                NetworkManager.bufferedInput.IsMoveCentre = true;
            }
        }
    }

    public void HandleJumpInput(InputAction.CallbackContext context)
    {
        if (!isGrounded || IsDead || GameManager.Instance.CurrentState == GameState.Paused || !context.performed)
            return;
        NetworkManager.bufferedInput.IsJump = true;
        //AudioManager.Instance.PlayJumpSound();
        isGrounded = false;
    }

    public void HandleRollInput(InputAction.CallbackContext context)
    {
        if (IsDead || GameManager.Instance.CurrentState == GameState.Paused || !context.performed)
            return;

        CharacterAnimator.SetTrigger(RollTriggerParameter);
    }

    public void HandleShootInput(InputAction.CallbackContext context)
    {
        if (IsDead || GameManager.Instance.CurrentState == GameState.Paused || !context.performed || !IsGunActive)
            return;

        ShootFireball();
    }

    private void SetCharacterAnimation(string boolName)
    {
        CharacterAnimator.SetBool(boolName, true);
    }

    private void ShootFireball()
    {
        GameObject fireball = ObjectPool.Instance.GetFireball();

        if (fireball == null)
            return;

        fireball.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
        fireball.SetActive(true);
    }

    public void SetPlayerState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Playing:
                CharacterAnimator.enabled = true;
                break;
            case GameState.GameOver:
                CharacterAnimator.SetBool(DeathAnimBool, true);
                IsDead = true;
                character.transform.position = new Vector3(character.transform.position.x, 0, character.transform.position.z);
                characterRb.constraints = RigidbodyConstraints.FreezeAll;
                gameObject.GetComponent<PlayerManager>().enabled = false;
                break;
            default:
                CharacterAnimator.enabled = false;
                break;
        }
        SetEnvironmentMovement();
    }

    private void SetEnvironmentMovement()
    {
        bool isPlaying;
        Transform playerParent = gameObject.transform.parent;

        if (CurrentState == GameState.GameOver)
            isPlaying = false;
        else
            isPlaying = CurrentState == GameState.Playing;

        playerParent.GetComponentInChildren<EnvironmentController>().enabled = isPlaying;

        foreach (var pooledObject in ObjectPool.Instance.EnvirontmentPool)
            if (playerParent == pooledObject.transform.parent)
                pooledObject.GetComponent<EnvironmentController>().enabled = isPlaying;
    }

    public void ActivateShield()
    {
        powerUpManager.ActivateShield(gameObject.GetComponent<PlayerManager>());
    }

    public void ActivateGun()
    {
        powerUpManager.ActivateGun(gameObject.GetComponent<PlayerManager>());
    }

    public void ActivateMagnet()
    {
        powerUpManager.ActivateMagnet(gameObject.GetComponent<PlayerManager>());
    }

    public void DecreaseHealth()
    {
        Health--;
    }

    public void IncreaseHealth()
    {
        Health++;
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
