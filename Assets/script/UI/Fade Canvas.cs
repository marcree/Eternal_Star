using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvas : MonoBehaviour
{
    public Image fadeImage;
    private void OnFadeEvent(Color target,float duration)
    {
        fadeImage.DOBlendableColor(target,duration);
    }
    
}
