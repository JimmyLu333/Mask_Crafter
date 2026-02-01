using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleTutorial : MonoBehaviour
{
    [Header("教程文本")]
    [TextArea(2, 4)]
    public string[] tutorialLines = new string[]
    {
        "It is time to make a nose!",
        "Make your nose following these steps:",
        "1. Place the prepared silicon mold onto the tray",
        "2. Apply glue evenly over the surface of the nose.",
        "3. Use a hairdryer to dry the glue completely.",
        "4. Dip your brush into the colors on the paint palette.",
        "5. Use the brush to apply color evenly to the nose.",
        "Lets Begin！"
    };

    [Header("UI引用")]
    public GameObject tutorialPanel;     // 教程面板
    public Text tutorialText;           // 教程文本

    [Header("设置")]
    public float typingSpeed = 0.05f;   // 打字速度
    public float lineDelay = 1.5f;      // 行间延迟
    public bool autoShowOnStart = true; // 游戏开始时自动显示

    void Start()
    {
        if (autoShowOnStart && tutorialLines.Length > 0)
        {
            StartCoroutine(ShowTutorial());
        }
    }

    IEnumerator ShowTutorial()
    {
        // 显示面板
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }

        // 逐行显示教程
        for (int i = 0; i < tutorialLines.Length; i++)
        {
            // 清空文本
            tutorialText.text = "";

            // 打字机效果显示当前行
            string currentLine = tutorialLines[i];
            foreach (char letter in currentLine.ToCharArray())
            {
                tutorialText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            // 添加点击提示（最后一行除外）
            if (i < tutorialLines.Length - 1)
            {
                tutorialText.text += "\n\n<size=20><color=#AAAAAA>[点击继续]</color></size>";

                // 等待点击
                yield return StartCoroutine(WaitForClick());
            }
        }

        // 最后一行等待点击开始游戏
        tutorialText.text += "\n\n<size=20><color=#AAAAAA>[点击开始游戏]</color></size>";
        yield return StartCoroutine(WaitForClick());

        // 结束教程
        EndTutorial();
    }

    IEnumerator WaitForClick()
    {
        bool clicked = false;

        // 防止立即点击
        yield return new WaitForSeconds(0.1f);

        while (!clicked)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                clicked = true;
            }
            yield return null;
        }
    }

    void EndTutorial()
    {
        // 隐藏教程面板
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        // 可选：通知其他脚本教程已结束
        OnTutorialComplete();
    }

    void OnTutorialComplete()
    {
        // 这里可以添加教程结束后的逻辑
        // 例如：启用游戏控制、播放声音等
        Debug.Log("✅ 教程结束，开始游戏");
    }

    // 外部调用的方法
    public void StartTutorial()
    {
        StartCoroutine(ShowTutorial());
    }

    public void SkipTutorial()
    {
        StopAllCoroutines();
        EndTutorial();
    }
}