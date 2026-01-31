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
        yield return new WaitForSeconds(2f);

        // === 阶段2：显示开场文字（在黑屏上显示）===
        Debug.Log("阶段2：显示开场文字（黑屏背景）");
        dialoguePanel.SetActive(true);
        speakerText.text = "";
        yield return StartCoroutine(ShowText("It's a rainy afternoon, water droplets slowly tracing lines down the window.", true));
        yield return StartCoroutine(ShowText("There are unfinished orders scattered on the workbench, " +
            "and the bell on the door hasn't rung in a long time.", true));

        // === 阶段3：黑屏淡出，显示背景 ===
        Debug.Log($"阶段3：黑屏淡出，背景逐渐显示（{fadeDuration}秒）");

        // 淡出黑屏，背景就会逐渐显现
        yield return StartCoroutine(FadeBlackScreen(0f));

        Debug.Log("✅ 黑屏淡出完成，背景已完全显示");

        // === 阶段4：主角出现 ===
        Debug.Log("阶段4：主角出现");
        dialoguePanel.SetActive(false);
        yield return new WaitForSeconds(0.2f); // 短暂等待

        playerImage.gameObject.SetActive(true);
        playerImage.color = new Color(1, 1, 1, 0);
        yield return StartCoroutine(FadeImage(playerImage, 2f));
        yield return new WaitForSeconds(0.5f);

        // === 阶段5：主角独白 ===
        Debug.Log("阶段5：主角独白");
        dialoguePanel.SetActive(true);
        speakerText.text = "Me";
        yield return StartCoroutine(ShowText("Haven't had a customer in a long time...", true));
        yield return StartCoroutine(ShowText("Wonder what kind of person will come this time.", true));

        // === 阶段6：NPC出现 ===
        Debug.Log("阶段6：NPC出现");
        npcImage.gameObject.SetActive(true);
        npcImage.color = new Color(1, 1, 1, 0);
        yield return StartCoroutine(FadeImage(npcImage, 2f));
        yield return new WaitForSeconds(0.5f);

        // === 阶段7：对话 ===
        Debug.Log("阶段7：对话开始");
        yield return StartCoroutine(ShowDialogue("阿姨", "Hello, is this the place that does... that thing?"));
        yield return StartCoroutine(ShowDialogue("Me", "Yes. How can I help you?"));
        yield return StartCoroutine(ShowDialogue("阿姨", "My daughter... she's getting married next month."));
        yield return StartCoroutine(ShowText("（She takes out her daughter's wedding photo.）", true));
        yield return StartCoroutine(ShowDialogue("阿姨", "But ever since I was little, after an accident... I lost my nose. So I've always kept it wrapped."));
        yield return StartCoroutine(ShowDialogue("阿姨", "I don't want to wear these bandages at the wedding. And I don't want the guests to see... this face."));
        yield return StartCoroutine(ShowDialogue("阿姨", "My daughter keeps saying we'll take family photos that day..."));
        yield return StartCoroutine(ShowDialogue("阿姨", "I want... to stand next to her. Like a normal mother."));
        yield return StartCoroutine(ShowDialogue("阿姨", "I'm not asking to look pretty... I just want a nose that people can look at."));
        yield return StartCoroutine(ShowDialogue("阿姨", "One that won't make them turn away or whisper. Just let me stand by her side with my head held high. That's enough."));
        yield return StartCoroutine(ShowDialogue("Me", "Then let's make you a nose that can \"face the light.\""));
        yield return StartCoroutine(ShowDialogue("Me", "It won't be fake, and it won't be just covering things up."));
        yield return StartCoroutine(ShowDialogue("Me", "It'll be a complete, natural shape—like the gentle curve of a tree branch in autumn."));
        yield return StartCoroutine(ShowDialogue("阿姨", "Really... can you do that?"));
        yield return StartCoroutine(ShowDialogue("Me", " Of course."));
        yield return StartCoroutine(ShowDialogue("Me", "Look how brightly your daughter is smiling. "));
        yield return StartCoroutine(ShowDialogue("Me", "You should stand next to her—complete, at ease, sharing that light. Not as someone she has to worry about or hide."));
        yield return StartCoroutine(ShowDialogue("Me", "Come next Tuesday for a fitting. Then you can look in the mirror—and see yourself without bandages."));
        yield return StartCoroutine(ShowDialogue("阿姨", "Okay... okay. I'll come next Tuesday."));
        speakerText.text = "";
        yield return StartCoroutine(ShowText("She leaves the bandages on the workbench. As she walks out, her back seems a little straighter. And outside, the rain is finally letting up.", true));



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
        yield return StartCoroutine(ShowText("It is time to work!", false));
        yield return new WaitForSeconds(1f);

        // 淡入黑屏
        yield return StartCoroutine(FadeBlackScreen(1f));
        yield return new WaitForSeconds(0.5f);

        // 切换场景
        SceneManager.LoadScene(nextSceneIndex);
    }
}