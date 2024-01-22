using UnityEngine;
using System.Collections;
public class PlayerManager : MonoBehaviour
{
    private const float TransitionDuration = 0.2f;
    private const float JumpForce = 5f;
    private const string JumpTriggerParametar = "jump"; // CR: Na engleskom se pise Parameter :)
    private const string RollTriggerParametar = "down"; // CR: Isto
    private const string RightBool = "right";
    private const string LeftBool = "left";
    private const string DeathAnimBool = "death";

    [SerializeField] private GameObject player; // CR: Posto smo vec u PlayerManager-u, bolje ime bi mozda bilo character (jer nam termin "Player" ima sire znacenje od 
    [SerializeField] private Transform leftPos;
    [SerializeField] private Transform centrePos;
    [SerializeField] private Transform rightPos;
    [SerializeField] private Transform firePoint;
    [SerializeField] private ParticleSystem collect; // CR: collectibleParticle bi bilo bolje ime
    [SerializeField] private ParticleSystem boom; // CR: boomParticle
    [SerializeField] private GameObject fireParticlePrefab; // CR: Ako je ovo prefab za projektil generalno, bolje ga nazovi fireProjectilePrefab. Ne interesuje nas da li je particle ili ne, interesuje nas sta predstavlja to sto instanciramo.
    public Animator PlayerAnim; // CR: Nema potrebe za skracivanjem ovde, ali ni za recju Player (posto smo vec u *Player*Controlleru). Mozes recimo CharacterAnimator.
    public BoxCollider CollectorColider; // CR: Mozda CollectibleCollider ili CollectionCollider ovde bolje lezi
    private Rigidbody playerRb; // CR: characterRb
    private BoxCollider playerBoxCollider; // CR: Posto vec imas jedan dedicated collider za collectables, bilo bi dobro da ime i ovog collidera eksplicitno objasni za sta on sluzi.

    private bool isTransitioning = false;
    private bool isGrounded;
    public bool hasGun;

    private void Awake()
    {
        PlayerAnim = player.GetComponent<Animator>();
        CollectorColider = GetComponent<BoxCollider>();
        playerRb = player.GetComponent<Rigidbody>();
        playerBoxCollider = player.GetComponent<BoxCollider>();
    }

    private void Update() // CR: Uoci kako si komentarima podelila ovu veliku funkciju u 4 odvojene operacije. Svaka od tih operacija treba da se izdvoji u svoju metodu, pa da se te metode samo pozovu odavde.
    {
        //Defining in which row player is moving
		// CR: Uoci kako ti svi uslovi u ovoj sekciji imaju && !isTransitioning. Ovo ti je znak da mozes da izdvojis taj uslov nivo iznad.
		// CR: Da li primecujes slicnost izmedju ovog koda i koda koji menja GameState u GameManager-u? Razmisli o tome pa cemo da caskamo.
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
        //Player shoot
        if (Input.GetKeyDown(KeyCode.Space) && hasGun)
        {
            ShootFireParticle();
        }
    }
    private void ShootFireParticle() // CR: Funkcija treba da opisuje operaciju, ne konkretnu implementaciju. Drugim recima, ni ovde nas ne interesuje sto je u pitanju particle. ShootFireProjectile ili cak nesto tipa ShootFireball je bolji naziv.
    {
        GameObject fireParticle = ObjectPool.Instance.GetPooledObjectFire();

        if (fireParticle != null)
        {
            fireParticle.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
            fireParticle.SetActive(true);
        }
    }

    public void Grounded() // CR: Ovo ne mora da bude metoda, samo napravi set property. 
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

    public void PlayerAnimation(GameState currentState) // CR: Glagol. Npr. SetPlayerAnimation
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

    public void CollectParticles() // CR: PlayCollectionParticle. Inace, primeti kako ime ove funkcije zvuci kao da player skuplja partikle
    {
        collect.Play();
    }
    public void ExplosionParticle() // CR: PlayExplosionParticle
    {
        boom.Play();
    }
}
