using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MysHouse_MiniGame : MonoBehaviour
{
    MysHouse_PageController parent;
    public bool autoComplete = false;
    public List<MysHouse_MiniGameCondition> conditions = new();

    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetParent(MysHouse_PageController parent)
    {
        this.parent = parent;
    }

    public void OnConditionChange()
    {
        if (autoComplete)
        {
            CheckCondition();
        }
    }

    public void CheckCondition()
    {
        var result = true;
        foreach (var condition in conditions)
        {
            if (!condition.isCorrected) result = false;
        }
        if (result)
        {
            SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
            {
                parent.FinishPage();
            });
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, () =>
            {
                parent.FinishPage();
            });
        }
    }

    public void StartMiniGame()
    {
        DragManager.instance.GetAllDragable();
        DragManager.instance.GetAllDropable();

        foreach (var condition in conditions)
        {
            condition.SetParent(this);
        }

    }

    public void Show(float duration = 0)
    {
        canvasGroup.DOFade(1f, duration).OnComplete(() =>
        {
            canvasGroup.TotalShow();
        });

    }

    public void Hide(float duration = 0)
    {
        canvasGroup.DOFade(0f, duration).OnComplete(() =>
        {
            canvasGroup.TotalHide();
        });
    }

}
