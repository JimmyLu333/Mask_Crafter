using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Step2Manager : MonoBehaviour
{
    public GameObject stage0;
    public GameObject stage1;
    public GameObject stage2;
    public GameObject stage3;
    public GameObject stage4;

    public TextMeshProUGUI hintText;

    private int step = 0;

    void Start()
    {
        ShowStage(0);
        SetHint("Click the silicone to make the mold");
    }

    void ShowStage(int index)
    {
        stage0.SetActive(index == 0);
        stage1.SetActive(index == 1);
        stage2.SetActive(index == 2);
        stage3.SetActive(index == 3);
        stage4.SetActive(index == 4);
    }

    void SetHint(string t)
    {
        if (hintText != null)
            hintText.text = t;
    }

    // 点硅胶
    public void OnSilicone()
    {
        if (step != 0) return;

        step = 1;
        ShowStage(1);
        SetHint("Use the dryer to harden it");
    }

    // 点吹风机
    public void OnDryer()
    {
        if (step != 1) return;

        step = 2;
        ShowStage(2);
        SetHint("Brush off the excess material");
    }

    // 点刷子
    public void OnBrush()
    {
        if (step != 2) return;

        step = 3;
        ShowStage(3);
        SetHint("Apply glue to finish");
    }

    // 点胶水
    public void OnGlue()
    {
        if (step != 3) return;

        step = 4;
        ShowStage(4);
        SetHint("Production completed");
    }
}
