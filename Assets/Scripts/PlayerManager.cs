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

    //[SerializeField] private GameObject character;
    private Transform leftPos;
    private Transform centrePos;
    private Transform rightPos;
    [SerializeField] private Transform firePoint;
    [SerializeField] private ParticleSystem collectibleParticle;
    [SerializeField] private ParticleSystem boomParticle;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject character;
    public Slider HealthSlider;
    private PowerUpManager powerUpManager;
    private Animator Animator;
    public BoxCollider CollectibleCollider;
    private Rigidbody Rigidbody;
    private BoxCollider BoxCollider;

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
        Animator = GetComponent<Animator>();
        CollectibleCollider = GetComponent<BoxCollider>();
        Rigidbody = GetComponent<Rigidbody>();
        BoxCollider = GetComponent<BoxCollider>();
        powerUpManager = GetComponent<PowerUpManager>();
    }

    private void Start()
    {
        Health = HealthMaxValue;
    }

    public void GetPos(Transform left, Transform centre, Transform right)
    {
        leftPos = left;
        centrePos = centre;
        rightPos= right;
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
            if (transform.position.z == centrePos.position.z)
            {
                NetworkManager.bufferedInput.IsMoveRight = true;
            }
            else if (transform.position.z == leftPos.position.z)
            {
                NetworkManager.bufferedInput.IsMoveCentre = true;
            }
        }
        else if (inputValue < 0)
        {
            if (transform.position.z == centrePos.position.z)
            {
                NetworkManager.bufferedInput.IsMoveLeft = true;
            }
            else if (transform.position.z == rightPos.position.z || transform.position.z >= rightPos.position.z - 0.1)
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

        //CharacterAnimator.SetTrigger(RollTriggerParameter);
    }

    public void HandleShootInput(InputAction.CallbackContext context)
    {
        if (IsDead || GameManager.Instance.CurrentState == GameState.Paused || !context.performed || !IsGunActive)
            return;

        ShootFireball();
    }

    private void SetCharacterAnimation(string boolName)
    {
        //CharacterAnimator.SetBool(boolName, true);
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
                Animator.enabled = true;
                break;
            case GameState.GameOver:
                Animator.SetBool(DeathAnimBool, true);
                IsDead = true;
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                gameObject.GetComponent<PlayerManager>().enabled = false;
                break;
            default:
                Animator.enabled = false;
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
        BoxCollider.size = new Vector3(BoxCollider.size.x, 1.2f, BoxCollider.size.z);
        BoxCollider.center = new Vector3(BoxCollider.center.x, 0f, BoxCollider.center.z);
    }

    public void RollUp()
    {
        BoxCollider.size = new Vector3(BoxCollider.size.x, 2f, BoxCollider.size.z);
        BoxCollider.center = new Vector3(BoxCollider.center.x, 0.956f, BoxCollider.center.z);
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
