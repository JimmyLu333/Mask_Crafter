using UnityEngine;

public class AudioMmanager : MonoBehaviour
{
    public static AudioMmanager instance;

    [Header("---------- Audio Source ----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("---------- Audio Clip ----------")]
    public AudioClip background;

    // 添加公共属性，让其他脚本可以访问
    public AudioSource MusicSource => musicSource;
    public AudioSource SfxSource => SFXSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (musicSource != null && background != null)
        {
            if (!musicSource.isPlaying || musicSource.clip != background)
            {
                musicSource.clip = background;
                musicSource.Play();
            }
        }
    }

    // 添加公共方法，方便其他脚本控制音频
    public void StopBackgroundMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void PauseBackgroundMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void ResumeBackgroundMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.UnPause();
        }
    }

    public void FadeOutBackgroundMusic(float duration)
    {
        StartCoroutine(FadeOutMusicCoroutine(duration));
    }

    private System.Collections.IEnumerator FadeOutMusicCoroutine(float duration)
    {
        if (musicSource == null || !musicSource.isPlaying) yield break;

        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;
    }
}