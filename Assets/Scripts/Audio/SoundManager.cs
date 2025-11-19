using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    
    [Header("BGM")]
    [SerializeField] private AudioClip gameBGM;
    
    [Header("SFX")]
    [SerializeField] private AudioClip kickSound;
    [SerializeField] private AudioClip goalSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip superModeGeodainoSound;
    [SerializeField] private AudioClip superModeLightningmanSound;
    [SerializeField] private AudioClip whistleSound;
    
    [Header("Settings")]
    [SerializeField] private float bgmVolume = 0.5f;
    [SerializeField] private float sfxVolume = 0.7f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;
        
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;
    }
    
    void Start()
    {
        PlayBGM();
    }
    
    public void PlayBGM()
    {
        if (gameBGM != null && bgmSource != null)
        {
            bgmSource.clip = gameBGM;
            bgmSource.Play();
        }
    }
    
    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }
    
    public void PlayKickSound()
    {
        PlaySFX(kickSound);
    }
    
    public void PlayGoalSound()
    {
        PlaySFX(goalSound);
    }
    
    public void PlayDashSound()
    {
        PlaySFX(dashSound);
    }
    
    public void PlaySuperModeSound(CharacterType.Type characterType)
    {
        if (characterType == CharacterType.Type.Geodaino)
        {
            PlaySFX(superModeGeodainoSound);
        }
        else if (characterType == CharacterType.Type.Lightningman)
        {
            PlaySFX(superModeLightningmanSound);
        }
    }
    
    public void PlayWhistleSound()
    {
        PlaySFX(whistleSound);
    }
    
    void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume;
        }
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }
}

