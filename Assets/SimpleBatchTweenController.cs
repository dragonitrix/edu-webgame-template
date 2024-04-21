using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SimpleBatchTweenController : MonoBehaviour
{

    public CanvasGroup canvasGroup;

    public RectTransform[] scale_zero;
    public RectTransform[] scale_one;
    public RectTransform[] pos_zero;
    public RectTransform[] pos_one;

    public float duration = 0.5f;

    void Start()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    [ContextMenu("Enter")]
    public void Enter()
    {
        canvasGroup.alpha = 1f;
        Tween(true);
    }
    [ContextMenu("Exit")]
    public void Exit()
    {
        canvasGroup.DOFade(0, 1f);
    }

    void Tween(bool val, bool skip = false)
    {
        val = true;
        var duration = skip ? 0 : this.duration;

        Vector3 one = val ? Vector3.one : Vector3.zero;
        Vector3 zero = val ? Vector3.zero : Vector3.one;

        var i = 0;

        i = 0;
        foreach (RectTransform item in scale_zero)
        {
            item.DOScale(zero, duration).SetDelay(i * 0.1f);
            i++;
        }

        i = 0;
        foreach (RectTransform item in scale_one)
        {
            item.DOScale(one, duration).SetDelay(i * 0.1f);
            i++;
        }

        i = 0;
        foreach (RectTransform item in pos_zero)
        {
            item.DOAnchorPos(zero, duration).SetDelay(i * 0.1f);
            i++;
        }

        i = 0;
        foreach (RectTransform item in pos_one)
        {
            item.DOAnchorPos(one, duration).SetDelay(i * 0.1f);
            i++;
        }
    }

}
