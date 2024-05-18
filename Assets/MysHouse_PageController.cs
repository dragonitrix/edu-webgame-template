using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MysHouse_PageController : MonoBehaviour
{
    public bool hightlightOnEnter = false;

    public RectTransform interactableGroup;
    public RectTransform subPageGroup;

    public List<MysHouse_PageController> subPages = new();
    public List<RectTransform> interactableObjs = new();

    CanvasGroup canvasGroup;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        //fetch
        foreach (RectTransform interactable in interactableGroup)
        {
            var highlight = interactable.Find("highlight");
            if (highlight)
            {
                highlight.localScale = Vector2.zero;
            }
            interactableObjs.Add(interactable);
        }

        foreach (RectTransform subPage in subPageGroup)
        {
            subPages.Add(subPage.GetComponent<MysHouse_PageController>());
        }


    }

    public void Show()
    {
        canvasGroup.TotalShow();
    }

    public void Hide()
    {
        canvasGroup.TotalHide();
    }

    public void Enter()
    {
        // actual enter script

        if (hightlightOnEnter)
        {
            for (int i = 0; i < interactableObjs.Count; i++)
            {
                var obj = interactableObjs[i];
                var highlight = obj.Find("highlight");
                if (highlight)
                {
                    highlight.DOScale(1f, 0.2f).From(0f).SetDelay(i * 0.1f);
                }
            }
        }

    }

}
