using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClayBoxControl : MonoBehaviour
{
    public GameObject closeBox;
    public GameObject openBox;

    // 加这个
    public NoseStep noseStep;

    void Start()
    {
        closeBox.SetActive(true);
        openBox.SetActive(false);
    }

    public void OpenBox()
    {
        closeBox.SetActive(false);
        openBox.SetActive(true);

        // 通知鼻子：盒子已开
        if (noseStep != null)
        {
            noseStep.OnBoxOpened();
        }
    }
}
