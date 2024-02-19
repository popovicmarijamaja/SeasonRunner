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
    [SerializeField] private GameObject audioMenu;
    [SerializeField] private GameObject commandsMenu;
    [SerializeField] private GameObject playMenu;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Dropdown numberOfPlayersDropdown;
    [SerializeField] private TMP_Text winnerText;

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
        SetListenerForDropdown();
    }

    private void SetListenerForDropdown()
    {
        if (numberOfPlayersDropdown != null)
        {
            numberOfPlayersDropdown.onValueChanged.AddListener(delegate {
                ChangeNumberOfPlayers(numberOfPlayersDropdown);
            });
        }
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
        playMenu.SetActive(false);
        GameManager.Instance.SetNumberOfPlayers();
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

    public void UpdateScoreText(int totalScore, TextMeshProUGUI scoreText)
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

    public void SetWinnerText(int winner)
    {
        winnerText.text = "Player " + winner + " has won!";
    }

    public void UpdateHealthSlider(int health, Slider healthSlider)
    {
        healthSlider.value = health;
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

    public void ToggleAudioMenu()
    {
        if(audioMenu.activeSelf)
            audioMenu.SetActive(false);
        else
            audioMenu.SetActive(true);
    }
    public void ToggleCommandsMenu()
    {
        if (commandsMenu.activeSelf)
            commandsMenu.SetActive(false);
        else
            commandsMenu.SetActive(true);
    }

    public void PlayAgain()
    {
        GameManager.Instance.PlayAgain();
    }

    public void TogglePlayMenu()
    {
        if (playMenu.activeSelf)
            playMenu.SetActive(false);
        else
            playMenu.SetActive(true);
    }

    public void ChangeNumberOfPlayers(TMP_Dropdown numberOfPlayersDropdown)
    {
        int numberOfPlayers = int.Parse(numberOfPlayersDropdown.captionText.text);
        GameManager.Instance.GetNumberOfPlayers(numberOfPlayers);
    }

}
