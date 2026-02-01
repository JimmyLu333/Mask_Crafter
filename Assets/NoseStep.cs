using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoseStep : MonoBehaviour
{
    public Image noseImage;

    public Sprite empty;
    public Sprite nose1;
    public Sprite nose2;
    public Sprite nose3;

    public TextMeshProUGUI hintText;

    // 新增：下一步按钮
    public GameObject nextButton;

    private int step = 0;
    private bool canEdit = false;

    void Start()
    {
        noseImage.sprite = empty;

        SetHint("Click the box to open it");

        // 开局隐藏按钮
        if (nextButton != null)
            nextButton.SetActive(false);
    }

    public void OnBoxOpened()
    {
        SetHint("Drag the wax to the nose");
    }

    public void Unlock()
    {
        canEdit = true;
        noseImage.sprite = nose1;

        SetHint("Click the nose to shape it");
    }

    public void NextNose()
    {
        if (!canEdit) return;

        step++;

        if (step == 1)
        {
            noseImage.sprite = nose2;
            SetHint("Keep shaping the nose");
        }
        else if (step == 2)
        {
            noseImage.sprite = nose3;

            SetHint("Nose shaping completed");

            // ⭐ 显示箭头按钮
            if (nextButton != null)
                nextButton.SetActive(true);
        }
    }

    void SetHint(string text)
    {
        if (hintText != null)
            hintText.text = text;
    }
}
