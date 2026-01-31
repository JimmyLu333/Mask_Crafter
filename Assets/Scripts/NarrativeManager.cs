using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class NarrativeManager : MonoBehaviour
{
    [Header("UI组件")]
    public Image backgroundImage;     // 游戏背景图片
    public Image blackBackground;     // 黑屏遮罩
    public Image playerImage;
    public Image npcImage;
    public GameObject dialoguePanel;
    public Text speakerText;
    public Text dialogueText;

    [Header("设置")]
    public float typingSpeed = 0.05f;
    public float fadeDuration = 2f;
    public int nextSceneIndex = 2;

    void Start()
    {
        Debug.Log("叙事场景开始");

        // 关键：先显示背景，再设置黑屏
        InitializeUI();

        StartCoroutine(NarrativeSequence());
    }

    void InitializeUI()
    {
        // 第一步：先显示背景图片（但被黑屏遮住）
        if (backgroundImage != null)
        {
            backgroundImage.gameObject.SetActive(true);
            backgroundImage.color = Color.white; // 确保正常显示
            Debug.Log("✅ 背景图片已激活");
        }

        // 第二步：设置其他UI为隐藏
        dialoguePanel.SetActive(false);
        playerImage.gameObject.SetActive(false);
        npcImage.gameObject.SetActive(false);

        // 第三步：确保黑屏在最上层并完全遮盖
        if (blackBackground != null)
        {
            blackBackground.transform.SetAsLastSibling(); // 放到最上层
            blackBackground.gameObject.SetActive(true);
            blackBackground.color = Color.black; // 完全黑屏
            Debug.Log("✅ 黑屏设置完成");
        }
    }

    IEnumerator NarrativeSequence()
    {
        // === 阶段1：初始黑屏等待 ===
        Debug.Log("阶段1：初始黑屏等待1秒");
        yield return new WaitForSeconds(1f);

        // === 阶段2：显示开场文字（在黑屏上显示）===
        Debug.Log("阶段2：显示开场文字（黑屏背景）");
        dialoguePanel.SetActive(true);
        speakerText.text = "";
        yield return StartCoroutine(ShowText("某个寻常的夜晚...", true));

        // === 阶段3：黑屏淡出，显示背景 ===
        Debug.Log($"阶段3：黑屏淡出，背景逐渐显示（{fadeDuration}秒）");

        // 淡出黑屏，背景就会逐渐显现
        yield return StartCoroutine(FadeBlackScreen(0f));

        Debug.Log("✅ 黑屏淡出完成，背景已完全显示");

        // === 阶段4：主角出现 ===
        Debug.Log("阶段4：主角出现");
        dialoguePanel.SetActive(false);
        yield return new WaitForSeconds(0.5f); // 短暂等待

        playerImage.gameObject.SetActive(true);
        playerImage.color = new Color(1, 1, 1, 0);
        yield return StartCoroutine(FadeImage(playerImage, 1f));
        yield return new WaitForSeconds(0.5f);

        // === 阶段5：主角独白 ===
        Debug.Log("阶段5：主角独白");
        dialoguePanel.SetActive(true);
        speakerText.text = "主角";
        yield return StartCoroutine(ShowText("今晚的街道格外安静...", true));
        yield return StartCoroutine(ShowText("或许我不该走这条路的...", true));

        // === 阶段6：NPC出现 ===
        Debug.Log("阶段6：NPC出现");
        npcImage.gameObject.SetActive(true);
        npcImage.color = new Color(1, 1, 1, 0);
        yield return StartCoroutine(FadeImage(npcImage, 1f));
        yield return new WaitForSeconds(0.5f);

        // === 阶段7：对话 ===
        Debug.Log("阶段7：对话开始");
        yield return StartCoroutine(ShowDialogue("神秘人", "这么晚了，一个人在这里做什么？"));
        yield return StartCoroutine(ShowDialogue("主角", "只是...随便走走。"));
        yield return StartCoroutine(ShowDialogue("神秘人", "这种时候散步可不是好主意。"));
        yield return StartCoroutine(ShowDialogue("主角", "你是谁？"));

        // === 阶段8：结束叙事 ===
        Debug.Log("阶段8：结束叙事");
        yield return StartCoroutine(EndNarrative());
    }

    IEnumerator FadeBlackScreen(float targetAlpha)
    {
        if (blackBackground == null) yield break;

        float startAlpha = blackBackground.color.a;
        float elapsed = 0f;

        Debug.Log($"开始Fade: {startAlpha} → {targetAlpha}");

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            blackBackground.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        blackBackground.color = new Color(0, 0, 0, targetAlpha);

        // 如果完全透明，可以放到下层（可选）
        if (targetAlpha == 0)
        {
            blackBackground.transform.SetAsFirstSibling();
        }
    }

    IEnumerator ShowText(string text, bool waitForClick = true)
    {
        dialogueText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (waitForClick)
        {
            yield return StartCoroutine(WaitForClick());
        }
    }

    IEnumerator ShowDialogue(string speaker, string text)
    {
        speakerText.text = speaker;
        yield return StartCoroutine(ShowText(text, true));
    }

    IEnumerator FadeImage(Image image, float targetAlpha)
    {
        if (image == null) yield break;

        float startAlpha = image.color.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            image.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        image.color = new Color(1, 1, 1, targetAlpha);
    }

    IEnumerator WaitForClick()
    {
        yield return new WaitForSeconds(0.1f);

        bool clicked = false;
        while (!clicked)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                clicked = true;
            }
            yield return null;
        }
    }

    IEnumerator EndNarrative()
    {
        // 显示结束文字
        dialoguePanel.SetActive(true);
        speakerText.text = "";
        yield return StartCoroutine(ShowText("故事开始了...", false));
        yield return new WaitForSeconds(1f);

        // 淡入黑屏
        yield return StartCoroutine(FadeBlackScreen(1f));
        yield return new WaitForSeconds(0.5f);

        // 切换场景
        SceneManager.LoadScene(nextSceneIndex);
    }
}