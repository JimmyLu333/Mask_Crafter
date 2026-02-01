using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameplaySceneManager : MonoBehaviour
{
    [Header("Fade效果设置")]
    public Image fadePanel;           // 黑屏面板
    public float fadeInDuration = 2f; // 黑屏淡出时间

    // 添加：确保无论如何都执行Fade
    public bool forceFade = true;

    void Awake()  // 改为Awake，确保在Start之前执行
    {
        Debug.Log("🎮 GameplaySceneManager Awake()");

        // 强制创建FadePanel如果不存在
        EnsureFadePanelExists();
    }

    void Start()
    {
        Debug.Log("🎮 GameplaySceneManager Start()");

        // 确保FadePanel正确设置
        if (fadePanel == null)
        {
            FindFadePanel();
        }

        if (fadePanel != null)
        {
            // 强制设置黑屏状态
            fadePanel.gameObject.SetActive(true);
            fadePanel.color = Color.black;
            fadePanel.raycastTarget = false; // 避免挡住点击

            Debug.Log($"✅ FadePanel设置完成: 透明度={fadePanel.color.a}, 激活={fadePanel.gameObject.activeSelf}");
        }
        else
        {
            Debug.LogError("❌ FadePanel仍然为空！");
            // 紧急创建
            CreateEmergencyFadePanel();
        }

        // 开始淡出序列
        StartCoroutine(GameOpeningSequence());
    }

    void EnsureFadePanelExists()
    {
        // 查找场景中的FadePanel
        GameObject fadeObj = GameObject.Find("FadePanel");
        if (fadeObj == null)
        {
            Debug.Log("未找到FadePanel，正在创建...");

            // 查找Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                // 创建Canvas
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // 创建FadePanel
            GameObject panel = new GameObject("FadePanel");
            panel.transform.SetParent(canvas.transform);

            // 设置RectTransform为全屏
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector3.one;

            // 添加Image组件
            Image img = panel.AddComponent<Image>();
            img.color = Color.black;

            fadePanel = img;
            Debug.Log("✅ 已创建紧急FadePanel");
        }
        else
        {
            fadePanel = fadeObj.GetComponent<Image>();
            Debug.Log($"✅ 找到已有FadePanel: {fadeObj.name}");
        }
    }

    void FindFadePanel()
    {
        // 尝试多种方式查找
        if (fadePanel == null)
        {
            fadePanel = GameObject.Find("FadePanel")?.GetComponent<Image>();
        }

        if (fadePanel == null)
        {
            // 查找任何名称包含fade的Image
            Image[] allImages = FindObjectsOfType<Image>();
            foreach (Image img in allImages)
            {
                if (img.name.ToLower().Contains("fade") ||
                    img.name.ToLower().Contains("black") ||
                    img.color == Color.black)
                {
                    fadePanel = img;
                    Debug.Log($"找到可能的FadePanel: {img.name}");
                    break;
                }
            }
        }
    }

    void CreateEmergencyFadePanel()
    {
        Debug.Log("🆘 创建紧急FadePanel");

        // 创建Canvas如果不存在
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("EmergencyCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        // 创建FadePanel
        GameObject panel = new GameObject("EmergencyFadePanel");
        panel.transform.SetParent(canvas.transform);

        // 设置全屏
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // 添加Image
        Image img = panel.AddComponent<Image>();
        img.color = Color.black;

        // 设置为最上层
        panel.transform.SetAsLastSibling();

        fadePanel = img;
    }

    IEnumerator GameOpeningSequence()
    {
        Debug.Log("开始游戏开场序列");

        // 确保有FadePanel
        if (fadePanel == null)
        {
            Debug.LogError("FadePanel为空，无法执行淡出！");
            yield break;
        }

        // 强制设置为黑屏
        fadePanel.gameObject.SetActive(true);
        fadePanel.color = Color.black;

        // 等待一帧确保渲染
        yield return null;

        // 检查当前状态
        Debug.Log($"开始淡出前检查: 透明度={fadePanel.color.a}, RGB={fadePanel.color}");

        // 等待短暂时间
        yield return new WaitForSeconds(0.5f);

        // 执行淡出
        Debug.Log($"开始淡出黑屏，耗时: {fadeInDuration}秒");
        yield return StartCoroutine(FadeOutBlackScreen());

        Debug.Log("✅ 淡出完成，游戏开始");
    }

    IEnumerator FadeOutBlackScreen()
    {
        float elapsed = 0f;

        Debug.Log($"淡出开始: 初始透明度 = {fadePanel.color.a}");

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / fadeInDuration;
            float alpha = Mathf.Lerp(1f, 0f, progress);

            fadePanel.color = new Color(0, 0, 0, alpha);

            // 输出进度（调试用）
            if (elapsed % 0.5f < Time.deltaTime)
            {
                Debug.Log($"淡出进度: {Mathf.RoundToInt(progress * 100)}%, Alpha: {alpha:F2}");
            }

            yield return null;
        }

        // 确保完全透明
        fadePanel.color = new Color(0, 0, 0, 0f);

        // 可以选择禁用
        fadePanel.gameObject.SetActive(false);

        Debug.Log("淡出完成，Alpha = 0");
    }

    void Update()
    {
        // 调试快捷键
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("强制重新执行淡出");
            StartCoroutine(GameOpeningSequence());
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log($"当前FadePanel状态: {fadePanel?.color}");
        }
    }
}