using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClayBoxControl : MonoBehaviour
{
    public GameObject closeBox;
    public GameObject openBox;

    // 点击关盒调用
    public void OpenBox()
    {
        closeBox.SetActive(false);
        openBox.SetActive(true);
    }
}
