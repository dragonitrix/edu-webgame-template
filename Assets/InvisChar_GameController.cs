using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;

public class InvisChar_GameController : GameController
{
    [Header("Prefab")]
    public GameObject numberButton_prefab;
    public GameObject dropChar_prefab;
    public GameObject dragChar_prefab;
    public GameObject fixedText_prefab;
    public GameObject plusText_prefab;

    [Header("Obj ref")]


    public RectTransform numberRect;
    public RectTransform gamePanel;
    public RectTransform mainCharRect;
    public RectTransform dragCharRect;
    public TextMeshProUGUI resultText;
    public Image resultImage;
    public TextMeshProUGUI hintText;

    [Header("Data")]

    public Color[] colors;

    public InvisChar_QuestionData[] levelDatas;
    InvisChar_QuestionData currentLevelData;

    int roundIndex = -1;
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    List<InvisChar_ButtonNumber> buttonNumbers = new();
    List<InvisChar_Drag> dragareas = new();
    List<InvisChar_Droparea> dropareas = new();

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        for (int i = 0; i < levelDatas.Length; i++)
        {
            var data = levelDatas[i];
            var clone = Instantiate(numberButton_prefab, numberRect);
            var number = clone.GetComponent<InvisChar_ButtonNumber>();
            number.Setup(i, this);
            buttonNumbers.Add(number);
        }

        // SetPhase(GAME_PHASE.ROUND_START);

        tutorialPopup.Enter();
        AudioManager.instance.PlaySpacialSound("mhw_tutorial_01");

        tutorialPopup.OnPopupExit += () =>
        {
            AudioManager.instance.StopSound(AudioManager.Channel.SPECIAL);
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

    void NewRound(int index)
    {
        roundIndex = index;

        var levelData = levelDatas[index];
        currentLevelData = levelData;


        //clear previous roundData
        foreach (var drag in dragareas)
        {
            DestroyImmediate(drag.gameObject);
        }
        dragareas.Clear();
        foreach (var drop in dropareas)
        {
            DestroyImmediate(drop.gameObject);
        }
        dropareas.Clear();

        // clear remaining
        for (int i = mainCharRect.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(mainCharRect.GetChild(i).gameObject);
        }


        // create question section
        for (int i = 0; i < levelData.data.Length; i++)
        {
            var c = levelData.data[i];
            switch (c.type)
            {
                case InvisChar_PartData.TYPE.DROP:
                    var dropclone = Instantiate(dropChar_prefab, mainCharRect);
                    var drop = dropclone.GetComponent<InvisChar_Droparea>();
                    drop.Setup(c.text, this);
                    drop.droppable.onDropped += OnDrop;
                    var type = GetCharType(c.text);
                    Debug.Log(type);
                    drop.image.color = colors[(int)type - 1];
                    dropareas.Add(drop);
                    break;
                case InvisChar_PartData.TYPE.FIX:
                    var fixclone = Instantiate(fixedText_prefab, mainCharRect);
                    var fixtext = fixclone.GetComponent<TextMeshProUGUI>();
                    fixtext.text = c.text;
                    break;
            }

            if (i < levelData.data.Length - 1)
            {
                var plusclone = Instantiate(plusText_prefab, mainCharRect);
            }
        }

        // create drag character section
        var pool = new List<string>();
        for (int i = 0; i < dropareas.Count; i++)
        {
            var drop = dropareas[i];
            var text = drop.correctChar;
            var type = GetCharType(text);
            var listType = ToListString(GetCharPool(text));

            pool.Add(text);
            var avoidPool = new List<string> { text };

            // censored
            if (text != "ห")
            {
                avoidPool.Add("ห");
            }

            pool.AddRange(listType.PickRandomObjects(2, avoidPool));
        }

        for (int i = 0; i < pool.Count; i++)
        {
            var c = pool[i];
            var clone = Instantiate(dragChar_prefab, dragCharRect);
            var drag = clone.GetComponent<InvisChar_Drag>();
            drag.SetText(c);
            drag.SetEnable(false);
            dragareas.Add(drag);
        }

        dragareas.Shuffle();
        foreach (var drag in dragareas)
        {
            drag.transform.SetAsLastSibling();
        }


        resultImage.sprite = spriteKeyValuePairs["mhw_obj_" + (roundIndex + 1).ToString("00")];

        UpdateResultText();
        UpdateResultImageAlpha();

        hintText.text = "คำใบ้: " + levelData.hintText;

        gamePanel.DOAnchorPos(Vector2.zero, 0.5f);

        AudioManager.instance.PlaySound("ui_swipe");

        var hintSoundID = "mhw_hint_" + (roundIndex + 1).ToString("00");
        AudioManager.instance.PlaySpacialSound(hintSoundID, () =>
        {
            SetPhase(GAME_PHASE.ROUND_WAITING);
        });

    }

    void OnEnterRoundWaiting()
    {
        foreach (var d in dragareas)
        {
            d.SetEnable(true);
        }
    }

    void OnEnterRoundAnswering()
    {
        var allCorrect = CheckCorrect();

        if (!allCorrect)
        {
            SetPhase(GAME_PHASE.ROUND_WAITING);
        }
        else
        {
            // question answer complete
            gamePanel.DOAnchorPos(new Vector2(0, -1080), 0.5f).OnComplete(() =>
            {
                buttonNumbers[roundIndex].SetCorrect();

                var totalCorrect = CheckTotalCorrect();

                if (totalCorrect)
                {
                    FinishedGame(true, 0);
                }
                else
                {
                    SetPhase(GAME_PHASE.ROUND_START);
                }

            });
        }
    }

    [ContextMenu("Cheat")]
    public void Cheat()
    {
        FinishedGame(true, 0);
    }

    public bool CheckCorrect()
    {
        var result = true;

        foreach (var d in dropareas)
        {
            if (!d.isCorrected) result = false;
        }

        return result;
    }

    public bool CheckTotalCorrect()
    {
        var result = true;

        foreach (var btn in buttonNumbers)
        {
            if (!btn.isCorrected) result = false;
        }

        return result;
    }

    public void OnNumberClick(InvisChar_ButtonNumber number)
    {
        NewRound(number.index);
    }

    public void OnDrop(Droppable dropable, Draggable dragable)
    {
        var drop = dropable.GetComponent<InvisChar_Droparea>();
        var drag = dragable.GetComponent<InvisChar_Drag>();

        if (drag.currentChar == drop.correctChar)
        {
            //magic
            drop.SetCorrect();
            drag.SetEnable(false);
            drag.SetVisible(false);

            SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
            {
                SetPhase(GAME_PHASE.ROUND_ANSWERING);
            });

            UpdateResultText();
            UpdateResultImageAlpha();

        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, () => { });
        }

    }

    void UpdateResultText()
    {
        var text = currentLevelData.fulltext;

        for (int i = 0; i < dropareas.Count; i++)
        {
            var drop = dropareas[i];
            text = text.Replace(i.ToString(), drop.isCorrected ? drop.correctChar : " ");
        }

        resultText.text = text;

    }

    void UpdateResultImageAlpha()
    {
        var correctCount = 0f;
        for (int i = 0; i < dropareas.Count; i++)
        {
            var drop = dropareas[i];
            if (drop.isCorrected) correctCount++;
        }

        var ratio = correctCount / dropareas.Count;

        resultImage.SetAlpha(Mathf.Lerp(0.25f, 1f, ratio));

    }

    public void ForceToNextGame()
    {
        // to room hidden game
        GameManager.instance.SetTargetGame(SUBGAME_INDEX.ROOM_HIDDEN);
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

    public enum CHAR_LEVEL
    {
        NULL = 0,
        HIGH, MID, LOW
    }

    static string chars_high = "ขฃฉฐถผฝศษสห";
    static string chars_mid = "กจฎฏดตบปอ";
    static string chars_low = "คฅฆงชซฌญฑฒณทธนพฟภมรลวฬฮ";

    public CHAR_LEVEL GetCharType(string input)
    {
        var level = CHAR_LEVEL.NULL;

        if (chars_high.Contains(input)) return CHAR_LEVEL.HIGH;
        if (chars_mid.Contains(input)) return CHAR_LEVEL.MID;
        if (chars_low.Contains(input)) return CHAR_LEVEL.LOW;

        return level;
    }

    public string GetCharPool(string text)
    {
        return GetCharPool(GetCharType(text));
    }
    public string GetCharPool(CHAR_LEVEL level)
    {
        switch (level)
        {
            case CHAR_LEVEL.HIGH: return chars_high;
            case CHAR_LEVEL.MID: return chars_mid;
            case CHAR_LEVEL.LOW: return chars_low;
            default: return "";
        }
    }

    public List<string> ToListString(string text)
    {
        var list = new List<string>();
        foreach (var t in text)
        {
            list.Add(t.ToString());
        }
        return list;
    }

}
