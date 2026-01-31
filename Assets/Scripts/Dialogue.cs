using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Dialogue : MonoBehaviour
{
    [Header("UI组件")]
    public Image backgroundImage;     // 背景图片
    public Image blackScreen;         // 黑屏遮罩
    public GameObject dialoguePanel;  // 对话框面板
    public Text speakerText;          // 说话者名字
    public Text dialogueText;         // 对话内容

    [Header("角色显示")]
    public Image leftCharacter;       // 左边角色
    public Image rightCharacter;      // 右边角色

    [Header("对话设置")]
    public float typingSpeed = 0.05f; // 打字速度
    public float fadeDuration = 1f;   // 淡入淡出时间

    [Header("对话数据")]
    [TextArea(3, 10)]
    public string[] dialogueLines;    // 对话内容数组

    // 私有变量
    private int currentLine = 0;
    private bool isTyping = false;
    private bool canContinue = true;

    void Start()
    {
        Debug.Log("对话场景开始");
        InitializeScene();
        StartCoroutine(DialogueSequence());
    }

    void InitializeScene()
    {
        // 隐藏所有UI元素
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (leftCharacter != null)
            leftCharacter.gameObject.SetActive(false);

        if (rightCharacter != null)
            rightCharacter.gameObject.SetActive(false);

        // 设置初始黑屏
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            blackScreen.color = Color.black;
        }

        // 显示背景
        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(true);
    }

    void Update()
    {
        // 点击控制
        if (Input.GetMouseButtonDown(0) && canContinue)
        {
            if (isTyping)
            {
                // 跳过打字
                SkipTyping();
            }
            else
            {
                // 继续下一句
                ContinueDialogue();
            }
        }
    }

    IEnumerator DialogueSequence()
    {
        // 开场淡入
        yield return new WaitForSeconds(1f);

        // 淡出黑屏
        if (blackScreen != null)
        {
            yield return StartCoroutine(FadeScreen(0f));
        }

        // 开始对话
        yield return StartCoroutine(StartDialogue());
    }

    IEnumerator StartDialogue()
    {
        // 等待一下
        yield return new WaitForSeconds(0.5f);

        // 显示对话框
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        // 显示角色（如果需要）
        if (leftCharacter != null)
        {
            leftCharacter.gameObject.SetActive(true);
            yield return StartCoroutine(FadeInCharacter(leftCharacter));
        }

        // 开始显示对话
        while (currentLine < dialogueLines.Length)
        {
            yield return StartCoroutine(DisplayDialogueLine(dialogueLines[currentLine]));
            currentLine++;
        }

        // 对话结束
        yield return StartCoroutine(EndDialogue());
    }

    IEnumerator DisplayDialogueLine(string line)
    {
        // 解析对话行格式：说话者:内容
        string speaker = "";
        string content = line;

        if (line.Contains(":"))
        {
            string[] parts = line.Split(':');
            speaker = parts[0];
            content = parts[1];
        }

        // 设置说话者
        if (speakerText != null)
            speakerText.text = speaker;

        // 打字机效果显示内容
        yield return StartCoroutine(TypeText(content));

        // 等待点击继续
        yield return StartCoroutine(WaitForClick());
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;

        if (dialogueText != null)
        {
            dialogueText.text = "";

            foreach (char letter in text.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        isTyping = false;
    }

    void SkipTyping()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            isTyping = false;

            // 显示完整文本
            if (dialogueText != null && currentLine < dialogueLines.Length)
            {
                string line = dialogueLines[currentLine];
                if (line.Contains(":"))
                {
                    dialogueText.text = line.Split(':')[1];
                }
                else
                {
                    dialogueText.text = line;
                }
            }
        }
    }

    void ContinueDialogue()
    {
        // 这个方法被Update调用
        // 实际逻辑在协程中
    }

    IEnumerator WaitForClick()
    {
        canContinue = false;
        yield return new WaitForSeconds(0.1f); // 防误触

        bool clicked = false;
        while (!clicked)
        {
            if (Input.GetMouseButtonDown(0))
            {
                clicked = true;
            }
            yield return null;
        }

        canContinue = true;
    }

    IEnumerator FadeScreen(float targetAlpha)
    {
        if (blackScreen == null) yield break;

        float startAlpha = blackScreen.color.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        blackScreen.color = new Color(0, 0, 0, targetAlpha);
    }

    IEnumerator FadeInCharacter(Image character)
    {
        if (character == null) yield break;

        character.color = new Color(1, 1, 1, 0);
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            character.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        character.color = Color.white;
    }

    IEnumerator EndDialogue()
    {
        Debug.Log("对话结束");

        // 淡入黑屏
        if (blackScreen != null)
        {
            yield return StartCoroutine(FadeScreen(1f));
        }

        // 等待一下
        yield return new WaitForSeconds(0.5f);

        // 这里可以切换到下一个场景
        // SceneManager.LoadScene(nextSceneIndex);

        Debug.Log("准备切换到下一个场景");
    }
}
