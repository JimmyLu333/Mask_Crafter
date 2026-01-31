using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoseStep : MonoBehaviour
{
    public Image noseImage;
    public Sprite nose1;
    public Sprite nose2;
    public Sprite nose3;

    private int step = 0;
    private bool canEdit = false; // 是否已放粘土

    // 被 NoseDrop 调用：解锁
    public void Unlock()
    {
        canEdit = true;
    }

    // 点击鼻子时调用
    public void NextNose()
    {
        if (!canEdit)
        {
            Debug.Log("还没放粘土，不能操作");
            return;
        }

        step++;

        if (step == 1)
            noseImage.sprite = nose2;
        else if (step == 2)
            noseImage.sprite = nose3;
    }
}
