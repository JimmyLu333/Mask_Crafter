using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step2Manager : MonoBehaviour
{
    [Header("Nose")]
    public GameObject nose1;
    public GameObject nose2;
    public GameObject nose3;
    public GameObject nose4;

    [Header("Zones")]
    public Collider2D dishZone;   // 0
    public Collider2D noseZone;   // 1
    public Collider2D paintZone;  // 2

    [Header("Brush")]
    public GameObject brushPrefab;
    public GameObject paintedBrushPrefab;
    public Transform brushSpawn;

    int step = 0;

    void Start()
    {
        ShowNose(0);
        SpawnBrush();
    }

    // ================= 鼻子显示 =================

    void ShowNose(int index)
    {
        nose1.SetActive(index == 1);
        nose2.SetActive(index == 2);
        nose3.SetActive(index == 3);
        nose4.SetActive(index == 4);
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

        SetupTool(brush, 3); // 普通刷子 = 3
    }

    void SpawnPaintedBrush()
    {
        GameObject brush = Instantiate(
            paintedBrushPrefab,
            brushSpawn.position,
            Quaternion.identity,
            brushSpawn.parent
        );

        SetupTool(brush, 4); // 染色刷子 = 4
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

    // ================= 拖拽入口（关键）=================

    public void OnToolDropped(DragTool tool)
    {
        Vector2 pos = tool.transform.position;

        if (dishZone.OverlapPoint(pos))
        {
            TryUseTool(tool, 0);
        }
        else if (noseZone.OverlapPoint(pos))
        {
            TryUseTool(tool, 1);
        }
        else if (paintZone != null &&
                 paintZone.gameObject.activeSelf &&
                 paintZone.OverlapPoint(pos))
        {
            TryUseTool(tool, 2);
        }
        else
        {
            Debug.Log("Dropped on nothing");
        }
    }

    // ================= 逻辑判断 =================

    public void TryUseTool(DragTool tool, int zoneID)
    {
        int id = tool.toolID;

        Debug.Log($"Use Tool {id} on Zone {zoneID} Step {step}");

        // Step 0: Silicone → Dish
        if (step == 0 && id == 0 && zoneID == 0)
        {
            step = 1;
            ShowNose(1);
            Destroy(tool.gameObject);
            return;
        }

        // Step 1: Glue → Nose
        if (step == 1 && id == 1 && zoneID == 1)
        {
            step = 2;
            ShowNose(2);
            Destroy(tool.gameObject);
            return;
        }

        // Step 2: Dryer → Nose
        if (step == 2 && id == 2 && zoneID == 1)
        {
            step = 3;
            ShowNose(3);
            Destroy(tool.gameObject);
            return;
        }

        // Step 3: Brush → Paint
        if (step == 3 && id == 3 && zoneID == 2)
        {
            Destroy(tool.gameObject);

            paintZone.gameObject.SetActive(false);

            SpawnPaintedBrush();

            return;
        }

        // Step 4: Painted Brush → Nose
        if (step == 3 && id == 4 && zoneID == 1)
        {
            step = 4;
            ShowNose(4);

            Destroy(tool.gameObject);

            Debug.Log("FINISH!");
            return;
        }

        Debug.Log("Wrong Step");
    }
}
