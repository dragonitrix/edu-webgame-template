using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MysHouse_PageController : MonoBehaviour
{
    public bool hightlightOnEnter = false;

    public RectTransform interactableGroup;
    public RectTransform subPageGroup;
    public RectTransform miniGameGroup;

    public List<MysHouse_PageController> subPages = new();
    public List<MysHouse_Interactable> interactableObjs = new();
    public List<MysHouse_MiniGame> miniGames = new();
    public List<RectTransform> extras = new();
    CanvasGroup canvasGroup;

    MysHouse_PageController parent;

    public string transitionInID = "";
    public string transitionOutID = "";

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        //fetch
        foreach (RectTransform interactable in interactableGroup)
        {
            var _interactable = interactable.GetComponent<MysHouse_Interactable>();
            _interactable.SetParent(this);
            interactableObjs.Add(_interactable);
        }

        foreach (RectTransform subPage in subPageGroup)
        {
            var _subpage = subPage.GetComponent<MysHouse_PageController>();
            _subpage.SetParent(this);
            _subpage.Hide();
            subPages.Add(_subpage);
        }
        foreach (RectTransform minigame in miniGameGroup)
        {
            var _minigame = minigame.GetComponent<MysHouse_MiniGame>();
            _minigame.SetParent(this);
            miniGames.Add(_minigame);
        }
    }

    public void SetParent(MysHouse_PageController parent)
    {
        this.parent = parent;
    }

    public void Show(float duration = 0, bool enter = false)
    {
        canvasGroup.DOFade(1f, duration).OnComplete(() =>
        {
            canvasGroup.TotalShow();
            if (enter)
            {
                Enter();
            }
        });

    }

    public void Hide(float duration = 0)
    {
        canvasGroup.DOFade(0f, duration).OnComplete(() =>
        {
            canvasGroup.TotalHide();
        });
    }

    public void Enter()
    {
        // actual enter script
        if (hightlightOnEnter)
        {
            for (int i = 0; i < interactableObjs.Count; i++)
            {
                var obj = interactableObjs[i];
                obj.ShowHightLight(0.2f);
            }
        }

        foreach (var minigame in miniGames)
        {
            minigame.StartMiniGame();
        }

    }

    public void FinishPage()
    {
        Debug.Log("myPage: " + gameObject.name);
        Debug.Log("transitionOutID: " + transitionOutID);
        if (transitionOutID != "")
        {
            TransitionOut();
            return;
        }

        ((MysHouse_GameController)GameController.instance).NextPage();
    }

    void TransitionOut()
    {
        switch (transitionOutID)
        {
            case "living1":
                foreach (var p in subPages)
                {
                    p.Hide();
                }
                var obj1 = interactableObjs[0];
                obj1.rectTransform.DOAnchorPosY(obj1.rectTransform.anchoredPosition.y - 100, 0.5f).SetDelay(0.5f);
                obj1.canvasGroup.DOFade(0f, 0.3f).SetDelay(0.5f);

                extras[0].GetComponent<CanvasGroup>().DOFade(1f, 1f).SetDelay(2f)
                .OnStart(() =>
                {
                    AudioManager.instance.PlaySound("ui_door");
                })
                .OnComplete(() =>
                {
                    DoDelayAction(1f, () =>
                    {
                        ((MysHouse_GameController)GameController.instance).NextPage();
                    });
                });
                break;

            case "living2_shelf":
                miniGames[0].Hide(0.2f);
                var shelf = extras[0];
                shelf.DOAnchorPos(Vector2.zero, 0.2f).OnComplete(() =>
                {
                    shelf.DOAnchorPosX(shelf.anchoredPosition.x - 100, 1f).SetDelay(1f)
                    .OnStart(() =>
                    {
                        AudioManager.instance.PlaySound("ui_obj_slide");
                    })
                    .OnComplete(() =>
                    {
                        Hide(1f);
                    });
                });
                break;
            case "living2_sofa":
                miniGames[0].Hide(0.2f);
                var sofa = extras[0];
                sofa.DOScale(1.2f, 0.2f);
                sofa.DOAnchorPos(Vector2.zero, 0.2f).OnComplete(() =>
                {
                    var key1 = extras[2];
                    key1.GetComponent<CanvasGroup>().alpha = 1;
                    sofa.DOAnchorPosX(sofa.anchoredPosition.x - 100, 1f).SetDelay(1f)
                    .OnStart(() =>
                    {
                        AudioManager.instance.PlaySound("ui_obj_slide");
                    })
                    .OnComplete(() =>
                    {
                        extras[1].DOScale(1f, 0.5f);
                        key1.DOScale(2f, 0.5f);
                        AudioManager.instance.PlaySound("ui_win_2", () =>
                        {
                            parent.FinishPage();
                        });
                    });
                });
                break;
        }
    }

    void DoDelayAction(float delayTime, UnityAction action)
    {
        StartCoroutine(DelayAction(delayTime, action));
    }
    IEnumerator DelayAction(float delayTime, UnityAction action)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);
        //Do the action after the delay time has finished.
        action.Invoke();
    }
}
