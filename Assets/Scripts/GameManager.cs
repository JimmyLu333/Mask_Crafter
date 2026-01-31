using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("=== 游戏设置 ===")]
    public int totalItemsRequired = 5;  // 需要找到的物品数量

    [Header("=== 要找的物品列表 ===")]
    public List<ItemData> targetItems = new List<ItemData>();  // 要找的5个物品

    [Header("=== 场景中的物品引用 ===")]
    public List<GameItem> sceneItems = new List<GameItem>();   // 所有可点击物品

    [Header("=== UI引用 ===")]
    public List<ItemSlotUI> itemSlots = new List<ItemSlotUI>(); // 5个物品槽
    public Text progressText;                                   // 进度文本
    public GameObject narrativePanel;                           // 叙事面板
    public Text narrativeText;                                  // 叙事文字
    public Image fadePanel;                                     // 淡入淡出面板

    [Header("=== 叙事设置 ===")]
    [TextArea(2, 4)]
    public string[] narrativeLines = new string[]
    {
        "太好了！我找到了所有需要的东西。",
        "现在我可以离开这里了...",
        "但前方还有什么在等着我呢？"
    };

    [Header("=== 时间设置 ===")]
    public float typingSpeed = 0.05f;   // 打字速度（秒/字符）
    public float lineDelay = 1f;        // 行与行之间的延迟（秒）
    public float endDelay = 2f;         // 最后一段文字的显示时间（秒）
    public int nextSceneIndex = 2;      // 下一个场景的索引

    // 私有变量
    private int foundCount = 0;
    private bool gameCompleted = false;
    private bool isShowingNarrative = false;

    void Start()
    {
        Debug.Log("🎮 找物品游戏开始");

        // 初始化游戏
        InitializeGame();

        // 隐藏叙事相关UI
        if (narrativePanel != null)
        {
            narrativePanel.SetActive(false);
        }

        if (fadePanel != null)
        {
            fadePanel.color = new Color(0, 0, 0, 0);
            fadePanel.gameObject.SetActive(false);
        }

        // 输出调试信息
        Debug.Log($"目标物品数量: {targetItems.Count}");
        Debug.Log($"场景物品数量: {sceneItems.Count}");
        Debug.Log($"物品槽数量: {itemSlots.Count}");
    }

    void InitializeGame()
    {
        foundCount = 0;
        gameCompleted = false;
        isShowingNarrative = false;

        // 重置所有目标物品状态
        foreach (ItemData item in targetItems)
        {
            item.isFound = false;
        }

        // 初始化UI
        InitializeUI();

        // 更新进度显示
        UpdateProgressText();
    }

    void InitializeUI()
    {
        // 初始化物品槽
        for (int i = 0; i < itemSlots.Count && i < targetItems.Count; i++)
        {
            if (itemSlots[i] != null)
            {
                itemSlots[i].Initialize(
                    targetItems[i].itemName,
                    targetItems[i].itemIcon
                );
            }
        }
    }

    // 当物品被点击时调用
    public void OnItemClicked(GameItem clickedItem)
    {
        if (gameCompleted || isShowingNarrative) return;

        string clickedName = clickedItem.GetItemName();
        Debug.Log($"检查物品: {clickedName}");

        // 检查是否是目标物品
        ItemData targetItem = FindTargetItem(clickedName);

        if (targetItem != null && !targetItem.isFound)
        {
            // 找到目标物品！
            targetItem.isFound = true;
            foundCount++;

            // 更新UI
            UpdateItemSlotUI(targetItem.itemName);

            // 标记场景中的物品
            clickedItem.MarkAsFound();

            // 更新进度
            UpdateProgressText();

            Debug.Log($"✅ 找到物品: {clickedName} ({foundCount}/{totalItemsRequired})");

            // 检查是否完成游戏
            if (foundCount >= totalItemsRequired)
            {
                StartCoroutine(OnGameCompleted());
            }
        }
        else if (targetItem != null && targetItem.isFound)
        {
            Debug.Log($"物品 {clickedName} 已经找到了");
        }
        else
        {
            Debug.Log($"物品 {clickedName} 不是目标物品");
        }
    }

    // 查找目标物品
    private ItemData FindTargetItem(string itemName)
    {
        foreach (ItemData item in targetItems)
        {
            if (item.itemName == itemName)
            {
                return item;
            }
        }
        return null;
    }

    // 更新物品槽UI
    void UpdateItemSlotUI(string itemName)
    {
        foreach (ItemSlotUI slot in itemSlots)
        {
            if (slot != null && slot.GetItemName() == itemName && !slot.IsFound())
            {
                slot.MarkAsFound();
                return;
            }
        }
    }

    // 更新进度文本
    void UpdateProgressText()
    {
        if (progressText != null)
        {
            progressText.text = $"找到 {foundCount}/{totalItemsRequired}";
        }
    }

    // 游戏完成
    IEnumerator OnGameCompleted()
    {
        gameCompleted = true;
        Debug.Log("🎉 游戏完成！所有物品已找到");

        // 等待短暂时间
        yield return new WaitForSeconds(0.5f);

        // 开始显示叙事
        isShowingNarrative = true;
        yield return StartCoroutine(ShowNarrative());
    }

    // 显示叙事
    IEnumerator ShowNarrative()
    {
        if (narrativePanel == null || narrativeText == null)
        {
            Debug.LogError("叙事面板或文本未设置！");
            yield break;
        }

        // 显示叙事面板
        narrativePanel.SetActive(true);

        // 显示每一段叙事
        for (int i = 0; i < narrativeLines.Length; i++)
        {
            // 清空文字
            narrativeText.text = "";

            // 打字机效果显示当前行
            string currentLine = narrativeLines[i];
            foreach (char letter in currentLine.ToCharArray())
            {
                narrativeText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            // 如果不是最后一行，等待点击继续
            if (i < narrativeLines.Length - 1)
            {
                // 添加"点击继续"提示
                narrativeText.text += "\n\n<size=24><color=#AAAAAA>[点击继续]</color></size>";
                yield return StartCoroutine(WaitForClick());
            }
            else
            {
                // 最后一行显示额外时间
                yield return new WaitForSeconds(endDelay);
            }
        }

        // 叙事结束，切换到下一个场景
        yield return StartCoroutine(TransitionToNextScene());
    }

    // 等待点击
    IEnumerator WaitForClick()
    {
        bool clicked = false;

        // 等待一小段时间防止误触
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

    // 切换到下一个场景
    IEnumerator TransitionToNextScene()
    {
        Debug.Log($"切换到场景索引: {nextSceneIndex}");

        // 显示淡入淡出面板
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);

            // 淡入到黑屏
            float fadeDuration = 1.5f;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                fadePanel.color = new Color(0, 0, 0, alpha);
                yield return null;
            }

            fadePanel.color = Color.black;
        }

        // 短暂等待
        yield return new WaitForSeconds(0.5f);

        // 切换到下一个场景
        if (nextSceneIndex >= 0 && nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogError($"无效的场景索引: {nextSceneIndex}");
        }
    }

    // 调试功能
    void Update()
    {
        // 按F1快速完成游戏（测试用）
        if (Input.GetKeyDown(KeyCode.F1) && !gameCompleted && !isShowingNarrative)
        {
            Debug.Log("🔧 调试：快速完成游戏");
            for (int i = 0; i < targetItems.Count; i++)
            {
                if (!targetItems[i].isFound)
                {
                    // 模拟找到物品
                    targetItems[i].isFound = true;
                    foundCount++;

                    // 更新物品槽
                    UpdateItemSlotUI(targetItems[i].itemName);

                    // 标记场景中的物品
                    GameItem sceneItem = FindSceneItem(targetItems[i].itemName);
                    if (sceneItem != null)
                    {
                        sceneItem.MarkAsFound();
                    }
                }
            }
            UpdateProgressText();
            StartCoroutine(OnGameCompleted());
        }

        // 按F2重置游戏（测试用）
        if (Input.GetKeyDown(KeyCode.F2) && !isShowingNarrative)
        {
            Debug.Log("🔧 调试：重置游戏");
            InitializeGame();
            if (narrativePanel != null)
                narrativePanel.SetActive(false);
        }
    }

    // 查找场景中的物品
    private GameItem FindSceneItem(string itemName)
    {
        foreach (GameItem item in sceneItems)
        {
            if (item.GetItemName() == itemName)
            {
                return item;
            }
        }
        return null;
    }

    // 在编辑器中显示调试信息
    void OnGUI()
    {
        if (Application.isEditor)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 14;
            style.normal.textColor = Color.white;

            GUI.Label(new Rect(10, 10, 300, 30), $"找到: {foundCount}/{totalItemsRequired}", style);
            GUI.Label(new Rect(10, 30, 300, 30), $"游戏状态: {(gameCompleted ? "已完成" : "进行中")}", style);
            GUI.Label(new Rect(10, 50, 300, 30), $"叙事状态: {(isShowingNarrative ? "显示中" : "未显示")}", style);

            // 显示所有目标物品状态
            for (int i = 0; i < targetItems.Count; i++)
            {
                string status = targetItems[i].isFound ? "✅" : "❌";
                GUI.Label(new Rect(10, 80 + i * 20, 300, 20), $"{status} {targetItems[i].itemName}", style);
            }
        }
    }
}