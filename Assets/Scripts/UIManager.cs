using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject youLostMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    public Slider HealthSlider;

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
        SetSavedSlidersValues();
    }

    private void SetSavedSlidersValues()
    {
        float savedMusicVolume = PlayerPrefs.GetFloat(AudioManager.MixerMusic, 1.0f);
        float savedSFXVolume = PlayerPrefs.GetFloat(AudioManager.MixerSFX, 1.0f);

        musicSlider.value = savedMusicVolume;
        sfxSlider.value = savedSFXVolume;
        AudioManager.Instance.SetMusicVolume(savedMusicVolume);
        AudioManager.Instance.SetSFXVolume(savedSFXVolume);
    }

    public void SetMusicSlider()
    {
        AudioManager.Instance.SetMusicVolume(musicSlider.value);
    }

    public void SetSFXSlider()
    {
        AudioManager.Instance.SetSFXVolume(sfxSlider.value);
    }

    public void PLayGame()
    {
        GameManager.Instance.PlayGame();
    }

    public void TogglePauseMenu(GameState currentState)
    {
        if (currentState == GameState.Paused)
        {
            pauseMenu.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(false);
        }
    }

    public void HideMainMenu()
    {
        mainMenu.SetActive(false);
    }

    public void UpdateScoreText(int totalScore)
    {
        scoreText.text = "SCORE: " + totalScore;
    }

    public void ToggleGameOverMenu(GameState currentState)
    {
        if (currentState == GameState.GameOver)
        {
            youLostMenu.SetActive(true);
        }
        else
        {
            youLostMenu.SetActive(false);
        }
    }

    public void UpdateHealthSlider(int health)
    {
        HealthSlider.value = health;
    }

    public void LoadGameplayScene()
    {
        GameManager.Instance.LoadGameplayScene();
    }

    public void QuitGame()
    {
        GameManager.Instance.Quit();
    }

    public void PauseGame()
    {
        GameManager.Instance.PauseGame();
    }

    public void ResumeGame()
    {
        GameManager.Instance.PlayGame();
    }

}
