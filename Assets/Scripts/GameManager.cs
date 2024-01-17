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
    private const float HealthMaxValue = 4f;
    private const float HealthMinValue = 0f;

    [SerializeField] private GameObject getReadyEnvironment;
    [SerializeField] private Transform newSectionPos;

    private int coinScore;
    private float runningScore;
    public int totalScore;
    private float runningScoreValue = 2f;
    private bool isMagnetRunning;
    private float health;

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
        PlayerManager.Instance.PlayerAnimation(CurrentState);
        PlayerManager.Instance.enabled = false;
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
        PlayerManager.Instance.PlayerAnimation(CurrentState);
        PlayerManager.Instance.enabled = true;
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
        PlayerManager.Instance.PlayerAnimation(CurrentState);
        PlayerManager.Instance.enabled = false;
        UIManager.Instance.PauseMenu(CurrentState);
    }

    public void GameOver()
    {
        SetGameState(GameState.GameOver);
        EnvironmentMoving();
        PlayerManager.Instance.PlayerAnimation(CurrentState);
        PlayerManager.Instance.enabled = false;
        StartCoroutine(WaitForEndOfParcticles());
        if (health != HealthMinValue)
        {
            PlayerManager.Instance.ExplosionParticle();
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
        
        if (CurrentState == GameState.Playing)
        {
            getReadyEnvironment.GetComponent<EnvironmentMoving>().enabled = true;
            foreach (var pooledObject in ObjectPool.Instance.pooledObjects)
                pooledObject.GetComponent<EnvironmentMoving>().enabled = true;
        }
        else
        {
            getReadyEnvironment.GetComponent<EnvironmentMoving>().enabled = false;
            foreach (var pooledObject in ObjectPool.Instance.pooledObjects)
                pooledObject.GetComponent<EnvironmentMoving>().enabled = false;
        }
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
        PlayerManager.Instance.CollectorColider.enabled = true;
        yield return new WaitForSeconds(MagnetLasting);
        PlayerManager.Instance.CollectorColider.enabled = false;
        isMagnetRunning = false;
    }

    private void Collectible()
    {
        AudioManager.Instance.CoinSound();
        PlayerManager.Instance.CollectParticles();
    }

    public void SpawnNewSection()
    {
        GameObject environment = ObjectPool.Instance.GetPooledObject();
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
