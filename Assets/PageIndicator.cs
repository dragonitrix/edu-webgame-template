using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PageIndicator : MonoBehaviour
{

    Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public Color inactiveColor;
    public Vector3 inactiveScale;
    public Color activeColor;
    public Vector3 activeScale;

    float tweenDuration = 0.2f;

    public void SetActive()
    {
        image.DOColor(activeColor, tweenDuration);
        transform.DOScale(activeScale, tweenDuration);
    }
    public void SetInactive()
    {
        image.DOColor(inactiveColor, tweenDuration);
        transform.DOScale(inactiveScale, tweenDuration);
    }
}
