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

    public void GetNumberOfPlayers(int number)
    {
        NumberOfPlayers = number;
    }

    public void SetNumberOfPlayers()
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

        AudioListener[] listeners = FindObjectsOfType<AudioListener>();

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


    private void SetGameState(GameState state)
    {
        SetCurrentState(state);
        SetStateForEachPlayer(state);
    }

    private void SetCurrentState(GameState state)
    {
        CurrentState = state;
    }

    private void SetStateForEachPlayer(GameState state)
    {
        PlayerManager[] players = FindObjectsOfType<PlayerManager>();

        foreach (PlayerManager player in players)
        {
            player.SetPlayerState(CurrentState);
            if (state == GameState.Playing)
                player.enabled = true;
            else
                player.enabled = false;
            SetEnvironmentMovement(player);
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

    public void EndGame(PlayerManager playerManager)
    {
        playerManager.SetPlayerState(GameState.GameOver);
        CheckIfAllPlayersAreDead();
        StartCoroutine(WaitForEndOfDeathAnimation());
        if (playerManager.Health == PlayerHealthMin)
            return;
        SetDeathByObstacleHit(playerManager);
        SetEnvironmentMovement(playerManager);
    }


    private void CheckIfAllPlayersAreDead()
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

    private void CheckWhoWon()
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

    private void SetDeathByObstacleHit(PlayerManager playerManager)
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

        if (playerManager.isDead)
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
        playerManager.DecreaseHealth();
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
