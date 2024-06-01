using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;
using Unity.VisualScripting;

public class FeelPark_GameController : GameController
{
    [Header("Prefab")]
    public GameObject stickman_prefab;
    public GameObject choice_prefab;

    [Header("Obj ref")]
    public RectTransform stickmanRect;
    public RectTransform gameRect;
    public Image feelImage;
    public TextMeshProUGUI qText;
    public TextMeshProUGUI aText;
    public RectTransform choicesRect;


    [Header("Data")]

    public int roundIndex = 0;
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();
    public List<Sprite> stickmanSprites = new();

    public FeelPark_Data[] feelPark_Datas;
    public List<FeelPark_Data> pools = new();
    public FeelPark_Data currentData;

    List<FeelPark_StickMan> stickMen = new();
    FeelPark_StickMan currentStickMan;
    List<FeelPark_Button> choices = new();

    bool isCorrected = false;

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        pools.Clear();
        pools.AddRange(feelPark_Datas.ToList());

        for (int i = 0; i < 10; i++)
        {
            var clone = Instantiate(stickman_prefab, stickmanRect);
            var stickMan = clone.GetComponent<FeelPark_StickMan>();
            var sprite = stickmanSprites.PickRandomObjects(1)[0];
            stickMan.SetUp(i, sprite, this);
            stickMen.Add(stickMan);
        }

        SetPhase(GAME_PHASE.ROUND_START);
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

    public void OnStickManClick(FeelPark_StickMan stickMan)
    {
        if (gamePhase != GAME_PHASE.ROUND_START) return;
        var index = stickMan.index;
        currentStickMan = stickMan;
        NewRound(index);
    }


    void NewRound(int index)
    {
        roundIndex = index;
        currentData = pools[roundIndex];

        foreach (var choice in choices)
        {
            DestroyImmediate(choice.gameObject);
        }
        choices.Clear();

        qText.text = currentData.question;
        aText.text = currentData.answerPrefix;

        for (int i = 0; i < currentData.choices.Length; i++)
        {
            var data = currentData.choices[i];
            var clone = Instantiate(choice_prefab, choicesRect);
            var choice = clone.GetComponent<FeelPark_Button>();
            choice.Setup(data, i == 0, this);
            choices.Add(choice);
        }

        choices.Shuffle();
        foreach (var choice in choices)
        {
            choice.SetEnable(false);
            choice.transform.SetAsLastSibling();
        }

        feelImage.sprite = spriteKeyValuePairs["feel_q_" + currentData.GetIndex().ToString("00")];

        gameRect.DOAnchorPos(Vector2.zero, 0.5f);

        AudioManager.instance.PlaySpacialSound("feel_quiz_02_" + currentData.GetIndex().ToString("00"), () =>
        {
            SetPhase(GAME_PHASE.ROUND_WAITING);
        });


    }

    void OnEnterRoundWaiting()
    {
        foreach (var choice in choices)
        {
            choice.SetEnable(true);
        }
    }

    public void OnAnswerClick(FeelPark_Button button)
    {
        if (gamePhase != GAME_PHASE.ROUND_WAITING) return;

        foreach (var choice in choices)
        {
            choice.SetEnable(false);
        }

        isCorrected = button.isCorrected;
        SetPhase(GAME_PHASE.ROUND_ANSWERING);


    }

    void OnEnterRoundAnswering()
    {
        if (isCorrected)
        {
            AudioManager.instance.PlaySpacialSound("feel_answer_02_" + currentData.GetIndex().ToString("00"), () =>
            {
                SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
                {

                    gameRect.DOAnchorPos(new Vector2(0, -1080), 0.5f).OnComplete(() =>
                    {
                        currentStickMan.SetCorrect();
                        currentStickMan = null;
                        var allCorrect = CheckCorrect();
                        if (allCorrect)
                        {
                            FinishedGame(true, 0);
                        }
                        else
                        {
                            SetPhase(GAME_PHASE.ROUND_START);
                        }
                    });
                });
            });

        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, () =>
            {
                SetPhase(GAME_PHASE.ROUND_WAITING);
            });
        }

    }

    public bool CheckCorrect()
    {
        var result = true;
        foreach (var stickman in stickMen)
        {
            if (!stickman.isCorrected) result = false;
        }

        return result;
    }
    public bool CheckTotalCorrect()
    {
        var result = true;
        // foreach (var sandwich in sandwiches)
        // {
        //     if (!sandwich.isCorrected) result = false;
        // }

        return result;
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
