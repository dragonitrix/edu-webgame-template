using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;

public class Wannayuuk_GameController : GameController
{
    [Header("Prefab")]
    public RectTransform cell_prefab;

    [Header("Obj ref")]
    public RectTransform cellRect;
    public RectTransform title;

    public Image bg;
    public RectTransform bg_scene;

    [Header("Data")]
    public Wannayuuk_LevelData[] levelDatas;
    Wannayuuk_LevelData currentLevelData;
    Wannayuuk_RoundData currentRoundData;
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();
    List<Wannayuuk_Cell> cells = new();
    WANNAYUUK_TYPE targetAnswer;

    public Theme[] themes;
    int currentTheme = -1;

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
        NewRound();
    }

    void NewRound()
    {
        roundIndex++;
        var roundData = currentLevelData.roundDatas[roundIndex];
        currentRoundData = roundData;

        foreach (var cell in cells)
        {
            cell.Kill(true);
        }
        cells.Clear();

        WANNAYUUK_TYPE targetType = currentRoundData.target;
        WANNAYUUK_TYPE nontargetType = WANNAYUUK_TYPE.TOH;

        List<Sprite> targetPool = new();
        List<Sprite> nontargetPool = new();

        int targetCount = 0;
        int nontargetCount = 0;

        switch (targetType)
        {
            case WANNAYUUK_TYPE.TOH:
                targetPool = currentRoundData.TOHs;
                targetCount = currentRoundData.TOH_Count;

                nontargetType = WANNAYUUK_TYPE.TRI;
                nontargetPool = currentRoundData.TRIs;
                nontargetCount = currentRoundData.TRI_Count;
                break;
            case WANNAYUUK_TYPE.TRI:
                targetPool = currentRoundData.TRIs;
                targetCount = currentRoundData.TRI_Count;

                nontargetType = WANNAYUUK_TYPE.TOH;
                nontargetPool = currentRoundData.TOHs;
                nontargetCount = currentRoundData.TOH_Count;
                break;
        }

        targetAnswer = targetType;

        if (targetCount == -1)
            targetCount = targetPool.Count;
        if (nontargetCount == -1)
            nontargetCount = nontargetPool.Count;

        InitCells(targetType, targetCount, targetPool);
        InitCells(nontargetType, nontargetCount, nontargetPool);

        cells.Shuffle();

        foreach (var cell in cells)
        {
            cell.transform.parent.SetAsLastSibling();
        }


        title.DOScale(Vector2.zero, 0.1f).OnComplete(() =>
        {
            title.GetComponent<Image>().sprite = spriteKeyValuePairs[targetAnswer.ToString()];
            title.DOScale(Vector2.one, 0.1f);
        });

        SetTheme(currentRoundData.themeIndex);

        AudioManager.instance.PlaySpacialSound(targetAnswer.ToString(), () =>
        {
            for (int i = 0; i < cells.Count; i++)
            {
                cells[i].SetOffset(i % 2 == 0 ? 200 : -200);
                cells[i].Show(i * 0.3f);
            }
            SetPhase(GAME_PHASE.ROUND_WAITING);
        });

    }

    void SetTheme(int index)
    {
        if (currentTheme == index) return;
        currentTheme = index;

        var theme = themes[index];

        bg.DOColor(theme.color, 0.2f);

        bg_scene.DOAnchorPos(new Vector2(0, -2000), 0.1f).OnComplete(() =>
        {
            bg_scene.GetComponent<Image>().sprite = theme.sprite;
            bg_scene.DOAnchorPos(Vector2.zero, 0.1f);
        });

    }

    void InitCells(WANNAYUUK_TYPE type, int count, List<Sprite> pool)
    {
        for (int i = 0; i < count; i++)
        {
            Sprite targetSprite;
            if (i < pool.Count)
            {
                targetSprite = pool[i];
            }
            else
            {
                targetSprite = pool[Random.Range(0, pool.Count)];
            }

            var targetClone = Instantiate(cell_prefab, cellRect);
            var cellScript = targetClone.GetComponentInChildren<Wannayuuk_Cell>();
            cellScript.parent = this;
            cellScript.Initcell(type, targetSprite);
            cells.Add(cellScript);
        }
    }

    void OnEnterRoundWaiting()
    {

    }

    void OnEnterRoundAnswering()
    {
        if (isCorrectAnswer)
        {
            // API_END_GAME
            if (roundIndex + 1 >= currentLevelData.roundDatas.Length)
            {
                FinishedGame(true, 0);
            }
            else
            {

                AudioManager.instance.StopSound("ui_ding");
                SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
                {
                    SetPhase(GAME_PHASE.ROUND_START);
                });
            }
        }
    }

    void OnRewardSoundFinished()
    {
    }

    void TweenGameElementIn(bool val)
    {
        if (val)
        {
        }
        else
        {
        }
    }

    public void OnCellClick(Wannayuuk_Cell cell)
    {
        if (cell.type == targetAnswer)
        {
            cell.SetCorrect();
            // AudioManager.instance.StopSound("ui_ding");
            AudioManager.instance.PlaySound("ui_ding");
            CheckCorrect();
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, () => { });
        }
    }

    void CheckCorrect()
    {
        var result = true;
        foreach (var cell in cells)
        {
            if (cell.type == targetAnswer)
                if (!cell.isCorrected) result = false;
        }

        if (result)
        {
            isCorrectAnswer = true;
            SetPhase(GAME_PHASE.ROUND_ANSWERING);
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

    [System.Serializable]
    public class Theme
    {
        public Color color;
        public Sprite sprite;
    }

}
