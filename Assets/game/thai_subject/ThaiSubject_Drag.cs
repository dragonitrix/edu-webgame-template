using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ThaiSubject_Drag : MonoBehaviour
{
    public int index;
    public Image image;
    CanvasGroup canvasGroup;

    void Awake()
    {
        if (transform.childCount > 0) image = transform.GetChild(0).GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void InitDrag(int index, Sprite sprite)
    {
        this.index = index;
        image.sprite = sprite;
    }

    public void Hide()
    {
        canvasGroup.TotalHide();
        canvasGroup.DOFade(0, 1f).From(1f);
    }

}
