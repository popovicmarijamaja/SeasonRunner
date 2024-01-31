using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private const int PlayerHealthMin = PlayerManager.HealthMinValue;
    private const int PlayerHealthMax = PlayerManager.HealthMaxValue;
    public const float SpeedIncrement = 0.03f;
    private const string GameplayScene = "Gameplay";

    [SerializeField] private GameObject emptyEnvironment;
    [SerializeField] private Transform environmentSpawnPos;
    [SerializeField] private PlayerManager playerManager;

    private int coinScore;
    private float runningScore;
    public int totalScore;
    private float scoreIncrementPerSecond = 2f;
    

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
        ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        SetGameState(GameState.MainMenu);
    }

    private void SetGameState(GameState state)
    {
        CurrentState = state;
        playerManager.SetPlayerAnimation(CurrentState);
        SetEnvironmentMovement();
        if (state == GameState.Playing)
            playerManager.enabled = true;
        else
            playerManager.enabled = false;
    }

    private void Update()
    {
        totalScore = Convert.ToInt32(coinScore + runningScore);
        if (CurrentState == GameState.Playing)
        {
            runningScore += Time.deltaTime * scoreIncrementPerSecond;
        }
        scoreIncrementPerSecond += SpeedIncrement * Time.deltaTime;
        UIManager.Instance.UpdateScoreText(totalScore);
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

    public void EndGame()
    {
        SetGameState(GameState.GameOver);
        StartCoroutine(WaitForEndOfParcticles());
        if (playerManager.health != PlayerHealthMin)
        {
            playerManager.PlayExplosionParticle();
            playerManager.health = PlayerHealthMin;
            UIManager.Instance.UpdateHealthSlider(playerManager.health);
        }
    }

    private IEnumerator WaitForEndOfParcticles()
    {
        yield return new WaitForSeconds(2f);
        UIManager.Instance.ToggleGameOverMenu(CurrentState);
    }

    private void SetEnvironmentMovement()
    {
        bool isPlaying = CurrentState == GameState.Playing;
        emptyEnvironment.GetComponent<EnvironmentController>().enabled = isPlaying;
        foreach (var pooledObject in ObjectPool.Instance.EnvirontmentPool)
            pooledObject.GetComponent<EnvironmentController>().enabled = isPlaying;
    }

    public void CollectCoin()
    {
        PlayCollectSoundAndParticle();
        coinScore += 10;
    }

    public void CollectStar()
    {
        PlayCollectSoundAndParticle();
        coinScore += 50;
    }

    private void PlayCollectSoundAndParticle()
    {
        AudioManager.Instance.PlayCoinSound();
        playerManager.PlayCollectionParticle();
    }

    public void SpawnNewSection() // CR: Ovo pripada u SpawnManageru
    {
        GameObject environment = ObjectPool.Instance.GetEnvironment();
        if (environment != null)
        {
            environment.transform.position = environmentSpawnPos.position;
            environment.SetActive(true);
        }
    }

    public void GetHurt() // CR: Ko biva povredjen? Ne GameManager, to je sigurno :D DamagePlayer bi bio bolji naziv.
    {
        playerManager.DecreaseHealth();
        UIManager.Instance.UpdateHealthSlider(playerManager.health);
        if (playerManager.health == PlayerHealthMin)
        {
            EndGame();
        }
    }
    public void CollectHeart()
    {
        PlayCollectSoundAndParticle();
        if (playerManager.health == PlayerHealthMax)
            return;
        playerManager.IncreaseHealth();
        UIManager.Instance.UpdateHealthSlider(playerManager.health);
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
    Paused,
    GameOver
}
