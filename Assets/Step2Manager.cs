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
    public GameObject brushPrefab;        // 普通刷子
    public GameObject paintedBrushPrefab; // 染色刷子
    public Transform brushSpawn;          // 出生点

    int step = 0;

    void Start()
    {
        ShowNose(0);
        SpawnBrush();
    }

    // 控制鼻子显示
    void ShowNose(int index)
    {
        nose1.SetActive(index == 1);
        nose2.SetActive(index == 2);
        nose3.SetActive(index == 3);
        nose4.SetActive(index == 4);
    }

    // 生成普通刷子
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

    // 生成染色刷子
    void SpawnPaintedBrush()
    {
        GameObject brush = Instantiate(
            paintedBrushPrefab,
            brushSpawn.position,
            Quaternion.identity,
            brushSpawn.parent
        );

        // 染色刷子 = ID 4
        SetupTool(brush, 4);
    }

    // 给新工具绑定 Manager 和 ID（核心）
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

    // 处理拖拽逻辑
    public void TryUseTool(DragTool tool, int zoneID)
    {
        int id = tool.toolID;

        Debug.Log("Use Tool: " + id + " on Zone: " + zoneID + " Step: " + step);

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
