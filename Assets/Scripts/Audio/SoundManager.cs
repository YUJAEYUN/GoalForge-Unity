using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM")]
    [SerializeField] private AudioClip gameBGM;

    [Header("Common SFX")]
    [SerializeField] private AudioClip kickSound;
    [SerializeField] private AudioClip goalSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip itemPickupSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip whistleSound;

    [Header("Legacy Super Mode SFX (Geodaino / Lightningman)")]
    [SerializeField] private AudioClip superModeGeodainoSound;
    [SerializeField] private AudioClip superModeLightningmanSound;

    [Header("Super Mode SFX (Real Players)")]
    [SerializeField] private AudioClip sonSuperSound;
    [SerializeField] private AudioClip ronaldoSuperSound;
    [SerializeField] private AudioClip messiSuperSound;
    [SerializeField] private AudioClip haalandSuperSound;

    [Header("Special Action SFX")]
    [SerializeField] private AudioClip messiAttachSound;
    [SerializeField] private AudioClip haalandPowerShotKickSound;

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

    public void PlayButtonClickSound()
    {
        PlaySFX(buttonClickSound);
    }

    public void PlayItemPickupSound()
    {
        PlaySFX(itemPickupSound);
    }

    public void PlayDashSound()
    {
        PlaySFX(dashSound);
    }

    public void PlayWhistleSound()
    {
        PlaySFX(whistleSound);
    }

    // ───────── 기존 캐릭터(Geodaino / Lightningman) ─────────
    public void PlayGeodainoSuperSound()
    {
        PlaySFX(superModeGeodainoSound);
    }

    public void PlayLightningmanSuperSound()
    {
        PlaySFX(superModeLightningmanSound);
    }

    // ───────── 손 / 호날두 / 메시 / 홀란드 궁극기 발동 사운드 ─────────
    public void PlaySonSuperSound()
    {
        PlaySFX(sonSuperSound);
    }

    public void PlayRonaldoSuperSound()
    {
        PlaySFX(ronaldoSuperSound);
    }

    public void PlayMessiSuperSound()
    {
        PlaySFX(messiSuperSound);
    }

    public void PlayHaalandSuperSound()
    {
        PlaySFX(haalandSuperSound);
    }

    // ───────── 메시 / 홀란드 특수 상황 사운드 ─────────
    public void PlayMessiAttachSound()
    {
        PlaySFX(messiAttachSound);
    }

    public void PlayHaalandPowerShotKickSound()
    {
        PlaySFX(haalandPowerShotKickSound);
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
