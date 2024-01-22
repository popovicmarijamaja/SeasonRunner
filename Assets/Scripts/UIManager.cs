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
    public Slider healthSlider;

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

    public void PauseMenu(GameState currentState) // CR: Glagol. Bez njega ne znamo po imenu sta ova metoda radi. Je l' pokazuje, krije ili oba? TogglePauseMenu nam to objasnjava.
    {
        if (currentState == GameState.Pause)
        {
            pauseMenu.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(false);
        }
    }

    public void Play() // CR: Ovo ime je cudno u kontekstu ove klase jer ne pokrecemo niti igramo UI manager. HideMainMenu je adekvatnije.
    {
        mainMenu.SetActive(false);
    }

    public void ScoreText(int totalScore) // CR: Glagol. UpdateScoreText
    {
        scoreText.text = "SCORE: " + totalScore;
    }

    public void YouLostMenu(GameState currentState) // CR: ToggleGameOverMenu. Toggle da bi imala glagol, a GameOver je ustaljen izraz cije je znacenje svima poznato pa je bolje da ne izmisljamo novi termin ako on vrsi posao.
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

    public void HealthSlider(int health) // CR: UpdateHealthSlider
    {
        healthSlider.value = health;
    }

    public void PlayButton() // CR: StartGame je ono sto ova metoda radi. To sto se poziva kada se klikne PlayButton nam je manje vazno. Sutra moze da se poziva sa jos 3 mesta i onda trenutni naziv nece biti tacan.
    {
        GameManager.Instance.LoadGameplayScene();
    }

    public void QuitButton() // CR: QuitGame, iz istih razloga.
    {
        GameManager.Instance.Quit();
    }

    public void PauseButton() // CR: PauseGame
    {
        GameManager.Instance.PauseGame();
    }

    public void BackButton() // CR: ResumeGame
    {
        GameManager.Instance.Play();
    }
}
