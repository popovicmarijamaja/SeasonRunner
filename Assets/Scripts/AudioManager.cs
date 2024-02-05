using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public const string MixerMusic = "MusicVolume";
    public const string MixerSFX = "SFXVolume";
    private const float SliderAdjustmentValue = 20;

    [SerializeField] private AudioSource coinSound;
    [SerializeField] private AudioSource powerUpSound;
    [SerializeField] private AudioSource hurtSound;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource explosionSound;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioMixer masterMixer;

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

    public void SetMusicVolume(float sliderValue)
    {
        masterMixer.SetFloat(MixerMusic, Mathf.Log10(sliderValue) * SliderAdjustmentValue);
        PlayerPrefs.SetFloat(MixerMusic, sliderValue);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float sliderValue)
    {
        masterMixer.SetFloat(MixerSFX, Mathf.Log10(sliderValue) * SliderAdjustmentValue);
        PlayerPrefs.SetFloat(MixerSFX, sliderValue);
        PlayerPrefs.Save();
    }

    public void StartBackgroundMusic()
    {
        if (!backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }

    public void PlayCoinSound()
    {
        coinSound.Play();
    }

    public void PlayPowerUpSound()
    {
        powerUpSound.Play();
    }

    public void PlayHurtSound()
    {
        hurtSound.Play();
    }

    public void PlayExposionSound()
    {
        explosionSound.Play();
    }

    public void PlayJumpSound()
    {
        jumpSound.Play();
    }
}
