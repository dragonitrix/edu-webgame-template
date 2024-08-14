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
using System;
public class ThaiJigsaw_GameController : GameController
{
    [Header("Prefab")]

    [Header("Obj ref")]
    public RectTransform[] levelButtons;
    public CanvasGroup maingameRect;
    public CanvasGroup[] jigsawGames;
    public RectTransform dragRect;
    public Image textImage;
    public CanvasGroup hintText;
    public TextMeshProUGUI hintText_level;
    public RectTransform check_true;
    public RectTransform check_false;
    public TextMeshProUGUI starsText;
    public TextMeshProUGUI resultText;

    [Header("Setting")]

    [Header("Data")]

    [TextArea(2, 2)]
    public string[] hintTexts;

    int roundIndex = -1;
    string roundID = "";
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();
    public Dictionary<string, CanvasGroup> jigsawKeyValuePairs = new();

    public int score = 0;
    int correctCount = 0;
    bool isAnswering = false;

    int answerCount = 0;

    CanvasGroup currentGame;
    RectTransform currentLevelButton;

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            var btn = levelButtons[i];
            btn.gameObject.AddComponent<ThaiJigsaw_Index>();
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnLevelClick(btn);
            });
        }

        for (int i = 0; i < jigsawGames.Length; i++)
        {
            var game = jigsawGames[i];
            // var roundID = Mathf.FloorToInt(i / 3);
            // var choiceID = i % 3;
            // jigsawKeyValuePairs.Add(roundID + "-" + choiceID, game);
            jigsawKeyValuePairs.Add(game.gameObject.name, game);
        }

        tutorialPopup.Enter();
        tutorialPopup.OnPopupExit += () =>
        {
            tutorialPopup.OnPopupExit = () => { };
            SetupRound(roundIndex + 1);
            SetPhase(GAME_PHASE.ROUND_START);
        };

    }


    public override void StartGame()
    {
        Debug.Log("start game");
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

    }


    void SetupRound(int index)
    {

        starsText.text = score.ToString("00") + "/24";

        roundIndex = index;
        answerCount = 0;

        maingameRect.interactable = false;
        maingameRect.blocksRaycasts = false;
        maingameRect.DOFade(0, 0.5f);
        isAnswering = false;

        var _choices = new List<int> { 0, 1, 2 };
        _choices.Shuffle();

        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].GetComponent<ThaiJigsaw_Index>().index = _choices[i];
            var img = levelButtons[i].GetChild(0).GetComponent<Image>();
            img.sprite = spriteKeyValuePairs["jigsaw_" + (roundIndex + 1).ToString("00") + "_" + (_choices[i] + 1).ToString("00")];
            img.SetNativeSize();
            levelButtons[i].GetComponent<Button>().interactable = true;
            levelButtons[i].GetComponent<Animator>().enabled = true;
        }

        hintText.GetComponent<TextMeshProUGUI>().text = hintTexts[index];
        hintText_level.text = hintTexts[index];

    }

    void OnLevelClick(RectTransform rect)
    {
        if (isAnswering) return;
        isAnswering = true;
        currentLevelButton = rect;
        var index = rect.GetComponent<ThaiJigsaw_Index>();
        NewRound((roundIndex + 1).ToString("00") + "-" + (index.index + 1).ToString("00"));
    }

    void NewRound(string id)
    {
        isAnswering = false;

        maingameRect.interactable = true;
        maingameRect.blocksRaycasts = true;
        maingameRect.DOFade(1, 0.5f);

        roundID = id;

        correctCount = 0;

        currentGame = jigsawKeyValuePairs[id];
        currentGame.TotalShow();

        //setup
        var dropsRect = currentGame.transform.GetChild(1).transform;
        var drops = new List<GameObject>();
        for (int i = 0; i < dropsRect.childCount; i++)
        {
            var child = dropsRect.GetChild(i).gameObject;
            var index = child.AddComponent<ThaiJigsaw_Index>();
            index.index = i;
            drops.Add(child);
        }

        for (int i = dragRect.childCount - 1; i >= 0; i--)
        {
            // Destroy the child GameObject immediately
            DestroyImmediate(dragRect.GetChild(i).gameObject);
        }

        var drags = new List<Transform>();

        foreach (var drop in drops)
        {
            drop.GetComponent<Droppable>().onDropped += OnDrop;

            var clone = Instantiate(drop, dragRect);
            clone.GetComponent<Droppable>().enabled = false;
            var drag = clone.AddComponent<Draggable>();
            drag.dragableBG = clone.GetComponent<Image>();
            drags.Add(drag.transform);

            drop.GetComponent<Image>().DOFade(0, 0);
        }

        drags.Shuffle();
        foreach (var drag in drags)
        {
            drag.SetAsLastSibling();
        }

        textImage.DOFade(0, 0);
        hintText.DOFade(0, 0);
        SetPhase(GAME_PHASE.ROUND_WAITING);
    }

    public void OnDrop(Droppable dropable, Draggable dragable)
    {
        var drop = dropable.GetComponent<ThaiJigsaw_Index>();
        var drag = dragable.GetComponent<ThaiJigsaw_Index>();

        if (drop.index == drag.index)
        {
            AudioManager.instance.PlaySound("ui_ding");
            drag.GetComponent<RectTransform>().DOScale(0, 0.3f);
            drop.GetComponent<Image>().DOFade(1, 0);
            drop.GetComponent<Image>().raycastTarget = false;
            drop.GetComponent<RectTransform>().DOScale(1, 0.3f).From(0);

            correctCount++;

            var dropsRect = currentGame.transform.GetChild(1).transform;
            if (correctCount == dropsRect.childCount)
            {
                AudioManager.instance.StopSound("ui_ding");
                SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
                {
                    JigsawAnswering();
                });
            }
        }
    }

    void JigsawAnswering()
    {
        var dropsRect = currentGame.transform.GetChild(1).transform;
        dropsRect.GetComponent<CanvasGroup>().DOFade(0, 1f).OnComplete(() =>
        {
            var guideID = "jigsaw_" + roundID.Replace("-", "_") + "_ans";
            var guideImg = spriteKeyValuePairs["jigsaw_" + roundID.Replace("-", "_") + "_ans"];
            textImage.sprite = guideImg;
            textImage.SetNativeSize();
            currentGame.transform.GetChild(0).GetComponent<Image>().DOFade(0, 0.5f);
            hintText.DOFade(1, 1);
            textImage.DOFade(1, 1).OnComplete(() =>
            {
                DoDelayAction(1f, () =>
                {
                    SetPhase(GAME_PHASE.ROUND_ANSWERING);
                });
            });
        });
    }

    void OnEnterRoundWaiting()
    {

    }


    void OnEnterRoundAnswering()
    {
        //Debug.Log(roundID);
        //
        var r = roundID.Split("-");
        if (r[1] == "01")
        {
            AudioManager.instance.PlaySound("ui_win_1");
            check_true.DOScale(1, 0.5f);

            DoDelayAction(1f, () =>
            {
                maingameRect.TotalHide();
                maingameRect.DOFade(0, 0.5f).From(1);
                check_true.DOScale(0, 0f);
                check_false.DOScale(0, 0f);
                currentGame.TotalHide();
                var score = answerCount == 0 ? 3 : 2 - answerCount;
                this.score += score;

                if (roundIndex >= hintTexts.Length - 1)
                {
                    resultText.text = resultText.text.Replace("[x]", this.score.ToString("00") + "/24");
                    FinishedGame(true, 0);
                }
                else
                {
                    SetupRound(roundIndex + 1);
                }

            });
        }
        else
        {
            answerCount++;
            AudioManager.instance.PlaySound("ui_fail_1");
            check_false.DOScale(1, 0.5f);
            DoDelayAction(1f, () =>
            {
                maingameRect.TotalHide();
                maingameRect.DOFade(0, 0.5f).From(1);
                check_true.DOScale(0, 0f);
                check_false.DOScale(0, 0f);
                currentLevelButton.GetComponent<Button>().interactable = false;
                currentLevelButton.GetComponent<Animator>().enabled = false;
                currentLevelButton.DOScale(0, 0.5f);
                currentGame.TotalHide();
                SetPhase(GAME_PHASE.ROUND_START);
            });
        }

        // if (roundIndex >= choices.Length - 1)
        // {
        //     FinishedGame(true, 0);
        // }
        // else
        // {
        //     SetPhase(GAME_PHASE.ROUND_START);
        // }
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

public class ThaiJigsaw_Index : MonoBehaviour
{
    public int index;
}
