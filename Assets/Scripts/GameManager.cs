using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public const float SpeedIncreasing = 0.03f; // CR: Imena field-ova, konstanti i promenljivih treba da se zavrsavaju imenicom, jer predstavljaju konkretnu instancu necega. Npr. ovde bi moglo SpeedIncrement.
    private const float MagnetLasting = 6f; // CR: Iz istog razloga bi ovde bilo bolje MagnetDuration.
    private const string GameplayScene = "Gameplay";
    private const int HealthMaxValue = 4;
    private const int HealthMinValue = 0;

    [SerializeField] private GameObject getReadyEnvironment; // CR: Kada identifikator pocinje glagolom (kao ovde get), prva asocijacija je da je to metoda (jer vrsi neku radnju). Field-ovi, konstante i promenljive treba da sadrze samo prideve i imenice (npr. blueButton).
    [SerializeField] private Transform newSectionPos; // CR: Ovo ime je adekvatne strukture, ali bi moglo da bude jasnije - npr. environmentSpawnPos
    [SerializeField] private PlayerManager playerManager;

    private int coinScore;
    private float runningScore;
    public int totalScore;
    private float runningScoreValue = 2f; // CR: Zasto je ovome inicijalna vrednost 2 a ne 0?
    private bool isMagnetRunning; // CR: Posto je ovo igrica o trcanju, rec "run" ovde ima malo snaznije znacenje nego u nekog drugoj igri. Da bi izbelgi konfuziju, bolje ime u ovom slucaju je isMagnetActive.
    private int health; // CR: Posto je ova klasa GameManager, ako nesto nazoves samo health ispada kao da GameManager ima svoj health. Ovde bi mogla da nazoves field playerHealth (jer to i jeste), ali ova informacija zapravo pripada u PlayerManageru (gde je okej da se zove samo health).

    public GameState CurrentState { get; private set; }

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
    }

    private void Start()
    {
        CurrentState = GameState.MainMenu;
        MainMenu();
        health = HealthMaxValue;
    }

    public void MainMenu() // CR: Kao sto sam pomenuo gore, imena metoda treba da pocinju glagolom koji opisuje radnju koju metoda vrsi. Npr. ShowMainMenu.
    {
		// CR: S obzirom da ova metoda aktivira glavni meni, promena stanja koja ti se desava na :43 bi trebalo da se prebaci ovde.
		// Posto ne postoji situacija gde ces da pozoves ovu metodu bez da promenis State aplikacije, promena State-a je deo ove operacije i treba da bude unutar metode.
        playerManager.PlayerAnimation(CurrentState);
        playerManager.enabled = false;
        EnvironmentMoving();
    }

    private void SetGameState(GameState state)
    {
        CurrentState = state;
    }

    private void Update()
    {
        totalScore = Convert.ToInt32(coinScore + runningScore);
        if (CurrentState == GameState.Playing)
        {
            runningScore += Time.deltaTime * runningScoreValue;
        }
        runningScoreValue += SpeedIncreasing * Time.deltaTime;
        UIManager.Instance.ScoreText(totalScore);
    }

    public void Play() // CR: Posto ti se metoda za pauzu zove PauseGame, ova bi valjalo da se zove PlayGame, zbog konzistentnosti.
    {
        SetGameState(GameState.Playing);
        EnvironmentMoving();
        playerManager.PlayerAnimation(CurrentState);
        playerManager.enabled = true;
        UIManager.Instance.PauseMenu(CurrentState);
        UIManager.Instance.Play();
        AudioManager.Instance.BackgroundMusic();
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.GameOver)
            return;
        
        SetGameState(GameState.Pause);
        EnvironmentMoving();
        playerManager.PlayerAnimation(CurrentState);
        playerManager.enabled = false;
        UIManager.Instance.PauseMenu(CurrentState);
    }

    public void GameOver() // CR: U duhu imenovanja metoda glagolima i konzistentnosti imenovanja, ova metoda moze da se zove EndGame (GameState u koji ona prelazi je super da ostane GameOver).
    {
        SetGameState(GameState.GameOver); // CR: Uoci kako ti ova i ostale metode pocinju promenom stanja. Samo ti je gorepomenuti MainMenu izuzetak.
        EnvironmentMoving(); // CR: Uoci takodje kako svaki put kada promenis stanje zoves ovu i sledece dve linije koda. To ti je jasan indikator da te tri linije pripadaju unutar SetGameState metode, jer su deo iste operacije.
		playerManager.PlayerAnimation(CurrentState); 
        playerManager.enabled = false;
        StartCoroutine(WaitForEndOfParcticles());
        if (health != HealthMinValue)
        {
            playerManager.ExplosionParticle();
            health = HealthMinValue;
            UIManager.Instance.HealthSlider(health);
        }
    }

    private IEnumerator WaitForEndOfParcticles()
    {
        yield return new WaitForSeconds(2f);
        UIManager.Instance.YouLostMenu(CurrentState);
    }

    private void EnvironmentMoving() // CR: Treba nam glagol, tako da bi nesto popus SetEnvironmentMovement bilo adekvatno.
    {
        bool isPlaying = CurrentState == GameState.Playing;
        getReadyEnvironment.GetComponent<EnvironmentMoving>().enabled = isPlaying;
        foreach (var pooledObject in ObjectPool.Instance.pooledObjectsEnvironment)
            pooledObject.GetComponent<EnvironmentMoving>().enabled = isPlaying;
    }

    public void CoinCollect() // CR: CollectCoin, jer ime treba da *pocne* glagolom
    {
        Collectible();
        coinScore += 10;
    }

    public void StarCollect() // CR: CollectStar
    {
        Collectible();
        coinScore += 50;
    }

    public void MagnetPower() // CR: CollectMagnet. Ili, ako zelis da diferenciras power-up metode od score metoda moze i ActivateMagnet. To bi bilo konzistentno i sa isMagnetActive fieldom koji se koristi unutar metode.
	{
        Collectible();
        if (isMagnetRunning)
        {
            StopCoroutine(MagnetCountDown());
        }

        isMagnetRunning = true;
        StartCoroutine(MagnetCountDown());
    }

    private IEnumerator MagnetCountDown()
    {
        playerManager.CollectorColider.enabled = true;
        yield return new WaitForSeconds(MagnetLasting);
        playerManager.CollectorColider.enabled = false;
        isMagnetRunning = false;
    }

    private void Collectible() // CR: I ovde nam treba glagol, plus konkretniji opis sta ovo radi. Npr. PlayCollectibleEffects ili PlayCollectSoundAndParticle.
    {
        AudioManager.Instance.CoinSound();
        playerManager.CollectParticles();
    }

    public void SpawnNewSection()
    {
        GameObject environment = ObjectPool.Instance.GetPooledObjectEnvironment();
        if (environment != null)
        {
            environment.transform.position = newSectionPos.position;
            environment.SetActive(true);
        }
    }

    public void HealthDecrease() // CR: Ostavi ovu metodu za kraj pa cemo da se cujemo oko ispravki za nju.
    {
        health--;
        UIManager.Instance.HealthSlider(health);
        if (health == HealthMinValue)
        {
            GameOver();
        }
    }
    public void HealthIncrease() // CR: Ova metoda nije opsta kao sto joj ime implicira, vec podrazumeva skupljanje collectible-a. Nazovi je CollectHeart ili nesto slicno.
    {
        Collectible();
        if (health == HealthMaxValue)
            return;
        health++;
        UIManager.Instance.HealthSlider(health);
    }


    public void LoadGameplayScene()
    {
        SceneManager.LoadScene(GameplayScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
public enum GameState
{
    MainMenu,
    Playing,
    Pause, // CR: Paused je bolje ime, jer je kontinualno - kao sto si nazvala prethodno stanje Playing a ne Play.
    GameOver
}
