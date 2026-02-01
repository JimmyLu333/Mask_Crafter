using UnityEngine;

public class AudioMmanager : MonoBehaviour
{
    public static AudioMmanager instance;

    [Header("---------- Audio Source ----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("---------- Audio Clip ----------")]
    public AudioClip background;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 确保它不被销毁
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 如果已经存在一个了，销毁当前这个多余的
            return; 
        }
    }

    private void Start()
    {
        // 增加一个判断：如果已经在播放了，就不要打断它
        if (musicSource != null && background != null)
        {
            if (!musicSource.isPlaying || musicSource.clip != background)
            {
                musicSource.clip = background;
                musicSource.Play();
            }
        }
    }
}