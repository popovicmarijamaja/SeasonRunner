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

    public Transform leftPos;
    public Transform centrePos;
    public Transform rightPos;
    [SerializeField] private Transform firePoint;
    [SerializeField] private ParticleSystem collectibleParticle;
    [SerializeField] private ParticleSystem boomParticle;
    public TextMeshProUGUI scoreText;
    [SerializeField] private GameObject character;
    public Slider HealthSlider;
    private PowerUpManager powerUpManager;
    private Animator Animator;
    public BoxCollider CollectibleCollider;
    private Rigidbody Rigidbody;
    private BoxCollider BoxCollider;
    private GameObject stage;

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
    public int PlayerID;
    private PlayerNetworkMovement playerNetworkMovement;


    public bool IsGrounded
    {
        get { return isGrounded; }
        set { isGrounded = value; }
    }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        BoxCollider = GetComponent<BoxCollider>();
        powerUpManager = GetComponent<PowerUpManager>();
        playerNetworkMovement = GetComponent<PlayerNetworkMovement>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        PlayerID = players.Length;
    }

    private void Start()
    {
        Health = HealthMaxValue;


        SetComponents();
        CameraManager.Instance.SetCamera();

    }
    public void SetComponents()
    {
        //CameraManager.Instance.SetCamera();
        GameObject[] stages = GameObject.FindGameObjectsWithTag("stage");
        print("broj stageova " + stages.Length);
        for (int i = 0; i < stages.Length; i++)
        {
            if (PlayerID == stages[i].GetComponent<StageManager>().StageID)
            {
                stage = stages[i];
                StageManager stageManager = stage.GetComponent<StageManager>();
                print("stiglo");
                GetPos(stageManager.leftPos, stageManager.centrePos, stageManager.rightPos);
                gameObject.GetComponent<PlayerNetworkMovement>().GetPos(stageManager.leftPos, stageManager.centrePos, stageManager.rightPos);
                scoreText = stageManager.scoreText;
                HealthSlider = stageManager.HealthSlider;
                gameObject.GetComponent<PlayerCollision>().environmentSpawnPos = stageManager.environmentSpawnPos;
                gameObject.GetComponent<PlayerCollision>().spawnManager = stageManager.spawnManager;
            }
            else
                print("nije stiglo");
        }
        //GameManager.Instance.InitializeGame();
    }

    public void GetPos(Transform left, Transform centre, Transform right)
    {
        leftPos = left;
        centrePos = centre;
        rightPos= right;
        playerNetworkMovement.GetPos(left, centrePos, rightPos);
    }

    private void Update()
    {
        UpdateScore();
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

        if (CanMove(context) == false)
            return;
        if (!HasInputAuthority)
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
    private bool CanMove(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.CurrentState == GameState.Paused || IsDead || !context.performed)
            return false;
        else
            return true;
    }

    public void HandleJumpInput(InputAction.CallbackContext context)
    {
        if (CanJump(context) == false)
            return;
        NetworkManager.bufferedInput.IsJump = true;
        AudioManager.Instance.PlayJumpSound();
        isGrounded = false;
    }
    private bool CanJump(InputAction.CallbackContext context)
    {
        if (!isGrounded || IsDead || GameManager.Instance.CurrentState == GameState.Paused || !context.performed)
            return false;
        else
            return true;
    }

    public void HandleRollInput(InputAction.CallbackContext context)
    {
        if (CanRoll(context) == false)
            return;
        NetworkManager.bufferedInput.IsCrawl = true;
    }

    private bool CanRoll(InputAction.CallbackContext context)
    {
        if (IsDead || GameManager.Instance.CurrentState == GameState.Paused || !context.performed)
            return false;
        else
            return true;
    }

    public void HandleShootInput(InputAction.CallbackContext context)
    {
        if (CanShoot(context) == false)
            return;

        ShootFireball();
    }

    private bool CanShoot(InputAction.CallbackContext context)
    {
        if (IsDead || GameManager.Instance.CurrentState == GameState.Paused || !context.performed || !IsGunActive)
            return false;
        else
            return true;
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
        print("current state je: " + CurrentState);
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
        //Transform playerParent = gameObject.transform.parent;
        Transform playerParent = stage.transform;
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
