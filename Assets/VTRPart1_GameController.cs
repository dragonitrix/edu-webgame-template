using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;

public class VTRPart1_GameController : GameController
{
    [Header("Prefab")]
    public GameObject buttonText1_prefab;
    public GameObject buttonText2_prefab;

    [Header("Obj ref")]
    public TextMeshProUGUI title;

    public CanvasGroup gameRect1;
    public CanvasGroup gameRect2;

    public Image questionImage;
    public RectTransform buttonRect1;
    public RectTransform buttonRect2;

    [Header("Data")]
    public int roundIndex = -1;
    public List<VTR1_LevelData> levelDatas;
    VTR1_LevelData currentLevelData;
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    List<VTR_TextButton> buttons = new();

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);
        // SetPhase(GAME_PHASE.ROUND_START);

        tutorialPopup.Enter();

        AudioManager.instance.PlaySpacialSound("vtr_this_info");

        tutorialPopup.OnPopupExit += OnTutorialExit;
    }

    void OnTutorialExit()
    {
        tutorialPopup.OnPopupExit = () => { };
        SetPhase(GAME_PHASE.ROUND_START);
        AudioManager.instance.StopSound(AudioManager.Channel.SPECIAL);
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
        currentLevelData = levelDatas[index];


        foreach (var b in buttons)
        {
            DestroyImmediate(b.gameObject);
        }
        buttons.Clear();


        title.text = "ข้อที่ " + (roundIndex + 1).ToString();

        switch (currentLevelData.type)
        {
            case VTR1_LevelData.TYPE.IMG:
                gameRect1.TotalShow();
                gameRect2.TotalHide();

                var spriteID = "vtr_this_" + (roundIndex + 1).ToString("00");
                questionImage.sprite = spriteKeyValuePairs[spriteID];

                for (int i = 0; i < currentLevelData.texts.Length; i++)
                {
                    var text = currentLevelData.texts[i];
                    var clone = Instantiate(buttonText1_prefab, buttonRect1);
                    var button = clone.GetComponent<VTR_TextButton>();
                    button.Setup(text, this);
                    buttons.Add(button);
                }

                buttons.Shuffle();
                foreach (var b in buttons)
                {
                    b.rect.SetAsLastSibling();
                }
                SetPhase(GAME_PHASE.ROUND_WAITING);

                break;
            case VTR1_LevelData.TYPE.SOUND:
                gameRect2.TotalShow();
                gameRect1.TotalHide();

                var soundID = "vtr_this_" + (roundIndex + 1).ToString("00");

                for (int i = 0; i < currentLevelData.texts.Length; i++)
                {
                    var text = currentLevelData.texts[i];
                    var clone = Instantiate(buttonText2_prefab, buttonRect2);
                    var button = clone.GetComponent<VTR_TextButton>();
                    button.Setup(text, this);
                    buttons.Add(button);
                }

                buttons.Shuffle();
                foreach (var b in buttons)
                {
                    b.SetDisplay(false);
                    b.rect.SetAsLastSibling();
                }

                AudioManager.instance.PlaySpacialSound(soundID, () =>
                {
                    SetPhase(GAME_PHASE.ROUND_WAITING);
                });
                break;
        }

    }

    void OnEnterRoundWaiting()
    {
        if (currentLevelData.type == VTR1_LevelData.TYPE.SOUND)
        {
            foreach (var b in buttons)
            {
                b.SetDisplay(true, 0.1f);
            }
        }
    }

    void OnEnterRoundAnswering()
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
    }

    public bool CheckCorrect()
    {
        return roundIndex >= levelDatas.Count - 1;
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

    public void OnButtonClick(VTR_TextButton button)
    {
        if (gamePhase != GAME_PHASE.ROUND_WAITING) return;

        if (button.text == currentLevelData.texts[0])
        {
            foreach (var b in buttons)
            {
                b.SetCorrect(b.text == currentLevelData.texts[0]);
            }
            SimpleEffectController.instance.SpawnAnswerEffectMinimal(true, () =>
            {
                SetPhase(GAME_PHASE.ROUND_ANSWERING);
            }, 0.5f);
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffectMinimal(false, () =>
            {
                SetPhase(GAME_PHASE.ROUND_WAITING);
            }, 0.5f);
        }
    }

    public void OnSoundButtonClick()
    {
        SetPhase(GAME_PHASE.NULL);
        AudioManager.instance.StopSound(AudioManager.Channel.SPECIAL);
        var soundID = "vtr_this_" + (roundIndex + 1).ToString("00");
        AudioManager.instance.PlaySpacialSound(soundID, () =>
        {
            SetPhase(GAME_PHASE.ROUND_WAITING);
        });
    }

    public void ForceToNextGame()
    {
        // to room hidden game
        GameManager.instance.SetTargetGame(SUBGAME_INDEX.VTR_PART2);
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
