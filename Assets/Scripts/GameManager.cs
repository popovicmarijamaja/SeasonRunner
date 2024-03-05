using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private static bool isPlayAgain;
    private static int numberOfPlayers;

    private const int PlayerHealthMin = PlayerManager.HealthMinValue;
    private const int PlayerHealthMax = PlayerManager.HealthMaxValue;
    private const int ScoreIncrementByCoin = 10;
    private const int ScoreIncrementByStar = 50;
    private const float DeathAimationDuration = 2f;
    public const float SpeedIncrement = 0.03f;
    private const string GameplayScene = "Gameplay";

    [SerializeField] private GameObject stagePrefab;

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
            InitializeGame();
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
        NumberOfPlayers = 1;
    }

    public int NumberOfPlayers
    {
        get { return numberOfPlayers; }
        set { numberOfPlayers = value; }
    }

    public void InitializeGame()
    {
        //InstantiatePlayers();
        //FindObjectOfType<Camera>().enabled = false;
        CameraManager.Instance.SetCamera();
        UIManager.Instance.HideMainMenu();
        PlayGame();
    }

    /*private void InstantiatePlayers()
    {
        for(int i = 1; i < numberOfPlayers; i++)
        {
            GameObject obj = Instantiate(stagePrefab);
            obj.transform.position = new Vector3(0f, 0f, (i * -50f));
            obj.transform.GetComponentInChildren<PlayerManager>().SetControlScheme(i);
        }
    }*/

    private void SetIsPlayAgainBool(bool value)
    {
        isPlayAgain = value;
    }


    public void SetGameState(GameState newState)
	{
        SetCurrentState(newState);
        SetStateForEachPlayer(newState);
    }

    private void SetCurrentState(GameState newState)
	{
        CurrentState = newState;
    }

    private void SetStateForEachPlayer(GameState newState)
	{
        PlayerManager[] players = FindObjectsOfType<PlayerManager>();
        print(players.Length);
        foreach (PlayerManager player in players)
        {
            player.SetPlayerState(newState);
            if (newState == GameState.Playing)
                player.enabled = true;
            else
                player.enabled = false;
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

    public void EndGameForPlayer(PlayerManager player)
    {
        player.SetPlayerState(GameState.GameOver);
        if (AreAllPlayersDead())
        {
            SetGameState(GameState.GameOver);
            SetWinnerUi();
        }
        StartCoroutine(WaitForEndOfDeathAnimation());
    }


    private bool AreAllPlayersDead()
    {
        PlayerManager[] players = FindObjectsOfType<PlayerManager>();

        foreach (PlayerManager player in players)
        {
            if (!player.IsDead)
            {
                return false;
            }
        }
        return true;
    }

    private void SetWinnerUi()
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

    public void SetDeathByObstacleHit(PlayerManager playerManager)
    {
        EndGameForPlayer(playerManager);
        SetDeathByObstacleEffects(playerManager);
        SetPlayerHeathToZero(playerManager);
    }

    private void SetPlayerHeathToZero(PlayerManager playerManager)
    {
        playerManager.Health = PlayerHealthMin;
        UIManager.Instance.UpdateHealthSlider(playerManager.Health, playerManager.HealthSlider);
    }

    private void SetDeathByObstacleEffects(PlayerManager playerManager)
    {
        playerManager.PlayExplosionParticle();
        AudioManager.Instance.PlayExposionSound();
        AudioManager.Instance.PlayHurtSound();
    }

    private IEnumerator WaitForEndOfDeathAnimation()
    {
        yield return new WaitForSeconds(DeathAimationDuration);
        UIManager.Instance.ToggleGameOverMenu(CurrentState);
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
        playerManager.CoinScore += value;
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
        playerManager.DecreaseHealth(); 
        UIManager.Instance.UpdateHealthSlider(playerManager.Health, playerManager.HealthSlider);
        AudioManager.Instance.PlayHurtSound();
        if (playerManager.Health == PlayerHealthMin)
        {
            EndGameForPlayer(playerManager);
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
