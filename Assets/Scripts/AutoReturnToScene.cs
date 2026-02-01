using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AutoReturnToScene : MonoBehaviour
{
    [Header("场景设置")]
    public string returnSceneName = "MainMenu"; // 要返回的场景名称
    public float delaySeconds = 5.0f;           // 延迟秒数

    [Header("UI显示（可选）")]
    public UnityEngine.UI.Text countdownText;  // 倒计时文本
    public GameObject countdownPanel;          // 倒计时面板

    void Start()
    {
        StartCoroutine(ReturnAfterDelay());
        if (AudioMmanager.instance != null)
        {
            AudioMmanager.instance.StopBackgroundMusic();
            // 或者淡出
            AudioMmanager.instance.FadeOutBackgroundMusic(1.0f);
        }
    }


    IEnumerator ReturnAfterDelay()
    {
        // 显示倒计时面板
        if (countdownPanel != null)
            countdownPanel.SetActive(true);

        // 倒计时
        for (int i = (int)delaySeconds; i > 0; i--)
        {
            // 更新倒计时文本
            if (countdownText != null)
                countdownText.text = $"返回主菜单... {i}秒";

            yield return new WaitForSeconds(1);
        }

        // 返回场景
        ReturnToTargetScene();
    }

    public void ReturnToTargetScene()
    {
        if (!string.IsNullOrEmpty(returnSceneName))
        {
            SceneManager.LoadScene(returnSceneName);
        }
        else
        {
            Debug.LogError("场景名称未设置！");
        }
    }

    // 允许玩家跳过等待，直接返回
    public void SkipAndReturn()
    {
        StopAllCoroutines();
        ReturnToTargetScene();
    }
}