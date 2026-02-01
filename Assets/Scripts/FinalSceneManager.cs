using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FinalSceneManager : MonoBehaviour
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

    [Header("阿姨Sprite设置")]
    public Sprite auntNormalSprite;    // 拖入正常Sprite
    public Sprite auntChangedSprite;   // 拖入改变后的Sprite
    public float spriteChangeDuration = 0.8f; // 改变持续时间


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
        yield return StartCoroutine(ShowText("Finished！！！", true));

        playerImage.gameObject.SetActive(true);
        playerImage.color = new Color(1, 1, 1, 0);
        yield return StartCoroutine(FadeImage(playerImage, 2f));
        yield return new WaitForSeconds(0.5f);

        speakerText.text = "Me";
        yield return StartCoroutine(ShowText("I wonder when she will come?", true));

        // === 阶段3：黑屏淡出，显示背景 ===
        Debug.Log($"阶段3：黑屏淡出，背景逐渐显示（{fadeDuration}秒）");

        // 淡出黑屏，背景就会逐渐显现
        yield return StartCoroutine(FadeBlackScreen(0f));

        Debug.Log("✅ 黑屏淡出完成，背景已完全显示");

        

        // === 阶段6：NPC出现 ===
        Debug.Log("阶段6：NPC出现");
        npcImage.gameObject.SetActive(true);
        npcImage.color = new Color(1, 1, 1, 0);
        yield return StartCoroutine(FadeImage(npcImage, 2f));
        yield return new WaitForSeconds(0.5f);

        // === 阶段7：对话 ===
        Debug.Log("阶段7：对话开始");
        yield return StartCoroutine(ShowDialogue("", "The lady pushes the door open, her face still swathed in thick bandages."));
        yield return StartCoroutine(ShowDialogue("阿姨", "Master... I'm here."));
        yield return StartCoroutine(ShowDialogue("Me", "Everything is ready. Please, have a seat."));
        yield return StartCoroutine(ShowDialogue("", "I pick up the custom-made prosthesis and fit it onto her face with a steady, gentle hand. As the final seam blends seamlessly into her skin, I hand her the mirror."));
        
        yield return StartCoroutine(ShowDialogue("Me", "Take a look. Not just at the nose—look at yourself."));

        yield return StartCoroutine(ShowDialogue("", "She stares into the mirror for a long time. Her eyes shift from shock to disbelief, finally settling into a pool of calm water."));
        
        yield return StartCoroutine(ChangeAuntSpriteSimple());

        yield return StartCoroutine(ShowDialogue("阿姨", "Is this... really me? I had almost forgotten what I looked like without the bandages."));
        yield return StartCoroutine(ShowDialogue("Me", "The contour is very natural. At the wedding next month, you won't have to hide in the corners, nor will you have to worry about the wind. You only need to stand by her side and share her smile."));
        yield return StartCoroutine(ShowDialogue("阿姨", "Thank you... thank you so much. Finally, I can walk her down the aisle with dignity."));

        speakerText.text = "";
        yield return StartCoroutine(ShowText("She rises to leave, leaving the pile of old bandages behind on the workbench. Outside, the sky has cleared after the rain, and the sunlight casts her long shadow across the ground.", true));





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
        yield return StartCoroutine(ShowText("Who will come next?", false));
        yield return new WaitForSeconds(1f);

        // 淡入黑屏
        yield return StartCoroutine(FadeBlackScreen(1f));
        yield return new WaitForSeconds(0.5f);

        // 切换场景
        SceneManager.LoadScene(nextSceneIndex);
    }

    IEnumerator ChangeAuntSpriteSimple()
    {
        if (npcImage == null || auntChangedSprite == null)
        {
            Debug.LogError("npcImage或auntChangedSprite未设置");
            yield break;
        }

        Debug.Log("开始改变阿姨Sprite");

        // 淡出效果
        yield return StartCoroutine(FadeImage(npcImage, 0.3f, 0.5f));

        // 更换Sprite
        npcImage.sprite = auntChangedSprite;

        // 淡入效果
        yield return StartCoroutine(FadeImage(npcImage, 1f, 0.5f));

        // 可选：添加一些特效
        yield return StartCoroutine(SparkleEffect());

        Debug.Log("阿姨Sprite改变完成");
    }

    // 带持续时间的淡入淡出方法
    IEnumerator FadeImage(Image image, float targetAlpha, float duration)
    {
        float startAlpha = image.color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            image.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        image.color = new Color(1, 1, 1, targetAlpha);
    }

    IEnumerator SparkleEffect()
    {
        // 简单的位置抖动
        RectTransform rt = npcImage.GetComponent<RectTransform>();
        Vector3 originalPos = rt.anchoredPosition;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float shake = Mathf.Sin(elapsed * 30f) * (1f - elapsed / duration) * 5f;
            rt.anchoredPosition = originalPos + new Vector3(shake, 0, 0);
            yield return null;
        }

        rt.anchoredPosition = originalPos;
    }

}