using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource coinSound;
    [SerializeField] private AudioSource backgroundMusic;

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

    public void BackgroundMusic() // CR: StartBackgroundMusic (ne Play zato sto kada je jednom pustimo, ona svira u kontinuitetu dok je ne zaustavimo)
    {
        if (!backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }

    public void CoinSound() // CR: PlayCoinSound (ovde je Play jer pustimo zvuk, on se zavrsi sam i gotov je)
    {
        coinSound.Play();
    }
}
