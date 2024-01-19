using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public const float SpeedIncreasing = 0.03f;
    private const float MagnetLasting = 6f;
    private const string GameplayScene = "Gameplay";
    private const int HealthMaxValue = 4;
    private const int HealthMinValue = 0;

    [SerializeField] private GameObject getReadyEnvironment;
    [SerializeField] private Transform newSectionPos;
    [SerializeField] private PlayerManager playerManager;

    private int coinScore;
    private float runningScore;
    public int totalScore;
    private float runningScoreValue = 2f;
    private bool isMagnetRunning;
    private int health;

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

    public void MainMenu()
    {
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

    public void Play()
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

    public void GameOver()
    {
        SetGameState(GameState.GameOver);
        EnvironmentMoving();
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

    private void EnvironmentMoving()
    {
        bool isPlaying = CurrentState == GameState.Playing;
        getReadyEnvironment.GetComponent<EnvironmentMoving>().enabled = isPlaying;
        foreach (var pooledObject in ObjectPool.Instance.pooledObjectsEnvironment)
            pooledObject.GetComponent<EnvironmentMoving>().enabled = isPlaying;
    }

    public void CoinCollect()
    {
        Collectible();
        coinScore += 10;
    }

    public void StarCollect()
    {
        Collectible();
        coinScore += 50;
    }

    public void MagnetPower()
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

    private void Collectible()
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

    public void HealthDecrease()
    {
        health--;
        UIManager.Instance.HealthSlider(health);
        if (health == HealthMinValue)
        {
            GameOver();
        }
    }
    public void HealthIncrease()
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
    Pause,
    GameOver
}
