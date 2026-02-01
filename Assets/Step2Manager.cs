using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 添加这个

public class Step2Manager : MonoBehaviour
{
    [Header("Nose Objects")]
    public GameObject nose1;
    public GameObject nose2;
    public GameObject nose3;
    public GameObject nose4;

    [Header("Zones")]
    public Collider2D dishZone;     // 0
    public Collider2D noseZone;     // 1
    public Collider2D noseZone2;    // 2
    public Collider2D noseZone3;    // 3
    public Collider2D noseZone4;    // 4
    public Collider2D paintZone;    // 5

    [Header("Brush")]
    public GameObject brushPrefab;
    public GameObject paintedBrushPrefab;
    public Transform brushSpawn;

    [Header("场景转换设置")]
    public string nextSceneName = "Step3"; // 下一个场景的名称
    public float transitionDelay = 2.0f;    // 延迟几秒后转场
    public GameObject transitionEffect;     // 可选的转场效果

    int step = 0;
    bool isTransitioning = false;          // 防止重复触发

    void Start()
    {
        ShowNose(0);
        SetupNoseColliders();
        SpawnBrush();
    }

    // ================= 鼻子显示和Collider管理 =================
    void ShowNose(int index)
    {
        nose1.SetActive(index == 1);
        nose2.SetActive(index == 2);
        nose3.SetActive(index == 3);
        nose4.SetActive(index == 4);

        UpdateNoseColliders(index);
    }

    void SetupNoseColliders()
    {
        if (noseZone != null) noseZone.gameObject.SetActive(false);
        if (noseZone2 != null) noseZone2.gameObject.SetActive(false);
        if (noseZone3 != null) noseZone3.gameObject.SetActive(false);
        if (noseZone4 != null) noseZone4.gameObject.SetActive(false);
    }

    void UpdateNoseColliders(int noseIndex)
    {
        if (noseZone != null)
            noseZone.gameObject.SetActive(noseIndex == 1 || noseIndex == 2 || noseIndex == 3 || noseIndex == 4);

        if (noseZone2 != null)
            noseZone2.gameObject.SetActive(noseIndex == 2 || noseIndex == 3 || noseIndex == 4);

        if (noseZone3 != null)
            noseZone3.gameObject.SetActive(noseIndex == 3 || noseIndex == 4);

        if (noseZone4 != null)
            noseZone4.gameObject.SetActive(noseIndex == 4);
    }

    // ================= 关闭Collider的方法 =================
    void DisableCollider(Collider2D collider)
    {
        if (collider != null)
        {
            collider.enabled = false;
        }
    }

    void DisableGameObject(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(false);
        }
    }

    // ================= 拖拽检测 =================
    public void OnToolDropped(DragTool tool)
    {
        Vector2 pos = tool.transform.position;

        if (dishZone != null && dishZone.OverlapPoint(pos))
        {
            TryUseTool(tool, 0);
        }
        else if (noseZone != null && noseZone.OverlapPoint(pos))
        {
            TryUseTool(tool, 1);
        }
        else if (noseZone2 != null && noseZone2.OverlapPoint(pos))
        {
            TryUseTool(tool, 2);
        }
        else if (noseZone3 != null && noseZone3.OverlapPoint(pos))
        {
            TryUseTool(tool, 3);
        }
        else if (noseZone4 != null && noseZone4.OverlapPoint(pos))
        {
            TryUseTool(tool, 4);
        }
        else if (paintZone != null && paintZone.gameObject.activeSelf && paintZone.OverlapPoint(pos))
        {
            TryUseTool(tool, 5);
        }
    }

    // ================= 逻辑判断 =================
    public void TryUseTool(DragTool tool, int zoneID)
    {
        int id = tool.toolID;

        // Step 0: Silicone → Dish
        if (step == 0 && id == 0 && zoneID == 0)
        {
            step = 1;
            ShowNose(1);
            Destroy(tool.gameObject);

            // 关闭dishZone的Collider
            DisableCollider(dishZone);
            return;
        }

        // Step 1: Glue → Nose
        if (step == 1 && id == 1 && zoneID == 1)
        {
            step = 2;
            ShowNose(2);
            Destroy(tool.gameObject);

            // 关闭noseZone的Collider
            DisableCollider(noseZone);
            return;
        }

        // Step 2: Dryer → Nose
        if (step == 2 && id == 2 && zoneID == 2)
        {
            step = 3;
            ShowNose(3);
            Destroy(tool.gameObject);

            // 关闭noseZone2的Collider
            DisableCollider(noseZone2);
            return;
        }

        // Step 3: Brush → Paint
        if (step == 3 && id == 3 && zoneID == 5)
        {
            Destroy(tool.gameObject);

            if (paintZone != null)
            {
                // 关闭paintZone的Collider和GameObject
                DisableCollider(paintZone);
                paintZone.gameObject.SetActive(false);
            }

            SpawnPaintedBrush();
            return;
        }

        // Step 4: Painted Brush → Nose
        if (step == 3 && id == 4 && zoneID == 3)
        {
            step = 4;
            ShowNose(4);
            Destroy(tool.gameObject);

            // 关闭noseZone3的Collider
            DisableCollider(noseZone3);

            // 完成所有步骤，开始转场
            StartCoroutine(CompleteAllSteps());
            return;
        }

        // 可选：额外步骤（如果需要）
        if (step == 4 && id == 4 && zoneID == 4)
        {
            Destroy(tool.gameObject);
            // 关闭noseZone4的Collider
            DisableCollider(noseZone4);

            // 完成额外步骤，开始转场
            StartCoroutine(CompleteAllSteps());
            return;
        }

        // 错误操作
        StartCoroutine(ShowErrorFeedback(tool));
    }

    // ================= 完成所有步骤后的处理 =================
    IEnumerator CompleteAllSteps()
    {
        if (isTransitioning) yield break; // 防止重复触发

        isTransitioning = true;

        Debug.Log("🎉 所有步骤完成！准备转场...");

        // 1. 显示完成效果（如果有）
        if (transitionEffect != null)
        {
            transitionEffect.SetActive(true);
        }

        // 2. 播放完成音效（可选）
        // AudioManager.Instance.Play("CompleteSound");

        // 3. 等待一段时间
        yield return new WaitForSeconds(transitionDelay);

        // 4. 切换到下一个场景
        LoadNextScene();
    }

    // ================= 加载下一个场景 =================
    void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("下一个场景名称未设置！");
            return;
        }

        try
        {
            SceneManager.LoadScene(nextSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载场景失败: {nextSceneName}\n错误信息: {e.Message}");
            // 可以在这里添加备用方案，比如回到主菜单
            // SceneManager.LoadScene("MainMenu");
        }
    }

    // ================= 生成刷子 =================
    void SpawnBrush()
    {
        GameObject brush = Instantiate(
            brushPrefab,
            brushSpawn.position,
            Quaternion.identity,
            brushSpawn.parent
        );

        SetupTool(brush, 3);
    }

    void SpawnPaintedBrush()
    {
        GameObject brush = Instantiate(
            paintedBrushPrefab,
            brushSpawn.position,
            Quaternion.identity,
            brushSpawn.parent
        );

        SetupTool(brush, 4);
    }

    // ================= 绑定工具 =================
    void SetupTool(GameObject obj, int id)
    {
        DragTool tool = obj.GetComponent<DragTool>();

        if (tool != null)
        {
            tool.manager = this;
            tool.toolID = id;
        }
        else
        {
            Debug.LogError("Missing DragTool on: " + obj.name);
        }
    }

    // ================= 错误反馈 =================
    IEnumerator ShowErrorFeedback(DragTool tool)
    {
        if (tool == null) yield break;

        RectTransform rt = tool.GetComponent<RectTransform>();
        if (rt == null) yield break;

        Vector3 originalScale = rt.localScale;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float shake = Mathf.Sin(elapsed * 20f) * 0.1f;
            rt.localScale = originalScale * (1f + shake);
            yield return null;
        }

        rt.localScale = originalScale;
    }

    // ================= 重置所有Collider（可选，用于重新开始游戏）=================
    [ContextMenu("重置所有Collider")]
    public void ResetAllColliders()
    {
        // 启用所有Collider
        if (dishZone != null) dishZone.enabled = true;
        if (noseZone != null) noseZone.enabled = true;
        if (noseZone2 != null) noseZone2.enabled = true;
        if (noseZone3 != null) noseZone3.enabled = true;
        if (noseZone4 != null) noseZone4.enabled = true;
        if (paintZone != null) paintZone.enabled = true;

        // 重新设置Collider激活状态
        SetupNoseColliders();

        // 重置步骤
        step = 0;
        ShowNose(0);

        // 重置转场状态
        isTransitioning = false;
    }

    // ================= 快速测试方法 =================
    [ContextMenu("测试转场")]
    public void TestTransition()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("请在运行模式下测试转场");
            return;
        }

        Debug.Log("🧪 测试转场功能...");
        StartCoroutine(CompleteAllSteps());
    }
}