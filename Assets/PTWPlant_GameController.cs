using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;

public class PTWPlant_GameController : GameController
{
    [Header("Prefab")]
    public GameObject button_prefab;

    [Header("Obj ref")]
    public Image title;
    public RectTransform buttonRect;
    public RectTransform plantRect;
    public Image plantImg;
    public Image plantImg_trans;

    [Header("Data")]
    public PTWPlant_LeveData[] leveDatas;
    PTWPlant_LeveData currentLevelData;

    public int roundIndex = -1;

    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    List<PTW_TextButton> buttons = new();

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        tutorialPopup.Enter();

        AudioManager.instance.PlaySpacialSound("ptw_plant_info");

        tutorialPopup.OnPopupExit += OnTutorialExit;
    }
    void OnTutorialExit()
    {
        tutorialPopup.OnPopupExit = () => { };
        AudioManager.instance.StopSound(AudioManager.Channel.SPECIAL);
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
        NewRound(roundIndex + 1);
    }

    void NewRound(int index)
    {
        roundIndex = index;
        currentLevelData = leveDatas[index];

        foreach (var b in buttons)
        {
            DestroyImmediate(b.gameObject);
        }
        buttons.Clear();

        for (int i = 0; i < currentLevelData.texts.Length; i++)
        {
            var text = currentLevelData.texts[i];
            var clone = Instantiate(button_prefab, buttonRect);
            var button = clone.GetComponent<PTW_TextButton>();
            button.Setup(text, this);
            buttons.Add(button);
        }
        buttons.Shuffle();
        foreach (var b in buttons)
        {
            b.rect.SetAsLastSibling();
        }

        plantImg.sprite = currentLevelData.sprite;
        plantImg_trans.sprite = currentLevelData.sprite;

        plantRect.sizeDelta = new Vector2(plantRect.sizeDelta.x, 0);

        title.sprite = spriteKeyValuePairs["ptw_02_title_" + (roundIndex + 1).ToString("00")];

        AudioManager.instance.PlaySpacialSound("ptw_plant_" + (roundIndex + 1).ToString("00"), () =>
        {
            SetPhase(GAME_PHASE.ROUND_WAITING);
        });


    }

    void OnEnterRoundWaiting()
    {

    }

    void OnEnterRoundAnswering()
    {
        foreach (var b in buttons)
        {
            b.SetCorrect(b.text == currentLevelData.texts[0]);
        }

        AudioManager.instance.PlaySpacialSound("ptw_plant_" + (roundIndex + 1).ToString("00"), () =>
        {
            DoDelayAction(0.5f, () =>
            {
                SimpleEffectController.instance.SpawnSuccessEffect(() =>
                {
                    var allCorrect = CheckCorrect();
                    if (!allCorrect)
                    {
                        SetPhase(GAME_PHASE.ROUND_START);

                    }
                    else
                    {
                        FinishedGame(true, 0);
                    }
                });
            });
        });
        plantRect.DOSizeDelta(new Vector2(600, 600), 1f);
    }

    public bool CheckCorrect()
    {
        // var result = true;
        // foreach (var piece in pieces.Concat(breads))
        // {
        //     if (!piece.isAnswered) result = false;
        // }

        return roundIndex >= leveDatas.Length - 1;
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

    public void OnButtonClick(PTW_TextButton button)
    {
        if (button.text == currentLevelData.texts[0])
        {
            SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
            {
                SetPhase(GAME_PHASE.ROUND_ANSWERING);
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
