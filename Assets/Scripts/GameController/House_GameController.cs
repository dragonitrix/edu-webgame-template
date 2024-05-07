using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;

public class House_GameController : GameController
{
    [Header("Prefabs")]
    public GameObject smallCard_prefab;

    [Header("intro")]
    public SimpleBatchTweenController[] intros;
    SimpleBatchTweenController intro;

    [Header("Obj Ref")]
    public House_LevelPresetController levelPreset;
    public House_CardController mainCard;
    public RectTransform mainCard_rect;
    public CanvasGroup mainCard_underlay;
    public Draggable mainCard_draggable;

    [Header("Data")]
    public House_LevelData[] levelDatas;
    public List<Sprite> levelSprites;
    House_LevelSettings levelSettings;
    House_LevelData currentLevelData;
    House_RoundData currentRoundData;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    bool firstTutorial = true;
    bool isCorrectAnswer = false;
    int currentScore = 0;
    int maxScore = 0;
    int roundIndex = -1;

    House_CardSmall currentAnswerCard;
    House_HouseBig currentBigHouse;

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(4, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);
        tutorialPopup.OnPopupExit += OnTutPopupExit;

        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        var level = (HOUSE_LEVEL)gameLevel;
        levelSettings = new House_LevelSettings(level);
        currentScore = 0;

        intro = intros[gameLevel];
        currentLevelData = levelDatas[gameLevel];
        maxScore = currentLevelData.rounds.Length;

        var houseDatas = new List<HouseData>();

        for (int i = 0; i < currentLevelData.houseTexts.Length; i++)
        {
            string prefix = "house";
            string _levelIndex = "_" + (gameLevel + 1).ToString("00");
            string _roundIndex = "_" + (i + 1).ToString("00");
            string imgID = prefix + _levelIndex + _roundIndex;
            houseDatas.Add(new HouseData(imgID, currentLevelData.houseTexts[i]));
        }


        switch (level)
        {
            case HOUSE_LEVEL._1:
            case HOUSE_LEVEL._2:
            case HOUSE_LEVEL._3:
            case HOUSE_LEVEL._4:
                levelPreset.SetUpHouses(houseDatas);
                break;
            case HOUSE_LEVEL._5:
            case HOUSE_LEVEL._6:
                levelPreset.SetUpHousesSpacial(houseDatas);
                break;
        }

        foreach (var droparea in levelPreset.GetDropArea())
        {
            droparea.onDropped += OnDrop;
        }

        SetDisplayRoundElement(false);
    }

    void ToNextLevelButtonEvent()
    {
        GameManager.instance.gameLevel++;
        GameManager.instance.ReloadScene();
    }

    public override void StartGame()
    {
        Debug.Log("start game");
        gameState = GAME_STATE.STARTED;
        // do command
        tutorialPopup.Enter();
        //tutorialPopup.closeButton.gameObject.SetActive(false);
        DoDelayAction(0.5f, () =>
        {
            AudioManager.instance.PlaySpacialSound("hou_tut", OnTutSoundFinished);
        });
    }

    void OnTutPopupExit()
    {
        if (!firstTutorial) return;
        firstTutorial = false;
        AudioManager.instance.StopSound("hou_tut", AudioManager.Channel.SPECIAL);
        SetPhase(GAME_PHASE.INTRO);
    }

    void OnTutSoundFinished()
    {
        tutorialPopup.closeButton.gameObject.SetActive(true);
    }

    void OnIntro1SoundFinished()
    {
        DoDelayAction(1f, () => { SetPhase(GAME_PHASE.ROUND_START); });
        //Debug.Log("OnIntro1SoundFinished");
        //DoDelayAction(1f, () =>
        //{
        //    AudioManager.instance.PlaySpacialSound(levelSettings.intro_soundid + "_02", OnIntro2SoundFinished);
        //});
    }
    void OnIntro2SoundFinished()
    {
        Debug.Log("OnIntro2SoundFinished");
        DoDelayAction(1f, () => { SetPhase(GAME_PHASE.ROUND_START); });
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
                intro.Exit();
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
        Debug.Log("OnEnterIntro");
        AudioManager.instance.PlaySpacialSound(levelSettings.intro_soundid + "_01", OnIntro1SoundFinished);
        intro.Enter();
        SetDisplayRoundElement(false);
    }

    void OnEnterRoundStart()
    {
        NewRound();
    }

    void OnEnterRoundWaiting()
    {
        mainCard_draggable.enabled = true;
        DragManager.instance.GetAllDropable();
    }

    void OnEnterRoundAnswering()
    {
        if (isCorrectAnswer)
        {
            AudioManager.instance.PlaySound("ui_win_1");
            currentScore++;
            isCorrectAnswer = false;
        }
        else
        {
            AudioManager.instance.PlaySound("ui_fail_1");
            currentAnswerCard.ExitAndKill();
        }



        // transition out and new round
        DoDelayAction(1f, () =>
        {
            currentBigHouse.Exit();
            if (roundIndex + 1 >= currentLevelData.rounds.Length)
            {
                // finished game
                // finishText.text = "คะแนนรวม : " + currentScore + "/" + maxScore;
                FinishedGame(true, currentScore);
            }
            else
            {
                SetPhase(GAME_PHASE.ROUND_START);
            }
        });

    }


    void NewRound()
    {
        roundIndex++;
        var roundData = currentLevelData.rounds[roundIndex];
        currentRoundData = roundData;

        // get card image id
        mainCard.SetFrontImage(GetCardImg());

        mainCard_rect.GetComponent<CanvasGroup>().DOFade(1f, 0.5f).From(0f);
        mainCard_rect.anchoredPosition = Vector2.zero;
        mainCard_underlay.DOFade(1f, 0.5f).From(0f);

        mainCard.FlipTo(true, OnMainCardFlipFinished, 1f);

    }
    void OnMainCardFlipFinished()
    {
        Debug.Log("OnMainCardFlipFinished (Game Controller)");
        AudioManager.instance.PlaySpacialSound(GetSoundID(SOUNDID_TYPE.HINT), () =>
        {
            DoDelayAction(1f, () =>
            {
                AudioManager.instance.PlaySpacialSound(GetSoundID(SOUNDID_TYPE.HINT), OnMainCardHintSoundFinished);
            });
        });
    }

    void OnMainCardHintSoundFinished()
    {
        // tween main card to side
        mainCard_underlay.DOFade(0f, 0.5f).From(1f);
        mainCard_rect.DOAnchorPos(new Vector2(600, 0), 0.5f);

        // tween dropable house
        levelPreset.smallHouseCanvasGroup.DOFade(1f, 0.5f);

        // play dropable house sound
        DoDelayAction(1f, () =>
        {
            AudioManager.instance.PlaySpacialSound(GetSoundID(SOUNDID_TYPE.HOUSE), OnHouseHintSoundFinished);
        });
    }

    void OnHouseHintSoundFinished()
    {
        SetPhase(GAME_PHASE.ROUND_WAITING);
    }

    void SetDisplayRoundElement(bool val)
    {
        if (val)
        {
            mainCard_rect.GetComponent<CanvasGroup>().alpha = 1f;
        }
        else
        {
            mainCard_rect.GetComponent<CanvasGroup>().alpha = 0f;
            levelPreset.smallHouseCanvasGroup.alpha = 0f;
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

    void OnDrop(Droppable droppable, Draggable draggable)
    {
        AudioManager.instance.PlaySound("ui_highlight_1");
        var house = droppable.GetComponentInParent<House_HouseSmall>();
        var current_answer = house.index;
        var correct_answer = currentRoundData.answer;

        isCorrectAnswer = current_answer == correct_answer;

        var houseBig = levelPreset.houseBigs[current_answer];
        var clone_smallCard = Instantiate(smallCard_prefab, houseBig.gridGroup);
        var smallCard = clone_smallCard.GetComponent<House_CardSmall>();
        smallCard.SetImage(GetCardImg());
        smallCard.SetText(currentRoundData.text);
        houseBig.Enter();

        currentAnswerCard = smallCard;
        currentBigHouse = houseBig;

        // show check mark or something
        currentAnswerCard.ShowMark(isCorrectAnswer);

        AudioManager.instance.PlaySpacialSound(GetSoundID(SOUNDID_TYPE.ANSWER, current_answer + 1), OnAnswerSoundFinished);
        SetDisplayRoundElement(false);
    }

    void OnAnswerSoundFinished()
    {
        SetPhase(GAME_PHASE.ROUND_ANSWERING);
    }

    Sprite GetCardImg()
    {
        string prefix = "hou_card";
        string _levelIndex = "_" + (gameLevel + 1).ToString("00");
        string _roundIndex = "_" + (roundIndex + 1).ToString("00");
        string imgID = prefix + _levelIndex + _roundIndex;
        return spriteKeyValuePairs[imgID];
    }

    string GetSoundID(SOUNDID_TYPE type, int index = 0)
    {
        var prefix = "hou_";
        string _levelIndex = "_" + (gameLevel + 1).ToString("00");
        string _roundIndex = "_" + (roundIndex + 1).ToString("00");

        switch (type)
        {
            case SOUNDID_TYPE.HINT:
                prefix += "hint";
                return prefix + _levelIndex + _roundIndex;
            case SOUNDID_TYPE.HOUSE:
                prefix += "house";
                return prefix + _levelIndex;
            case SOUNDID_TYPE.ANSWER:
                prefix += "answer";
                return prefix + _levelIndex + _roundIndex + "_" + index.ToString("00");
        }

        return null;
    }

    public enum SOUNDID_TYPE
    {
        HINT,
        HOUSE,
        ANSWER
    }

}

public class HouseData
{
    public string spriteID;
    public string text;

    public HouseData(string spriteID, string text)
    {
        this.spriteID = spriteID;
        this.text = text;
    }
}
