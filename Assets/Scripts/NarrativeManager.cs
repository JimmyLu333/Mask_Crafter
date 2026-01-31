using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SimpleNarrativeManager : MonoBehaviour
{
    [Header("UI组件")]
    public Image backgroundImage;
    public Image blackBackground;
    public Image playerImage;
    public Image npcImage;
    public GameObject dialoguePanel;
    public Text speakerText;
    public Text dialogueText;

    [Header("设置")]
    public float typingSpeed = 0.05f;
    public float fadeDuration = 1.5f; // 黑屏淡入时间

    [Header("场景设置")]
    public int nextSceneIndex = 2; // 下一个场景的索引
    public float blackScreenHoldTime = 1f; // 黑屏后等待多久切换场景

    private bool isTyping = false;

    void Start()
    {
        InitializeUI();
        StartCoroutine(NarrativeFlow());
    }

    void InitializeUI()
    {
        dialoguePanel.SetActive(false);
        playerImage.gameObject.SetActive(false);
        npcImage.gameObject.SetActive(false);

        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(false);

        if (blackBackground != null)
        {
            blackBackground.gameObject.SetActive(true);
            blackBackground.color = Color.black; // 初始黑屏
        }
    }

    IEnumerator NarrativeFlow()
    {
        // === 1. 开场等待 ===
        yield return new WaitForSeconds(1f);

        // === 2. 显示开场文字 ===
        dialoguePanel.SetActive(true);
        speakerText.text = "";
        yield return StartCoroutine(TypeText("某个寻常的夜晚..."));
        yield return WaitForClickOrTime(2f);

        // === 3. 黑屏淡出，显示背景 ===
        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(true);

        yield return StartCoroutine(FadeBlackScreen(0f)); // 黑屏变透明

        // === 4. 主角出现 ===
        dialoguePanel.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        playerImage.gameObject.SetActive(true);
        playerImage.color = new Color(1, 1, 1, 0);
        yield return StartCoroutine(FadeInImage(playerImage));
        yield return new WaitForSeconds(0.5f);

        // === 5. 主角独白 ===
        dialoguePanel.SetActive(true);
        speakerText.text = "主角";

        yield return StartCoroutine(ShowDialogue("今晚的街道格外安静..."));
        yield return StartCoroutine(ShowDialogue("或许我不该走这条路的..."));

        // === 6. NPC出现 ===
        npcImage.gameObject.SetActive(true);
        npcImage.color = new Color(1, 1, 1, 0);
        yield return StartCoroutine(FadeInImage(npcImage));
        yield return new WaitForSeconds(0.5f);

        // === 7. 对话 ===
        yield return StartCoroutine(ShowDialogueWithSpeaker("神秘人", "这么晚了，一个人在这里做什么？"));
        yield return StartCoroutine(ShowDialogueWithSpeaker("主角", "只是...随便走走。"));
        yield return StartCoroutine(ShowDialogueWithSpeaker("神秘人", "这种时候散步可不是好主意。"));
        yield return StartCoroutine(ShowDialogueWithSpeaker("主角", "你是谁？"));

        // === 8. 结束叙事，切换场景 ===
        yield return StartCoroutine(EndAndTransition());
    }

    IEnumerator EndAndTransition()
    {
        Debug.Log("对话结束，准备切换场景");

        // 8.1 可选：显示结束文字
        dialoguePanel.SetActive(true);
        speakerText.text = "";
        yield return StartCoroutine(TypeText("故事开始了..."));
        yield return new WaitForSeconds(1f);

        // 8.2 黑屏淡入
        Debug.Log("开始黑屏淡入");
        yield return StartCoroutine(FadeBlackScreen(1f));

        // 8.3 在黑屏状态下等待一会儿
        Debug.Log($"黑屏保持 {blackScreenHoldTime} 秒");
        yield return new WaitForSeconds(blackScreenHoldTime);

        // 8.4 切换到下一个场景
        Debug.Log($"切换到场景索引: {nextSceneIndex}");
        LoadNextScene();

        yield break;
    }

    void LoadNextScene()
    {
        if (nextSceneIndex >= 0 && nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogError($"无效的场景索引: {nextSceneIndex}");
            // 或者回到菜单场景
            // SceneManager.LoadScene(0);
        }
    }

    // ========== 辅助方法 ==========

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    IEnumerator WaitForClick()
    {
        yield return new WaitForSeconds(0.1f);

        bool clicked = false;
        while (!clicked)
        {
            if (Input.GetMouseButtonDown(0))
                clicked = true;
            yield return null;
        }
    }

    IEnumerator WaitForClickOrTime(float maxTime)
    {
        float elapsed = 0f;
        bool clicked = false;

        while (elapsed < maxTime && !clicked)
        {
            elapsed += Time.deltaTime;
            if (Input.GetMouseButtonDown(0))
                clicked = true;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
    }

    IEnumerator ShowDialogue(string text)
    {
        yield return StartCoroutine(TypeText(text));
        yield return WaitForClick();
    }

    IEnumerator ShowDialogueWithSpeaker(string speaker, string text)
    {
        speakerText.text = speaker;
        yield return StartCoroutine(ShowDialogue(text));
    }

    IEnumerator FadeInImage(Image image)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            image.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        image.color = Color.white;
    }

    IEnumerator FadeBlackScreen(float targetAlpha)
    {
        float startAlpha = blackBackground.color.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            blackBackground.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        blackBackground.color = new Color(0, 0, 0, targetAlpha);
    }
}