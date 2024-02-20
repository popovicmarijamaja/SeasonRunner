using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private static bool isPlayAgain;
    public static int NumberOfPlayers;

    private const int PlayerHealthMin = PlayerManager.HealthMinValue;
    private const int PlayerHealthMax = PlayerManager.HealthMaxValue;
    private const int ScoreIncrementByCoin = 10;
    private const int ScoreIncrementByStar = 50;
    private const float DeathAimationDuration = 2f;
    public const float SpeedIncrement = 0.03f;
    private const string GameplayScene = "Gameplay";

    [SerializeField] private GameObject stagePrefab;
    
	// CR: Obrisi stvari koje ti vise ne trebaju. Ovakvi komentari su mrtav kod. A ako ti zatreba nekad da vratis taj kod (sto kontam da ovde nije slucaj), git svakako pamti istoriju.
    //public int NumberOfPlayers; 
    

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
        if (isPlayAgain)
        {
            PlayGame();
            SetNumberOfPlayers();
            SetIsPlayAgainBool(false);
        }
        else
        {
            ShowMainMenu();
        }
    }

    private void ShowMainMenu()
    {
        SetGameState(GameState.MainMenu);
        GetNumberOfPlayers(1);
    }

    public void GetNumberOfPlayers(int number) // CR: Get se koristi za funkcije koje vracaju nesto (u ovom slucaju broj igraca, sto bi znacilo da je povratna vrednost int, a ne void). Ono sto tebi ovde treba je Set. Ali i to je bolje da resis preko set property-ja, ne mora da bude metoda.
    {
        NumberOfPlayers = number;
    }

    public void SetNumberOfPlayers() // CR: Ovo nije Set funkcija - ime joj je pogresno. Ovo je vise nekakav InitializeGame ili nesto slicno.
    {
        InstantiatePlayers();
        CameraManager.Instance.SetCamera(NumberOfPlayers);
        PlayGame();
    }

    private void InstantiatePlayers()
    {
        for(int i = 1; i < NumberOfPlayers; i++)
        {
            GameObject obj = Instantiate(stagePrefab);
            obj.transform.position = new Vector3(0f, 0f, (i * -50f));
            obj.transform.GetComponentInChildren<PlayerManager>().SetControlScheme(i);
        }

        AudioListener[] listeners = FindObjectsOfType<AudioListener>(); // Bolje je da player prefab ne sadrzi Listener pa da ga naknadno dodas jednom igracu, nego da ih instanciras N pa da brieses sve osim jednog.

        // If there is more than one AudioListener, remove the extras
        if (listeners.Length > 1)
        {
            for (int i = 1; i < listeners.Length; i++)
            {
                Destroy(listeners[i]);
            }
        }
    }

    private void SetIsPlayAgainBool(bool value)
    {
        isPlayAgain = value;
    }


    private void SetGameState(GameState state) // CR: Dobro ime za ovaj parametar je newState
	{
        SetCurrentState(state);
        SetStateForEachPlayer(state);
    }

    private void SetCurrentState(GameState state) // CR: Dobro ime za ovaj parametar je newState
	{
        CurrentState = state;
    }

    private void SetStateForEachPlayer(GameState state) // CR: Dobro ime za ovaj parametar je newState
	{
        PlayerManager[] players = FindObjectsOfType<PlayerManager>();

        foreach (PlayerManager player in players)
        {
            player.SetPlayerState(CurrentState); // CR: Posto si vec prosledila newState kao parametar, iskoristi njega radije nego field CurrentState. Tacno je da ce ovde CurrentState sigurno da bude dobro postavljen, ali si ovako napravila implicitnu zavisnost - SetCurrentState mora da se pozove pre SetStateForEachPlayer. Kad bi koristila parametar, kod bi ti bio eksplicitan, otporan na promenu redosleda poziva u buducnosti, i plus je funkcija citljiva sama za sebe (trenutno moram da imam siri kontekst sta se poziva pre nje da bih razumeo celu funkcionalnost ove metode).
            if (state == GameState.Playing)
                player.enabled = true;
            else
                player.enabled = false;
            SetEnvironmentMovement(player); // CR: Na oba mesta gde pozivas ovu funkciju, pozivas je ubrzo nakon SetPlayerState. To je znak da bi ovo trebalo da bude deo SetPlayerState operacije (da se poziva unutar te funkcije).
        }
    }

    public void PlayGame()
    {
        SetGameState(GameState.Playing);
        UIManager.Instance.TogglePauseMenu(CurrentState);
        UIManager.Instance.HideMainMenu();
        AudioManager.Instance.StartBackgroundMusic();
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.GameOver)
            return;
        
        SetGameState(GameState.Paused);
        UIManager.Instance.TogglePauseMenu(CurrentState);
    }

    public void EndGame(PlayerManager playerManager) // CR: Ovo vise nije EndGame. EndGameForPlayer je sada tacniji naziv. Mozda bih ja parametar ovde nazvao samo player radije nego playerManager, ali to je stvar preference.
    {
        playerManager.SetPlayerState(GameState.GameOver);
        CheckIfAllPlayersAreDead();
        StartCoroutine(WaitForEndOfDeathAnimation());
        if (playerManager.Health == PlayerHealthMin) // CR: Ovo mi smrdi. U sledecoj funkciji postavljas health na 0. Ako pre toga moras da bitas da li je health 0, to znaci da imas vise mesta gde moze da se postavi na 0, sto je lose.
            return;
        SetDeathByObstacleHit(playerManager);
        SetEnvironmentMovement(playerManager);
    }


    private void CheckIfAllPlayersAreDead() // CR: Ova funkcija radi vise od provere - ona zavrsava igru ukoliko su svi mrtvi. EndGameIfAllPlayersAreDead bi bilo bolje ime. Potencijalno je jos bolje da imas zasebnu bool funkciju AreAllPlayersDead, pa da u EndGame kazes tipa if (AreAllPlayersDead()) { EndGame(); }, gde bi EndGame bile zadnje dve linije ove funkcije.
    {
        PlayerManager[] players = FindObjectsOfType<PlayerManager>();

        // Check if any player is alive
        foreach (PlayerManager player in players)
        {
            if (!player.isDead)
            {
                return;
            }
        }
        SetGameState(GameState.GameOver);
        CheckWhoWon();
    }

    private void CheckWhoWon() // CR: I ova funkcija radi vise od provere. Mozda nesto tipa SetWinnerUi
    {
        PlayerManager[] players = FindObjectsOfType<PlayerManager>();
        if (players.Length == 1)
            return;
        int bestScore = 0;
        int bestPlayer = 0;

        for(int i = 0; i < players.Length; i++)
        {
            if (bestScore < players[i].TotalScore)
            {
                bestScore = players[i].TotalScore;
                bestPlayer = i + 1;
            }
        }
        UIManager.Instance.SetWinnerText(bestPlayer);
    }

    private void SetDeathByObstacleHit(PlayerManager playerManager) // CR: Ja bih ovo podelio u dve funkcije: jedna koja sredjuje health i jedna koja se bavi efektima. Onda svaka moze da ima ilustrativno ime.
    {
        playerManager.PlayExplosionParticle();
        playerManager.Health = PlayerHealthMin;
        UIManager.Instance.UpdateHealthSlider(playerManager.Health, playerManager.HealthSlider);
        AudioManager.Instance.PlayExposionSound();
        AudioManager.Instance.PlayHurtSound();
    }

    private IEnumerator WaitForEndOfDeathAnimation()
    {
        yield return new WaitForSeconds(DeathAimationDuration);
        UIManager.Instance.ToggleGameOverMenu(CurrentState);
    }

    private void SetEnvironmentMovement(PlayerManager playerManager)
    {
        bool isPlaying;
        Transform playerParent = playerManager.gameObject.transform.parent;

        if (playerManager.isDead) // CR: isDead je suvisan field - PlayerManager ima svoje stanje. Mogli bismo prosto da pitamo da li je njegov state GameOver, zar ne?
            isPlaying = false;
        else
            isPlaying = CurrentState == GameState.Playing;

        playerParent.GetComponentInChildren<EnvironmentController>().enabled = isPlaying;

        foreach (var pooledObject in ObjectPool.Instance.EnvirontmentPool)
            if (playerParent == pooledObject.transform.parent)
                pooledObject.GetComponent<EnvironmentController>().enabled = isPlaying;
    }

    public void CollectCoin(PlayerManager playerManager)
    {
        PlayCoinCollectSoundAndParticle(playerManager);
        IncreaseCoinScore(ScoreIncrementByCoin, playerManager);
    }

    public void CollectStar(PlayerManager playerManager)
    {
        PlayCoinCollectSoundAndParticle(playerManager);
        IncreaseCoinScore(ScoreIncrementByStar, playerManager);
    }

    private void IncreaseCoinScore(int value, PlayerManager playerManager)
    {
        playerManager.coinScore += value;
    }

    public void PlayPowerUpSoundAndParticle(PlayerManager playerManager)
    {
        AudioManager.Instance.PlayPowerUpSound();
        playerManager.PlayCollectionParticle();
    }

    public void PlayCoinCollectSoundAndParticle(PlayerManager playerManager)
    {
        AudioManager.Instance.PlayCoinSound();
        playerManager.PlayCollectionParticle();
    }

    public void DamagePlayer(PlayerManager playerManager)
    {
        playerManager.DecreaseHealth(); // CR: Evo potvrde da se health na vise mesta postavlja na 0. Probaj da svedes da bude samo na jednom mestu. Ovde verovatno treba da postoji, ono u EndGame mi je sumnjivo.
        UIManager.Instance.UpdateHealthSlider(playerManager.Health, playerManager.HealthSlider);
        AudioManager.Instance.PlayHurtSound();
        if (playerManager.Health == PlayerHealthMin)
        {
            EndGame(playerManager);
        }
    }

    public void CollectHeart(PlayerManager playerManager)
    {
        PlayPowerUpSoundAndParticle(playerManager);
        if (playerManager.Health == PlayerHealthMax)
            return;
        playerManager.IncreaseHealth();
        UIManager.Instance.UpdateHealthSlider(playerManager.Health, playerManager.HealthSlider);
    }


    public void LoadGameplayScene()
    {
        SceneManager.LoadScene(GameplayScene);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void PlayAgain()
    {
        SetIsPlayAgainBool(true);
        LoadGameplayScene();
    }

}
public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}
