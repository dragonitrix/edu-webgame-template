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

    public string audioInSoundID = "";
    public string audioOutSoundID = "";

    public MysHouse_PageController targetPage;

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
            if (_interactable)
            {
                _interactable.SetParent(this);
                interactableObjs.Add(_interactable);
            }
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

        if (audioInSoundID != "")
        {
            AudioManager.instance.PlaySpacialSound(audioInSoundID);
        }

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

        TransitionIn();

    }

    void TransitionIn()
    {
        switch (transitionInID)
        {
            case "living2":

                interactableObjs[0].ShowHightLight(0.5f);
                interactableObjs[1].canvasGroup.interactable = false;
                break;
            case "kitchen":
                interactableObjs[0].ShowHightLight(0.5f);
                interactableObjs[1].canvasGroup.interactable = false;
                interactableObjs[2].canvasGroup.interactable = false;
                interactableObjs[3].canvasGroup.interactable = false;
                break;
            case "wc2":
                interactableObjs[0].ShowHightLight(0.5f);
                interactableObjs[1].canvasGroup.interactable = false;
                interactableObjs[2].canvasGroup.interactable = false;
                interactableObjs[3].canvasGroup.interactable = false;
                break;
            case "bedroom":
                interactableObjs[0].ShowHightLight(0.5f);
                interactableObjs[1].canvasGroup.interactable = false;
                interactableObjs[2].canvasGroup.interactable = false;
                break;
        }

    }

    public void FinishPage()
    {
        if (transitionOutID != "")
        {
            TransitionOut();
            return;
        }

        if (audioOutSoundID != "")
        {
            AudioManager.instance.PlaySpacialSound(audioOutSoundID);
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
                    AudioManager.instance.PlaySound("sfx_door");
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
                extras[1].gameObject.SetActive(false);
                var shelf = extras[0];
                shelf.DOAnchorPos(Vector2.zero, 0.2f).OnComplete(() =>
                {
                    shelf.DOAnchorPosX(shelf.anchoredPosition.x - 300, 1f).SetDelay(1f)
                    .OnStart(() =>
                    {
                        AudioManager.instance.PlaySound("sfx_obj_slide");
                    })
                    .OnComplete(() =>
                    {
                        Hide(1f);
                        parent.interactableObjs[1].canvasGroup.interactable = true;
                        parent.interactableObjs[1].ShowHightLight(0.5f);
                    });
                });
                break;
            case "living2_sofa":
                miniGames[0].Hide(0.2f);
                extras[4].gameObject.SetActive(false);
                var sofa = extras[0];
                sofa.DOScale(1.2f, 0.2f);
                sofa.DOAnchorPos(Vector2.zero, 0.2f).OnComplete(() =>
                {
                    var key1 = extras[2];
                    key1.GetComponent<CanvasGroup>().alpha = 1;
                    sofa.DOAnchorPosX(sofa.anchoredPosition.x - 1000, 1f).SetDelay(1f)
                    .OnStart(() =>
                    {
                        AudioManager.instance.PlaySound("sfx_obj_slide");
                    })
                    .OnComplete(() =>
                    {
                        AudioManager.instance.PlaySpacialSound(audioOutSoundID);
                        extras[1].DOScale(1f, 0.5f);
                        key1.DOScale(2f, 0.5f);
                        extras[3].DOScale(1f, 0.5f);
                        AudioManager.instance.PlaySound("ui_win_2", () =>
                        {
                            parent.FinishPage();
                        });
                    });
                });
                break;
            case "kitchen_dish":
                miniGames[0].Hide(0.2f);
                extras[1].gameObject.SetActive(false);
                var dish = extras[0];
                dish.DOAnchorPos(Vector2.zero, 0.2f).OnComplete(() =>
                {
                    extras[2].localScale = Vector2.one;
                    dish.DOAnchorPosX(dish.anchoredPosition.x - 1000, 1f).SetDelay(1f)
                    .OnStart(() =>
                    {
                        AudioManager.instance.PlaySound("sfx_obj_slide");
                    })
                    .OnComplete(() =>
                    {
                        Hide(0f);
                        targetPage.Show(1f, true);
                    });
                });
                break;
            case "kitchen_oven":
                var oven = extras[0];
                oven.GetComponent<CanvasGroup>().DOFade(1f, 1f).OnComplete(() =>
                {
                    Hide(0f);
                    targetPage.Show(1f, true);
                });
                break;
            case "kitchen_note_table":
            case "kitchen_note_oven":
                Hide(0f);
                targetPage.Show(1f, true);
                break;
            case "kitchen_bin":
                var bin = extras[0];
                var key_bin = extras[1];
                var flare_bin = extras[2];
                var text_bin = extras[3];

                key_bin.localScale = Vector2.one;
                bin.DOAnchorPosX(bin.anchoredPosition.x - 1000, 1f).SetDelay(0.2f)
                .OnStart(() =>
                {
                    AudioManager.instance.PlaySound("sfx_obj_slide");
                })
                .OnComplete(() =>
                {
                    AudioManager.instance.PlaySpacialSound(audioOutSoundID);
                    flare_bin.DOScale(1f, 0.5f);
                    key_bin.DOScale(2f, 0.5f);
                    text_bin.DOScale(1f, 0.5f);
                    AudioManager.instance.PlaySound("ui_win_2", () =>
                    {
                        parent.FinishPage();
                    });
                });
                break;
            case "stair1":
                extras[0].GetComponent<CanvasGroup>().DOFade(1f, 1f).SetDelay(2f)
                .OnStart(() =>
                {
                    AudioManager.instance.PlaySound("sfx_walk");
                })
                .OnComplete(() =>
                {
                    DoDelayAction(1f, () =>
                    {
                        ((MysHouse_GameController)GameController.instance).NextPage();
                    });
                });
                break;

            case "wc1":
                foreach (var p in subPages)
                {
                    p.Hide(0.2f);
                }
                var objkey = interactableObjs[0];
                objkey.rectTransform.DOAnchorPosY(objkey.rectTransform.anchoredPosition.y - 100, 0.5f).SetDelay(0.5f);
                objkey.canvasGroup.DOFade(0f, 0.3f).SetDelay(0.5f);

                interactableObjs[1].gameObject.SetActive(false);

                AudioManager.instance.PlaySpacialSound("mh_quotes_03_01");
                extras[1].DOScale(1f, 0.5f);

                extras[0].GetComponent<CanvasGroup>().DOFade(1f, 1f).SetDelay(2f)
                .OnStart(() =>
                {
                    AudioManager.instance.PlaySound("sfx_door");
                })
                .OnComplete(() =>
                {
                    DoDelayAction(1f, () =>
                    {
                        ((MysHouse_GameController)GameController.instance).NextPage();
                    });
                });
                break;
            case "wc2_toothbrush":
                miniGames[0].Hide(0.2f);
                extras[1].gameObject.SetActive(false);
                var wc_toothbrush = extras[0];
                wc_toothbrush.DOAnchorPos(Vector2.zero, 0.2f).OnComplete(() =>
                {
                    wc_toothbrush.DOAnchorPosX(wc_toothbrush.anchoredPosition.x - 300, 1f).SetDelay(1f)
                    .OnStart(() =>
                    {
                        // AudioManager.instance.PlaySound("sfx_obj_slide");
                    })
                    .OnComplete(() =>
                    {
                        Hide(1f);
                        parent.interactableObjs[1].canvasGroup.interactable = true;
                        parent.interactableObjs[1].ShowHightLight(0.5f);
                    });
                });
                break;
            case "wc2_soap":
                miniGames[0].Hide(0.2f);
                extras[1].gameObject.SetActive(false);
                var wc2_soap = extras[0];
                wc2_soap.DOAnchorPos(Vector2.zero, 0.2f).OnComplete(() =>
                {
                    wc2_soap.DOAnchorPosX(wc2_soap.anchoredPosition.x - 300, 1f).SetDelay(1f)
                    .OnStart(() =>
                    {
                        // AudioManager.instance.PlaySound("sfx_obj_slide");
                    })
                    .OnComplete(() =>
                    {
                        Hide(1f);
                        parent.interactableObjs[2].canvasGroup.interactable = true;
                        parent.interactableObjs[2].ShowHightLight(0.5f);
                    });
                });
                break;
            case "wc2_toilet":
                miniGames[0].Hide(0.2f);
                extras[1].gameObject.SetActive(false);
                var wc2_toilet = extras[0];
                wc2_toilet.DOAnchorPos(Vector2.zero, 0.2f).OnComplete(() =>
                {
                    wc2_toilet.DOAnchorPosX(wc2_toilet.anchoredPosition.x - 300, 1f).SetDelay(1f)
                    .OnStart(() =>
                    {
                        AudioManager.instance.PlaySound("sfx_obj_slide");
                    })
                    .OnComplete(() =>
                    {
                        Hide(1f);
                        parent.interactableObjs[3].canvasGroup.interactable = true;
                        parent.interactableObjs[3].ShowHightLight(0.5f);
                    });
                });
                break;

            case "wc2_sink":
                miniGames[0].Hide(0.2f);
                extras[1].gameObject.SetActive(false);
                var wc_sink = extras[0];

                var key_sink = extras[2];
                var flare_sink = extras[3];
                var text_sink = extras[4];

                wc_sink.DOAnchorPos(Vector2.zero, 0.2f).OnComplete(() =>
                {
                    key_sink.localScale = Vector2.one;
                    wc_sink.DOAnchorPosX(wc_sink.anchoredPosition.x - 1000, 1f).SetDelay(1f)
                    .OnStart(() =>
                    {
                        AudioManager.instance.PlaySound("sfx_obj_slide");
                    })
                    .OnComplete(() =>
                    {
                        AudioManager.instance.PlaySpacialSound(audioOutSoundID);
                        flare_sink.DOScale(1f, 0.5f);
                        text_sink.DOScale(1f, 0.5f);
                        AudioManager.instance.PlaySound("ui_win_2", () =>
                        {
                            parent.FinishPage();
                        });
                    });

                });
                break;

            case "wc3":
                foreach (var p in subPages)
                {
                    p.Hide(0.2f);
                }
                var objkey3 = interactableObjs[0];
                objkey3.rectTransform.DOAnchorPosY(objkey3.rectTransform.anchoredPosition.y - 100, 0.5f).SetDelay(0.5f);
                objkey3.canvasGroup.DOFade(0f, 0.3f).SetDelay(0.5f);

                interactableObjs[1].gameObject.SetActive(false);

                // AudioManager.instance.PlaySpacialSound("mh_quotes_03_01");

                extras[0].GetComponent<CanvasGroup>().DOFade(1f, 1f).SetDelay(2f)
                .OnStart(() =>
                {
                    AudioManager.instance.PlaySound("sfx_door");
                })
                .OnComplete(() =>
                {
                    DoDelayAction(1f, () =>
                    {
                        ((MysHouse_GameController)GameController.instance).NextPage();
                    });
                });
                break;
            case "bedroom_clothes":
                miniGames[0].Hide(0.2f);
                extras[1].gameObject.SetActive(false);
                var bedroom_clothes = extras[0];
                bedroom_clothes.DOAnchorPos(Vector2.zero, 0.2f).OnComplete(() =>
                {
                    bedroom_clothes.DOAnchorPosX(bedroom_clothes.anchoredPosition.x - 300, 1f).SetDelay(1f)
                    .OnStart(() =>
                    {
                        // AudioManager.instance.PlaySound("sfx_obj_slide");
                    })
                    .OnComplete(() =>
                    {
                        Hide(1f);
                        parent.interactableObjs[1].canvasGroup.interactable = true;
                        parent.interactableObjs[1].ShowHightLight(0.5f);
                    });
                });
                break;
            case "bedroom_pillow":
                miniGames[0].Hide(0.2f);
                extras[1].gameObject.SetActive(false);
                var bedroom_pillow = extras[0];
                bedroom_pillow.DOAnchorPos(Vector2.zero, 0.2f).OnComplete(() =>
                {
                    bedroom_pillow.DOAnchorPosX(bedroom_pillow.anchoredPosition.x - 300, 1f).SetDelay(1f)
                    .OnStart(() =>
                    {
                        // AudioManager.instance.PlaySound("sfx_obj_slide");
                    })
                    .OnComplete(() =>
                    {
                        Hide(1f);
                        parent.interactableObjs[2].canvasGroup.interactable = true;
                        parent.interactableObjs[2].ShowHightLight(0.5f);
                    });
                });
                break;
            case "bedroom_bed":
                miniGames[0].Hide(0.2f);
                extras[1].gameObject.SetActive(false);
                var bedroom_bed = extras[0];
                bedroom_bed.DOAnchorPos(Vector2.zero, 0.2f).OnComplete(() =>
                {
                    bedroom_bed.DOAnchorPosX(bedroom_bed.anchoredPosition.x - 300, 1f).SetDelay(1f)
                    .OnStart(() =>
                    {
                        AudioManager.instance.PlaySound("sfx_obj_slide");
                    })
                    .OnComplete(() =>
                    {
                        Hide(1f);
                        ((MysHouse_GameController)GameController.instance).EndGame();
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
