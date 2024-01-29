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
