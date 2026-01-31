using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoseStep : MonoBehaviour
{
    public Image faceImage;
    public Sprite nose1;
    public Sprite nose2;
    public Sprite nose3;

    private int step = 0;

    public void NextNose()
    {
        step++;

        if (step == 1)
            faceImage.sprite = nose2;
        else if (step == 2)
            faceImage.sprite = nose3;
    }
}