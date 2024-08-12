using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EngTrash4_Drag : MonoBehaviour
{
    public int index;
    [HideInInspector]
    public bool isCorrect = false;
    [HideInInspector]
    public Draggable draggable;
    [HideInInspector]
    public CanvasGroup canvasGroup;

    void Awake()
    {
        draggable = GetComponent<Draggable>();
        canvasGroup = GetComponent<CanvasGroup>();
        draggable.dragableBG = GetComponent<Image>();
    }

    public void SetCorrect()
    {
        if (isCorrect) return;
        isCorrect = true;

        var rt = GetComponent<RectTransform>();
        rt.DOScale(0, 0.5f).OnComplete(() =>
        {
            canvasGroup.TotalHide();
        });
    }



}
