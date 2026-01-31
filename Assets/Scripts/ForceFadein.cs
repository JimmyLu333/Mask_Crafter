using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ForceFadeIn : MonoBehaviour
{
    public Image blackScreenOverride;
    public float fadeDuration = 2f;

    void Start()
    {
        Debug.Log("⚠️ 强制FadeIn效果启动");
        StartCoroutine(ForceFadeInSequence());
    }

    IEnumerator ForceFadeInSequence()
    {
        // 步骤1：确保有黑屏
        Image blackScreen = GetBlackScreen();

        if (blackScreen == null)
        {
            Debug.LogError("❌ 找不到黑屏，创建紧急黑屏");
            blackScreen = CreateBlackScreen();
        }

        // 步骤2：强制设置为完全不透明
        blackScreen.gameObject.SetActive(true);
        blackScreen.color = Color.black;
        blackScreen.raycastTarget = false;

        // 步骤3：等待一帧确保渲染
        yield return null;

        Debug.Log($"✅ 黑屏设置完成，开始淡出。透明度: {blackScreen.color.a}");

        // 步骤4：执行淡出
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);

            // 输出进度
            if ((int)(elapsed * 10) != (int)((elapsed - Time.deltaTime) * 10))
            {
                Debug.Log($"淡出进度: {(int)(elapsed / fadeDuration * 100)}%");
            }

            yield return null;
        }

        // 步骤5：完成
        blackScreen.color = new Color(0, 0, 0, 0f);
        Debug.Log("✅ 强制FadeIn完成");

        // 可选：禁用黑屏
        // blackScreen.gameObject.SetActive(false);
    }

    Image GetBlackScreen()
    {
        // 尝试多种方式查找黑屏
        if (blackScreenOverride != null) return blackScreenOverride;

        // 查找名为BlackBackground的对象
        GameObject bg = GameObject.Find("BlackBackground");
        if (bg != null) return bg.GetComponent<Image>();

        // 查找任何黑色全屏Image
        Image[] allImages = FindObjectsOfType<Image>();
        foreach (Image img in allImages)
        {
            if (img.color == Color.black || img.color.a > 0.9f)
            {
                RectTransform rt = img.GetComponent<RectTransform>();
                if (rt != null && rt.anchorMin == Vector2.zero && rt.anchorMax == Vector2.one)
                {
                    return img;
                }
            }
        }

        return null;
    }

    Image CreateBlackScreen()
    {
        Debug.Log("创建紧急黑屏");

        // 查找或创建Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("EmergencyCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // 创建黑屏
        GameObject screen = new GameObject("EmergencyBlackScreen");
        screen.transform.SetParent(canvas.transform);
        screen.transform.SetAsFirstSibling(); // 放到最底层

        // 设置全屏
        RectTransform rt = screen.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // 添加Image
        Image img = screen.AddComponent<Image>();
        img.color = Color.black;
        img.raycastTarget = false;

        return img;
    }
}