using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;
using System.Text;


public class ThaiTales_GameController : GameController
{

    [Header("Prefab")]
    public GameObject choice_prefab;


    [Header("Obj ref")]
    public PopupController[] talesPopups;
    public CanvasGroup doorRect;
    public Button doorA;
    public Button doorB;
    public CanvasGroup gameRect;
    public TextMeshProUGUI questionText;
    public RectTransform choicesRect;
    public CanvasGroup safeRect;
    public RectTransform[] digits;


    public CanvasGroup rewardRect;
    public CanvasGroup safe_close;
    public CanvasGroup safe_open;
    public RectTransform reward;
    public RectTransform fail;
    public TextMeshProUGUI resultText;

    [Header("Setting")]

    [Header("Data")]

    public int talesIndex = 0;
    public int roundIndex = -1;

    public ThaiTales_Datas[] tales_Datas;
    public ThaiTales_Datas currentTale = null;
    public ThaiTales_Data currentQuestion = null;

    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    List<ThaiTales_Choice> choices = new();

    public int score = 0;

    bool isAnswering = false;

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        foreach (var tale in talesPopups)
        {
            var closeButton = tale.closeButton;
            closeButton.gameObject.SetActive(false);

            var pageController = tale.GetComponentInChildren<PageController>();
            pageController.onFinalPage += () =>
            {
                closeButton.gameObject.SetActive(true);
            };
        }

        doorA.onClick.AddListener(() =>
        {
            OnDoorClick(0);
        });
        doorB.onClick.AddListener(() =>
        {
            OnDoorClick(1);
        });

        HideDoors();
        HideQuestion();
        HidePassword();

        tutorialPopup.Enter();
        tutorialPopup.OnPopupExit += () =>
        {
            tutorialPopup.OnPopupExit = () => { };
            SetPhase(GAME_PHASE.ROUND_START);
        };

    }


    public override void StartGame()
    {
        Debug.Log("start game");

    }

    void ShowDoors()
    {
        doorRect.TotalHide();
        doorRect.DOFade(1, 0.5f).OnComplete(() =>
        {
            doorRect.TotalShow();
        });
    }
    void HideDoors()
    {
        doorRect.TotalHide();
        doorRect.DOFade(0, 0.5f).From(1).OnComplete(() =>
        {
        });
    }

    void ShowQuestion()
    {
        gameRect.TotalHide();
        gameRect.DOFade(1, 0.5f).OnComplete(() =>
        {
            gameRect.TotalShow();
        });
    }
    void HideQuestion()
    {
        gameRect.TotalHide();
        gameRect.DOFade(0, 0.5f).From(1).OnComplete(() =>
        {
        });
    }

    void ShowPassword()
    {
        safeRect.TotalHide();
        safeRect.DOFade(1, 0.5f).OnComplete(() =>
        {
            safeRect.TotalShow();
        });
    }
    void HidePassword()
    {
        safeRect.TotalHide();
        safeRect.DOFade(0, 0.5f).From(1).OnComplete(() =>
        {
        });
    }

    public GAME_PHASE gamePhase = GAME_PHASE.NULL;

    public void SetPhase(GAME_PHASE targetPhase)
    {
        if (gamePhase == targetPhase) return;

        // exit current phase
        switch (gamePhase)
        {
            case GAME_PHASE.NULL:
                break;
            case GAME_PHASE.INTRO:
                break;
            case GAME_PHASE.ROUND_START:
                break;
            case GAME_PHASE.ROUND_WAITING:
                break;
            case GAME_PHASE.ROUND_ANSWERING:
                break;
        }

        gamePhase = targetPhase;
        // Debug.Log("Set phase: " + gamePhase);

        // enter target phase
        switch (gamePhase)
        {
            case GAME_PHASE.NULL:
                break;
            case GAME_PHASE.INTRO:
                OnEnterIntro();
                break;
            case GAME_PHASE.ROUND_START:
                OnEnterRoundStart();
                break;
            case GAME_PHASE.ROUND_WAITING:
                OnEnterRoundWaiting();
                break;
            case GAME_PHASE.ROUND_ANSWERING:
                OnEnterRoundAnswering();
                break;
        }
    }

    void OnEnterIntro()
    {
    }

    void OnEnterRoundStart()
    {

        if (currentTale == null || roundIndex >= currentTale.datas.Length - 1)
        {
            talesIndex++;
            currentTale = tales_Datas[talesIndex];
            talesPopups[talesIndex].Enter();
            roundIndex = -1;
            ResetPassword();
        }
        ShowDoors();
        //NewRound(roundIndex + 1);
    }

    void NewRound(int index, int type)
    {
        Debug.Log("round start");

        //clear data
        foreach (var choice in choices)
        {
            DestroyImmediate(choice.gameObject);
        }
        choices.Clear();

        roundIndex = index;
        currentQuestion = currentTale.datas[index];
        questionText.text = currentQuestion.questions[type];

        //init choices
        for (int i = 0; i < currentQuestion.choices.Length; i++)
        {
            var clone = Instantiate(choice_prefab, choicesRect);
            var script = clone.GetComponent<ThaiTales_Choice>();
            script.InitChoice(this, i, currentQuestion.choices[i]);
            choices.Add(script);
        }

        choices.Shuffle();

        foreach (var choice in choices)
        {
            choice.transform.SetAsLastSibling();
        }

        ShowQuestion();
        SetPhase(GAME_PHASE.ROUND_WAITING);
        isAnswering = false;
    }

    void OnEnterRoundWaiting()
    {


    }

    void OnEnterRoundAnswering()
    {

        if (roundIndex >= currentTale.datas.Length - 1)
        {
            // show reward
            var correctCount = 0;

            foreach (var digit in digits)
            {
                var text = digit.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

                if (text == "1")
                {
                    digit.DOScale(0, 0.3f).OnComplete(() =>
                    {
                        ColorUtility.TryParseHtmlString("#35D977", out Color color);
                        digit.GetComponent<Image>().color = color;
                        digit.DOScale(1, 0.3f);
                    });
                    correctCount++;
                }
            }

            DoDelayAction(1f, () =>
            {
                HidePassword();
                rewardRect.alpha = 1;
                safe_close.alpha = 0;
                safe_open.alpha = 0;
                reward.localScale = Vector3.zero;
                fail.localScale = Vector3.zero;
                if (correctCount >= 4)
                {

                    safe_close.DOFade(1f, 0.5f).OnComplete(() =>
                    {
                        safe_close.DOFade(0f, 1f).SetDelay(1f);
                        safe_open.DOFade(1f, 1f).SetDelay(1f).OnComplete(() =>
                        {
                            DoDelayAction(1f, () =>
                            {
                                AudioManager.instance.PlaySound("ui_win_2");
                                reward.DOScale(1, 0.5f).OnComplete(() =>
                                {
                                    DoDelayAction(1f, () =>
                                    {
                                        rewardRect.alpha = 1;
                                        safe_close.alpha = 0;
                                        safe_open.alpha = 0;
                                        reward.localScale = Vector3.zero;
                                        score += 2;
                                        if (talesIndex >= tales_Datas.Length - 1)
                                        {
                                            resultText.text = "สำเร็จ\nได้รับทอง " + score.ToString() + " แท่ง";
                                            FinishedGame(true, 0);
                                        }
                                        else
                                        {
                                            SetPhase(GAME_PHASE.ROUND_START);
                                        }
                                    });
                                });
                            });
                        });
                    });
                }
                else
                {
                    safe_close.DOFade(1f, 0.5f).SetDelay(1f).OnComplete(() =>
                    {
                        DoDelayAction(1f, () =>
                        {
                            AudioManager.instance.PlaySound("ui_fail_1");
                            fail.DOScale(1, 0.5f).SetDelay(1f).OnComplete(() =>
                            {
                                DoDelayAction(1f, () =>
                                {
                                    rewardRect.alpha = 1;
                                    safe_close.alpha = 0;
                                    safe_open.alpha = 0;
                                    fail.localScale = Vector3.zero;
                                    if (talesIndex >= tales_Datas.Length - 1)
                                    {
                                        resultText.text = "สำเร็จ\nได้รับทอง " + score.ToString() + " แท่ง";
                                        FinishedGame(true, 0);
                                    }
                                    else
                                    {
                                        SetPhase(GAME_PHASE.ROUND_START);
                                    }
                                });
                            });
                        });
                    });
                }
            });

        }
        else
        {
            HidePassword();
            SetPhase(GAME_PHASE.ROUND_START);
        }
    }

    void OnDoorClick(int index)
    {
        AudioManager.instance.PlaySound("ui_click_1");
        HideDoors();
        var type = index;
        NewRound(roundIndex + 1, type);
    }

    public void OnChoiceClick(ThaiTales_Choice choice)
    {
        if (isAnswering) return;
        isAnswering = true;

        if (choice.index == 0)
        {
            //correct
            SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
            {
                //SetPhase(GAME_PHASE.ROUND_ANSWERING);
                SetPasswordScore(true);
            });
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, () =>
            {
                //SetPhase(GAME_PHASE.ROUND_ANSWERING);
                SetPasswordScore(false);
            });
        }

    }

    public void SetPasswordScore(bool val)
    {
        HideQuestion();
        ShowPassword();
        DoDelayAction(1f, () =>
        {
            var digit = digits[roundIndex];
            digit.DOScale(0, 0.3f).OnComplete(() =>
            {
                if (val)
                {
                    AudioManager.instance.PlaySound("ui_ding");
                }
                else
                {
                    AudioManager.instance.PlaySound("ui_fail_1");
                }

                digit.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = val ? 1.ToString() : 0.ToString();
                digit.DOScale(1, 0.3f).OnComplete(() =>
                {
                    DoDelayAction(1f, () =>
                    {
                        SetPhase(GAME_PHASE.ROUND_ANSWERING);
                    });
                });
            });
        });
    }

    public void ResetPassword()
    {
        foreach (var digit in digits)
        {
            digit.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "-";
            ColorUtility.TryParseHtmlString("#2B2B2B", out Color color);
            digit.GetComponent<Image>().color = color;
        }
    }

    public enum GAME_PHASE
    {
        NULL,
        INTRO,
        ROUND_START,
        ROUND_WAITING,
        ROUND_ANSWERING
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

    public Sprite GetSprite(string id)
    {
        if (spriteKeyValuePairs.ContainsKey(id))
        {
            return spriteKeyValuePairs[id];
        }
        else
        {
            return null;
        }
    }
}
