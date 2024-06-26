using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;

public class VTRPart2_GameController : GameController
{
    [Header("Prefab")]
    public GameObject buttonText1_prefab;

    [Header("Obj ref")]
    public PopupController soundPopup;
    public TextMeshProUGUI title;
    public CanvasGroup gameRect1;
    public TextMeshProUGUI prefixText;
    public Image questionImage;
    public RectTransform buttonRect1;

    [Header("Data")]
    public int roundIndex = -1;
    public List<VTR2_LevelData> levelDatas;
    VTR2_LevelData currentLevelData;
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

        AudioManager.instance.PlaySpacialSound("vtr_var_info");

        tutorialPopup.OnPopupExit += OnTutorialExit;

        soundPopup.OnPopupExit += OnSoundPopupExit;

    }

    void OnTutorialExit()
    {
        tutorialPopup.OnPopupExit = () => { };
        SetPhase(GAME_PHASE.ROUND_START);
        AudioManager.instance.StopSound(AudioManager.Channel.SPECIAL);
    }

    void OnSoundPopupExit()
    {
        SetPhase(GAME_PHASE.ROUND_WAITING);
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

        var spriteID = "vtr_var_" + (roundIndex + 1).ToString("00");
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

        prefixText.text = currentLevelData.prefix;

        soundPopup.Enter();

    }

    void OnEnterRoundWaiting()
    {

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

    public void OnSoundButtonClick(int index)
    {
        AudioManager.instance.StopSound(AudioManager.Channel.SPECIAL);
        AudioManager.instance.PlaySpacialSound("vtr_var_" + (roundIndex + 1).ToString("00") + "_" + index.ToString("00"));
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
