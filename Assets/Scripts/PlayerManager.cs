using UnityEngine;
using System.Collections;
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    private const float TransitionDuration = 0.2f;
    private const float JumpForce = 5f;
    private const string JumpTriggerParametar = "jump";
    private const string RollTriggerParametar = "down";
    private const string RightBool = "right";
    private const string LeftBool = "left";
    private const string DeathAnimBool = "death";

    [SerializeField] private GameObject player;
    [SerializeField] private Transform leftPos;
    [SerializeField] private Transform centrePos;
    [SerializeField] private Transform rightPos;
    [SerializeField] private ParticleSystem collect;
    [SerializeField] private ParticleSystem boom;
    public Animator PlayerAnim;
    public BoxCollider CollectorColider;
    private Rigidbody playerRb;
    private BoxCollider playerBoxCollider;

    private bool isTransitioning = false;
    private bool isGrounded;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        PlayerAnim = player.GetComponent<Animator>();
        CollectorColider = GetComponent<BoxCollider>();
        playerRb = player.GetComponent<Rigidbody>();
        playerBoxCollider = player.GetComponent<BoxCollider>();
    }

    private void Update()
    {
        //Defining in witch row player is moving
        if (Input.GetKeyDown(KeyCode.RightArrow) && transform.position == centrePos.position && !isTransitioning)
        {
            PlayerAnim.SetBool(RightBool, true);
            StartCoroutine(MoveToPosition(rightPos.position));
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && transform.position == leftPos.position && !isTransitioning)
        {
            PlayerAnim.SetBool(RightBool, true);
            StartCoroutine(MoveToPosition(centrePos.position));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && transform.position == centrePos.position && !isTransitioning)
        {
            PlayerAnim.SetBool(LeftBool, true);
            StartCoroutine(MoveToPosition(leftPos.position));
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && transform.position == rightPos.position && !isTransitioning)
        {
            PlayerAnim.SetBool(LeftBool, true);
            StartCoroutine(MoveToPosition(centrePos.position));
        }
        //Player jump
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            PlayerAnim.SetTrigger(JumpTriggerParametar);
            playerRb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
        //Player roll down
        if (Input.GetKeyDown(KeyCode.DownArrow) && isGrounded)
        {
            PlayerAnim.SetTrigger(RollTriggerParametar);
        }
    }

    public void Grounded()
    {
        isGrounded = true;
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
        PlayerAnim.SetBool(RightBool, false);
        PlayerAnim.SetBool(LeftBool, false);
        transform.position = targetPosition;
        isTransitioning = false;
    }

    public void PlayerAnimation(GameState currentState)
    {
        switch (currentState)
        {
            case GameState.Playing:
                PlayerAnim.enabled = true;
                break;
            case GameState.GameOver:
                PlayerAnim.SetBool(DeathAnimBool, true);
                break;
            default:
                PlayerAnim.enabled = false;
                break;
        }
    }
    public void RollDown()
    {
        playerBoxCollider.size = new Vector3(playerBoxCollider.size.x, 1.2f, playerBoxCollider.size.z);
        playerBoxCollider.center = new Vector3(playerBoxCollider.center.x, 0f, playerBoxCollider.center.z);
    }

    public void RollUp()
    {
        playerBoxCollider.size = new Vector3(playerBoxCollider.size.x, 2f, playerBoxCollider.size.z);
        playerBoxCollider.center = new Vector3(playerBoxCollider.center.x, 0.956f, playerBoxCollider.center.z);
    }

    public void CollectParticles()
    {
        collect.Play();
    }
    public void ExplosionParticle()
    {
        boom.Play();
    }
}
