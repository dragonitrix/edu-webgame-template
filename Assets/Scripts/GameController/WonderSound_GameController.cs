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

public class WonderSound_GameController : GameController
{

    [Header("Prefabs")]
    public GameObject cell_prefab;

    [Header("intro")]
    public SimpleBatchTweenController[] intros;
    SimpleBatchTweenController intro;
    WonderSound_LevelSettings levelSettings;

    [Header("Obj ref")]
    public Image mainImage;
    public RectTransform mainImageRect;
    public TextMeshProUGUI hintText;
    public RectTransform hintTextRect;
    public RectTransform cellRect;
    public Image gameBGImage;
    public RectTransform gameBGRect;
    public Droppable droppArea;
    public CellController dropCell;
    public RectTransform dropAreaRect;
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI finishText;

    [Header("Data")]
    public WonderSound_LevelData[] levelDatas;
    public List<Sprite> levelSprites;
    public Color[] level_colors;
    Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();
    List<CellController> cells;

    WonderSound_LevelData currentLevelData;

    WonderSound_RoundData currentRoundData;

    bool firstTutorial = true;
    bool isCorrectAnswer = false;
    int currentScore = 0;
    int maxScore = 0;

    int roundIndex = -1;
    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(2, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);

        int maxLevel = System.Enum.GetValues(typeof(WONDERSOUND_LEVEL)).Length;
        

        Button nextLevelButton = ((ResultPopupController)resultPopup).retryButton;
        nextLevelButton.onClick.RemoveAllListeners();
        nextLevelButton.onClick.AddListener(ToNextLevelButtonEvent);

        if (gameLevel >= maxLevel - 1)
        {
            nextLevelButton.gameObject.SetActive(false);
            Button homeButton = ((ResultPopupController)resultPopup).homeButton;
            homeButton.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, homeButton.gameObject.GetComponent<RectTransform>().anchoredPosition.y);
        }

        var level = (WONDERSOUND_LEVEL)gameLevel;
        levelSettings = new WonderSound_LevelSettings(level);
        currentScore = 0;
        switch (level)
        {
            case WONDERSOUND_LEVEL._1:
                intro = intros[0];
                currentLevelData = levelDatas[0];
                break;
            case WONDERSOUND_LEVEL._2:
                intro = intros[1];
                currentLevelData = levelDatas[1];
                break;
            case WONDERSOUND_LEVEL._3:
                intro = intros[2];
                currentLevelData = levelDatas[2];
                break;
        }
        maxScore = currentLevelData.rounds.Length;
        tutorialPopup.OnPopupExit += OnTutPopupExit;
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);
        gameBGImage.color = level_colors[gameLevel];

        droppArea.onDropped += OnDrop;

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
            AudioManager.instance.PlaySpacialSound("wds_tut", OnTutSoundFinished);
        });
    }

    void OnTutPopupExit()
    {
        if (!firstTutorial) return;
        firstTutorial = false;
        AudioManager.instance.StopSound("wds_tut", AudioManager.Channel.SPECIAL);
        SetPhase(GAME_PHASE.INTRO);
    }

    void OnTutSoundFinished()
    {
        tutorialPopup.closeButton.gameObject.SetActive(true);
    }

    void OnIntroSoundFinished()
    {
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
        AudioManager.instance.PlaySpacialSound(levelSettings.intro_soundid, OnIntroSoundFinished);
        intro.Enter();
        SetDisplayRoundElement(false);

    }

    void OnEnterRoundStart()
    {
        NewRound();
    }

    void OnEnterRoundWaiting()
    {
        foreach (var cell in cells)
        {
            cell.GetComponent<Draggable>().EnableSelf(true);
        }
        dropAreaRect.DOScale(Vector3.one, 0.3f);
        DragManager.instance.GetAllDragable();

    }

    void OnEnterRoundAnswering()
    {
        //transition out;
        mainImageRect.DOScale(Vector3.zero, 0.25f);
        hintTextRect.DOScale(Vector3.zero, 0.25f);
        gameBGRect.DOAnchorPos(new Vector2(0, -600), 0.25f);
        dropAreaRect.DOScale(Vector3.zero, 0.25f);
        answerText.GetComponent<CanvasGroup>().DOFade(0f, 0.25f);
        dropCell.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);

        if (isCorrectAnswer)
        { 
            currentScore++; 
            isCorrectAnswer = false;
        }

        if (roundIndex + 1 >= currentLevelData.rounds.Length)
        {
            // finished game
            finishText.text = "คะแนนรวม : " + currentScore + "/" + maxScore;
            FinishedGame(true, currentScore);
        }
        else
        {
            DoDelayAction(1f, () => { SetPhase(GAME_PHASE.ROUND_START); });
        }

    }


    void NewRound()
    {
        roundIndex++;
        var roundData = currentLevelData.rounds[roundIndex];
        currentRoundData = roundData;

        string prefix = "wds";
        string _levelIndex = "_" + (gameLevel + 1).ToString("00");
        string _roundIndex = "_" + (roundIndex + 1).ToString("00");
        // string hint_soundID = prefix + "_hint" + _levelIndex + _roundIndex;
        // string correct_soundID = prefix + _levelIndex + _roundIndex + "_01";
        // List<string> wrong_soundIDs = new List<string>();
        // for (int i = 0; i < roundData.wrong_answer.Length; i++)
        // {
        //     wrong_soundIDs.Add(prefix + _levelIndex + _roundIndex + "_" + (i + 2).ToString("00"));
        // }

        string imgID = prefix + _levelIndex + _roundIndex;
        Sprite imgSprite = spriteKeyValuePairs[imgID];

        //Debug.Log("imgID: " + imgID);
        //Debug.Log("hint_soundID: " + hint_soundID);
        //Debug.Log("correct_soundID: " + correct_soundID);
        //foreach (var item in wrong_soundIDs)
        //{
        //    Debug.Log("wrong_soundID: " + item);
        //}
        //Debug.Log("imgSprite: " + imgSprite);

        //setting
        mainImage.sprite = imgSprite;
        var _hintText = "คำไบ้: " + roundData.hint.Replace("(x)", " ... ");
        hintText.text = ThaiFontAdjuster.Adjust(_hintText);

        //clear previous cell
        for (int i = cellRect.childCount - 1; i >= 0; i--)
        {
            // Destroy the child GameObject immediately
            DestroyImmediate(cellRect.GetChild(i).gameObject);
        }

        cells = new List<CellController>();

        var correct_clone = Instantiate(cell_prefab, cellRect);
        var correct_cell = correct_clone.GetComponent<CellController>();
        correct_cell.SetText(roundData.correct_answer.text);
        correct_cell.index = 0;
        correct_cell.disableButton = true;
        cells.Add(correct_cell);

        for (int i = 0; i < roundData.wrong_answer.Length; i++)
        {
            var data = roundData.wrong_answer[i];
            var wrong_clone = Instantiate(cell_prefab, cellRect);
            var wrong_cell = wrong_clone.GetComponent<CellController>();
            wrong_cell.SetText(data.text);
            wrong_cell.index = i + 1;
            correct_cell.disableButton = true;
            cells.Add(wrong_cell);
        }

        cells.Shuffle();

        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].transform.SetAsLastSibling();
        }

        foreach (var cell in cells)
        {
            cell.GetComponent<Draggable>().onDragged += OnBeginCellDrag;
            cell.GetComponent<Draggable>().onEndDragged += OnEndCellDrag;
            cell.GetComponent<Draggable>().EnableSelf(false);
            cell.GetComponent<RectTransform>().localScale = Vector3.zero;
        }

        //transition;
        mainImageRect.DOScale(Vector3.one, 0.25f).From(Vector3.zero);
        hintTextRect.DOScale(Vector3.one, 0.25f).From(Vector3.zero);
        gameBGRect.DOAnchorPos(Vector2.zero, 0.25f).From(new Vector2(0, -600));
        dropAreaRect.DOScale(Vector3.zero, 0.25f);

        AudioManager.instance.PlaySpacialSound(GetSoundID(SOUNDID_TYPE.HINT), OnHintSoundFinished);
    }

    void SetDisplayRoundElement(bool val)
    {
        if (val)
        {
            mainImageRect.localScale = Vector3.one;
            hintTextRect.localScale = Vector3.one;
            dropAreaRect.localScale = Vector3.one;
            gameBGRect.anchoredPosition = new Vector2(0, 0);
            answerText.GetComponent<CanvasGroup>().alpha = 0;
        }
        else
        {
            mainImageRect.localScale = Vector3.zero;
            hintTextRect.localScale = Vector3.zero;
            dropAreaRect.localScale = Vector3.zero;
            gameBGRect.anchoredPosition = new Vector2(0, -600);
        }
    }

    void OnHintSoundFinished()
    {
        StartCoroutine(ShowCellCoroutine());
    }

    IEnumerator ShowCellCoroutine()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            cell.GetComponent<RectTransform>().DOScale(Vector3.one, 0.3f);
            var word_soundID = GetSoundID(SOUNDID_TYPE.WORD, cell.index + 1);
            AudioManager.instance.PlaySpacialSound(word_soundID);
            yield return new WaitForSeconds(2);
        }
        SetPhase(GAME_PHASE.ROUND_WAITING);
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

    void OnBeginCellDrag(Draggable obj)
    {
        obj.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);
    }
    void OnEndCellDrag(Draggable obj)
    {
        obj.GetComponent<CanvasGroup>().DOFade(1f, 0.2f);
    }

    void OnDrop(Droppable droppable, Draggable draggable)
    {
        var cell = draggable.GetComponent<CellController>();
        var index = cell.index;

        dropCell.SetText(cell.text.text);
        dropCell.GetComponent<CanvasGroup>().DOFade(1f, 0.2f);


        foreach (var _cell in cells)
        {
            _cell.GetComponent<CanvasGroup>().DOFade(0f, 1f).From(1f);
        }

        var _answerText = "คำตอบ: " + currentRoundData.hint.Replace("(x)", currentRoundData.correct_answer.text);
        answerText.text = ThaiFontAdjuster.Adjust(_answerText);
        answerText.GetComponent<CanvasGroup>().DOFade(1f, 0.3f).SetDelay(1f);
        if(index == 0) isCorrectAnswer = true;
        AudioManager.instance.PlaySpacialSound(GetSoundID(SOUNDID_TYPE.ANSWER, index + 1), OnAnswerSoundFinished);
    }

    void OnAnswerSoundFinished()
    {
        SetPhase(GAME_PHASE.ROUND_ANSWERING);
    }

    string GetSoundID(SOUNDID_TYPE type, int index = 0)
    {
        var prefix = "wds_";
        string _levelIndex = "_" + (gameLevel + 1).ToString("00");
        string _roundIndex = "_" + (roundIndex + 1).ToString("00");

        switch (type)
        {
            case SOUNDID_TYPE.HINT:
                prefix += "hint";
                return prefix + _levelIndex + _roundIndex;
            case SOUNDID_TYPE.WORD:
                prefix += "word";
                return prefix + _levelIndex + _roundIndex + "_" + index.ToString("00");
            case SOUNDID_TYPE.ANSWER:
                prefix += "answer";
                return prefix + _levelIndex + _roundIndex + "_" + index.ToString("00");
        }

        return null;
    }

    public enum SOUNDID_TYPE
    {
        HINT,
        WORD,
        ANSWER
    }

}
