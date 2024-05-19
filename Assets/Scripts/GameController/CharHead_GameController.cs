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

public class CharHead_GameController : GameController
{
    [Header("Prefab")]
    public GameObject part_prefab;

    [Header("Obj ref")]
    List<CharHead_Char> chars = new();
    List<CharHead_Part> parts = new();

    public RectTransform levelTitleRect;
    public Image levelTitle;
    public RectTransform shelfRect;
    public RectTransform shelf_2;
    public RectTransform shelf_3;
    public RectTransform shelf_5;
    public RectTransform bucketRect;
    public RectTransform partsBucket;
    public RectTransform labelRect;
    public Image labelImage;
    public RectTransform rewardRect;
    public Image rewardImage;

    public CanvasGroup finalCanvasGroup;
    public RectTransform finalLabel;
    public RectTransform finalReward;

    [Header("Data")]
    public CharHead_LevelData[] levelDatas;
    CharHead_LevelData currentLevelData;
    CharHead_RoundData currentRoundData;
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    bool isCorrectAnswer = false;
    int currentScore = 0;
    int maxScore = 0;

    [SerializeField]
    int roundIndex = -1;
    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);

        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);
        maxScore = 0;
        int ms = maxScore;
        currentLevelData = levelDatas[0];

        levelTitleRect.localScale = Vector2.zero;

        tutorialPopup.Enter();

        tutorialPopup.OnPopupExit += () =>
        {
            tutorialPopup.OnPopupExit = ()=>{};
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
        NewRound();
    }

    void NewRound()
    {
        roundIndex++;
        var roundData = currentLevelData.rounds[roundIndex];
        currentRoundData = roundData;

        ClearCharList();
        ClearPartList();

        RectTransform targetShelf = shelf_2;

        switch (currentRoundData.chars.Length)
        {
            case 2:
                targetShelf = shelf_2;
                break;
            case 3:
                targetShelf = shelf_3;
                break;
            case 5:
                targetShelf = shelf_5;
                break;
        }

        shelf_2.gameObject.SetActive(false);
        shelf_3.gameObject.SetActive(false);
        shelf_5.gameObject.SetActive(false);
        targetShelf.gameObject.SetActive(true);

        var parts = new List<CHARHEAD_PART_TYPE>();

        foreach (var _char in currentRoundData.chars)
        {
            var clone = Instantiate(_char, targetShelf);
            var charScript = clone.GetComponent<CharHead_Char>();
            charScript.index = _char.name.Split(" ")[0];
            charScript.InitChar();
            parts.AddRange(charScript.GetAllParts());

            chars.Add(charScript);
        }

        // Debug.Log("fetch part: " + parts.Count);

        foreach (var part in parts)
        {
            var clone = Instantiate(part_prefab, partsBucket);
            var partScript = clone.GetComponent<CharHead_Part>();
            partScript.SetPart(part);
            partScript.InitPart();
            partScript.draggable.enabled = false;

            this.parts.Add(partScript);
        }

        levelTitleRect.DOScale(Vector2.zero, 0.1f).OnComplete(() =>
        {
            var titleID = "chh_title_" + (roundIndex + 1).ToString("00");
            levelTitle.sprite = spriteKeyValuePairs[titleID];
            levelTitle.SetNativeSize();
            levelTitleRect.DOScale(Vector2.one, 0.1f);
        });

        labelRect.localScale = Vector2.zero;
        rewardRect.anchoredPosition = new Vector2(1500, 0);

        TweenGameElementIn(true);

        var soundID = "chh_level_" + (roundIndex + 1).ToString("00");
        AudioManager.instance.PlaySpacialSound(soundID, () =>
        {
            //play sound sequence
            PlayCharsSound(0.5f, () =>
            {
                SetPhase(GAME_PHASE.ROUND_WAITING);
            });
        });

    }

    void ClearCharList()
    {
        foreach (var cha in this.chars)
        {
            Destroy(cha.gameObject);
        }
        this.chars.Clear();
    }

    void ClearPartList()
    {
        foreach (var part in this.parts)
        {
            Destroy(part.gameObject);
        }
        this.parts.Clear();
    }
    void OnEnterRoundWaiting()
    {
        // enable drag 
        SetEnableDrag(true);
    }

    void OnEnterRoundAnswering()
    {
        bucketRect.DOAnchorPos(new Vector2(1500, 0), 0.2f);

        labelImage.sprite = spriteKeyValuePairs["chh_clear"];
        labelRect.DOScale(Vector2.one, 0.2f);

        var rewardSoundID = "chh_success";
        AudioManager.instance.PlaySpacialSound(rewardSoundID, () =>
        {
            labelRect.DOScale(Vector2.zero, 0.2f).OnComplete(() =>
            {
                var rewardSpriteID = "chh_reward_" + (roundIndex + 1).ToString("00");
                rewardImage.sprite = spriteKeyValuePairs[rewardSpriteID];
                rewardImage.SetNativeSize();

                var rewardTitleID = "chh_reward_title_" + (roundIndex + 1).ToString("00");
                labelImage.sprite = spriteKeyValuePairs[rewardTitleID];

                var rewardSoundID = "chh_reward_" + (roundIndex + 1).ToString("00");
                AudioManager.instance.PlaySpacialSound(rewardSoundID, OnRewardSoundFinished);

                labelRect.DOScale(Vector2.one, 0.2f);
                rewardRect.DOAnchorPos(Vector2.zero, 0.2f);

            });
        });
    }

    void OnRewardSoundFinished()
    {
        rewardRect.DOAnchorPos(new Vector2(1500, 0), 0.2f).SetDelay(1f);
        shelfRect.DOAnchorPos(new Vector2(-1500, 0), 0.2f).SetDelay(1f).OnComplete(() =>
        {
            // API_END_GAME
            if (roundIndex + 1 >= currentLevelData.rounds.Length)
            {
                FinishedGameSequence();
            }
            else
            {
                SetPhase(GAME_PHASE.ROUND_START);
            }
        });
    }

    void FinishedGameSequence()
    {
        labelRect.DOScale(Vector2.zero, 0.2f);

        finalCanvasGroup.DOFade(1f, 1f);

        finalLabel.DOScale(Vector2.one, 0.2f);

        AudioManager.instance.PlaySpacialSound("chh_success_final", () =>
        {
            finalLabel.DOScale(Vector2.zero, 0.2f);
            finalReward.DOScale(Vector2.one, 0.2f);
            DoDelayAction(1f, () =>
            {
                AudioManager.instance.PlaySpacialSound("chh_reward_final", () =>
                {
                    // finished game
                    // finishText.text = "คะแนนรวม : " + currentScore + "/" + maxScore;
                    FinishedGame(true, currentScore);
                });
            });
        });
    }

    public void ForceToNextGame()
    {
        // to Wannayuuk game
        GameManager.instance.SetTargetGame(SUBGAME_INDEX.WANNAYUUK);
    }

    void TweenGameElementIn(bool val)
    {
        if (val)
        {
            shelfRect.DOAnchorPos(Vector2.zero, 0.2f);
            bucketRect.DOAnchorPos(Vector2.zero, 0.2f);
        }
        else
        {
            shelfRect.DOAnchorPos(new Vector2(-1500, 0), 0.2f);
            bucketRect.DOAnchorPos(new Vector2(1500, 0), 0.2f);
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

    public void OnSubmitClicked()
    {
        if (isPlayingSound) return;
        var result = true;
        foreach (var _char in chars)
        {
            if (!_char.correctStatus) result = false;
        }
        isCorrectAnswer = result;

        switch (isCorrectAnswer)
        {
            case true:
                break;
            case false:
                break;
        }
        SimpleEffectController.instance.SpawnAnswerEffect(isCorrectAnswer, () =>
        {
            if (isCorrectAnswer)
            {
                SetPhase(GAME_PHASE.ROUND_ANSWERING);
            }
        });
    }

    public void OnPlaySoundClicked()
    {
        if (isPlayingSound) return;
        SetEnableDrag(false);
        PlayCharsSound(0, () =>
        {
            SetEnableDrag(true);
        });
    }

    bool isPlayingSound = false;

    void PlayCharsSound(float delay, UnityAction callback)
    {
        if (isPlayingSound) return;
        StartCoroutine(_PlayCharsSound(delay, callback));
    }

    IEnumerator _PlayCharsSound(float delay, UnityAction callback)
    {
        isPlayingSound = true;
        yield return new WaitForSeconds(delay);
        foreach (var _char in chars)
        {
            var name = _char.index;
            var clipname = "chh_" + name;
            var clipLength = AudioManager.instance.GetClipLength(clipname);
            AudioManager.instance.PlaySpacialSound(clipname);
            _char.HightLight(true);
            yield return new WaitForSeconds(clipLength);
            _char.HightLight(false);
            yield return new WaitForSeconds(1);
        }
        callback();
        isPlayingSound = false;
    }

    string GetSoundID(SOUNDID_TYPE type, int index = 0)
    {
        return null;
    }

    public enum SOUNDID_TYPE
    {
        HINT,
        WORD,
        ANSWER
    }

    void SetEnableDrag(bool val)
    {
        foreach (var part in parts)
        {
            if (!part.isDrop)
                part.draggable.enabled = val;
        }

    }

}
