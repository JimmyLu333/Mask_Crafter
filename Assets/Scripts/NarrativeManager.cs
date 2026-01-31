using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NarrativeManager : MonoBehaviour
{
    [Header("UI组件")]
    public Text speakerText;
    public Text dialogueText;
    public Image playerImage;
    public Image npcImage;
    public GameObject dialoguePanel;
    public Image blackBackground; // 黑屏遮罩
    public Image backgroundImage; // 游戏背景图片

    [Header("设置")]
    public float typingSpeed = 0.05f;
    public float fadeDuration = 1f;

    private bool isTyping = false;
    private Coroutine currentCoroutine;

    void Start()
    {
        InitializeScene();
        StartCoroutine(NarrativeFlow());
    }

    void InitializeScene()
    {
        // 初始状态
        dialoguePanel.SetActive(false);
        playerImage.gameObject.SetActive(false);
        npcImage.gameObject.SetActive(false);

        // 确保背景图片存在
        if (backgroundImage != null)
        {
            backgroundImage.gameObject.SetActive(false); // 先隐藏背景
        }

        // 初始为黑屏
        if (blackBackground != null)
        {
            blackBackground.color = Color.black;
            blackBackground.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        // 点击控制
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // 跳过打字
                StopAllCoroutines();
                isTyping = false;
            }
        }
    }

    IEnumerator NarrativeFlow()
    {
        // === 第1阶段：开场黑屏 ===
        Debug.Log("阶段1: 开场黑屏");
        yield return new WaitForSeconds(1f);

        // === 第2阶段：显示开场文字 ===
        Debug.Log("阶段2: 显示开场文字");
        dialoguePanel.SetActive(true);
        speakerText.text = "";
        yield return StartCoroutine(TypeText("某个寻常的夜晚..."));
        yield return new WaitForSeconds(1.5f);

        // === 第3阶段：淡出黑屏，显示背景 ===
        Debug.Log("阶段3: 淡出黑屏，显示背景");
        yield return StartCoroutine(FadeOutBlackScreen());

        // === 第4阶段：右边主角出现 ===
        Debug.Log("阶段4: 主角出现");
        playerImage.gameObject.SetActive(true);
        playerImage.color = new Color(1, 1, 1, 0);
        yield return StartCoroutine(FadeInImage(playerImage, 1f));
        yield return new WaitForSeconds(0.5f);

        // === 第5阶段：主角独白 ===
        Debug.Log("阶段5: 主角独白");
        speakerText.text = "主角";
        yield return StartCoroutine(ShowDialogue("今晚的街道格外安静..."));
        yield return StartCoroutine(ShowDialogue("或许我不该走这条路的..."));

        // === 第6阶段：左边NPC出现 ===
        Debug.Log("阶段6: NPC出现");
        npcImage.gameObject.SetActive(true);
        npcImage.color = new Color(1, 1, 1, 0);
        yield return StartCoroutine(FadeInImage(npcImage, 1f));
        yield return new WaitForSeconds(0.5f);

        // === 第7阶段：对话开始 ===
        Debug.Log("阶段7: 对话开始");
        yield return StartCoroutine(ShowDialogueWithSpeaker("神秘人", "这么晚了，一个人在这里做什么？"));
        yield return StartCoroutine(ShowDialogueWithSpeaker("主角", "只是...随便走走。"));
        yield return StartCoroutine(ShowDialogueWithSpeaker("神秘人", "这种时候散步可不是好主意。"));
        yield return StartCoroutine(ShowDialogueWithSpeaker("主角", "你是谁？"));

        // === 第8阶段：结束 ===
        Debug.Log("叙事结束");
        yield return StartCoroutine(FadeToBlack());

        // 这里可以跳转到下一个场景
        Debug.Log("准备加载下一个场景...");
    }

    // 淡出黑屏（显示背景）
    IEnumerator FadeOutBlackScreen()
    {
        // 先显示背景图片
        if (backgroundImage != null)
        {
            backgroundImage.gameObject.SetActive(true);
        }

        // 逐渐将黑屏变为透明
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            blackBackground.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // 完全透明后，可以禁用黑屏对象（可选）
        // blackBackground.gameObject.SetActive(false);
    }

    // 淡入到黑屏
    IEnumerator FadeToBlack()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            blackBackground.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    // 打字机效果
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

    // 显示对话（等待点击）
    IEnumerator ShowDialogue(string text)
    {
        yield return StartCoroutine(TypeText(text));
        yield return WaitForClick();
    }

    // 显示带说话者的对话
    IEnumerator ShowDialogueWithSpeaker(string speaker, string text)
    {
        speakerText.text = speaker;
        yield return StartCoroutine(ShowDialogue(text));
    }

    // 淡入图片
    IEnumerator FadeInImage(Image image, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsed / duration);
            image.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
    }

    // 等待点击
    IEnumerator WaitForClick()
    {
        yield return new WaitForSeconds(0.2f); // 防止连续点击

        bool clicked = false;
        while (!clicked)
        {
            if (Input.GetMouseButtonDown(0))
            {
                clicked = true;
            }
            yield return null;
        }
    }
}