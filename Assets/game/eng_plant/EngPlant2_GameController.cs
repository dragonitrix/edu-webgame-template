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

public class EngPlant2_GameController : GameController
{
    [Header("Prefab")]
    public GameObject wordDrop_prefab;
    public GameObject wordDrag_prefab;

    [Header("Obj ref")]

    public Button startButton;
    public RectTransform startButtonLabel;
    public RectTransform arrow;
    public CanvasGroup mainGameCanvas;

    public CanvasGroup batch1;
    public CanvasGroup batch2;

    public CanvasGroup wordGameRect;
    public Image wordImage;

    public TextMeshProUGUI mainText;
    public RectTransform dropRect;
    public RectTransform dragRect;
    public Vector2 dropOffset;

    public List<EngPlant2_Field> b1_fields;
    public List<EngPlant2_Field> b2_fields;

    [Header("Data")]

    public int roundIndex = -1;
    public EngPlant2_Datas engPlant2_Datas;
    public List<EngPlant2_Data> pools;
    public EngPlant2_Data currentQuestion;
    public EngPlant2_Field currentField;
    public EngPlant2_WordData currentWord;

    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    public int score = 0;

    public List<EngPlant2_WordDrop> drops = new();
    public List<EngPlant2_WordDrag> drags = new();

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        pools = engPlant2_Datas.datas.ToList<EngPlant2_Data>();
        mainGameCanvas.TotalHide();
        tutorialPopup.Enter();

        foreach (var field in b1_fields)
        {
            field.InitField();
        }

        foreach (var field in b2_fields)
        {
            field.InitField();
        }

        startButton.onClick.AddListener(() =>
        {
            arrow.DOScale(Vector3.zero, 0.2f);
            startButtonLabel.DOScale(Vector3.zero, 0.2f);
            startButton.interactable = false;

            mainGameCanvas.DOFade(1f, 0.5f);
            mainGameCanvas.interactable = true;
            mainGameCanvas.blocksRaycasts = true;

            batch1.TotalShow();
            batch2.TotalHide();
            wordGameRect.TotalHide();

            SetPhase(GAME_PHASE.ROUND_START);
        });

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
        Debug.Log("round start");
        roundIndex = index;

        currentQuestion = pools[roundIndex];

        wordGameRect.DOFade(1f, 0);
        wordGameRect.interactable = true;
        wordGameRect.blocksRaycasts = true;

        wordImage.sprite = spriteKeyValuePairs["02-" + (roundIndex + 1).ToString("00")];
        wordImage.SetNativeSize();

        InitNewWord(roundIndex);

        SetPhase(GAME_PHASE.ROUND_WAITING);
    }

    public void InitNewWord(int index)
    {
        //clear old data
        foreach (var drag in drags)
        {
            DestroyImmediate(drag.gameObject);
        }
        drags.Clear();

        foreach (var drop in drops)
        {
            DestroyImmediate(drop.gameObject);
        }
        drops.Clear();

        var word = CreateWordData(pools[index]);

        foreach (var dropData in word.dropDatas)
        {
            var clone = Instantiate(wordDrop_prefab, dropRect);
            clone.GetComponent<RectTransform>().anchoredPosition = dropData.pos;
            var dropScript = clone.GetComponent<EngPlant2_WordDrop>();
            dropScript.droppable.onDropped += OnCharDrop;
            dropScript.text = dropData.correctChar;
            dropScript.index = dropData.index;
            drops.Add(dropScript);
        }

        foreach (var dragData in word.choices)
        {
            var clone = Instantiate(wordDrag_prefab, dragRect);
            var dragScript = clone.GetComponent<EngPlant2_WordDrag>();
            dragScript.text = dragData;
            dragScript.textMesh.text = dragData;
            drags.Add(dragScript);
        }

        this.currentWord = word;

    }
    public EngPlant2_WordData CreateWordData(EngPlant2_Data raw)
    {
        var mainWord = raw.mainWord;
        var maskWord = raw.maskWord;

        var wordData = new EngPlant2_WordData(mainWord, maskWord);

        mainText.text = maskWord;
        mainText.ForceMeshUpdate();

        var textInfo = mainText.textInfo;
        for (int i = 0; i < maskWord.Length; i++)
        {
            var charString = maskWord[i].ToString();
            if (charString == "_")
            {
                var charInfo = textInfo.characterInfo[i];
                var pos = charInfo.bottomLeft + (Vector3)dropOffset;
                var correctChar = mainWord[i].ToString();
                var dropData = new EngPlant2_DropData(i, pos, correctChar);
                wordData.dropDatas.Add(dropData);
            }
        }

        var rawChoices = raw.choices.Replace(" ", "");
        for (int i = 0; i < rawChoices.Length; i++)
        {
            wordData.choices.Add(rawChoices[i].ToString());
        }

        return wordData;
    }

    void OnCharDrop(Droppable droppable, Draggable draggable)
    {
        var dropScript = droppable.GetComponent<EngPlant2_WordDrop>();
        var dragScript = draggable.GetComponent<EngPlant2_WordDrag>();

        if (dropScript.isCorrect) return;
        if (dropScript.text != dragScript.text)
        {
            AudioManager.instance.PlaySound("ui_fail_1");
            return;
        }

        AudioManager.instance.PlaySound("ui_ding");
        StringBuilder sb = new(mainText.text);
        sb[dropScript.index] = dropScript.text.ToCharArray()[0];
        mainText.text = sb.ToString();
        dropScript.isCorrect = true;
        dragScript.canvasGroup.TotalHide();

        mainText.ForceMeshUpdate();
        var textInfo = mainText.textInfo;

        foreach (var drop in drops)
        {
            var charInfo = textInfo.characterInfo[drop.index];
            var pos = charInfo.bottomLeft + (Vector3)dropOffset;
            drop.GetComponent<RectTransform>().anchoredPosition = pos;
        }

        //check chars correct
        var allCorrect = true;
        foreach (var drop in drops)
        {
            if (!drop.isCorrect)
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            foreach (var drag in drags)
            {
                drag.canvasGroup.interactable = false;
            }

            SetPhase(GAME_PHASE.ROUND_ANSWERING);

        }

    }

    public void OnFieldDrop(int index, EngPlant2_Field field)
    {
        currentField = field;
        NewRound(index);
    }


    void OnEnterRoundWaiting()
    {


    }

    void OnEnterRoundAnswering()
    {
        wordGameRect.interactable = false;
        wordGameRect.blocksRaycasts = false;
        wordGameRect.DOFade(0, 0.7f).OnComplete(() =>
        {

            currentField.SetCorrect();

            DoDelayAction(0.5f, () =>
            {
                if (roundIndex <= 3)
                {
                    var allCorrect = CheckBatchCorrect(b1_fields);
                    if (allCorrect)
                    {
                        batch1.TotalHide();
                        batch2.TotalHide();

                        batch1.DOFade(0, 0.5f).From(1f);
                        batch2.DOFade(1, 0.5f).From(0f).OnComplete(() =>
                        {
                            batch2.interactable = true;
                            batch2.blocksRaycasts = true;

                            SetPhase(GAME_PHASE.ROUND_START);
                        });
                    }
                }
                else
                {
                    var allCorrect = CheckBatchCorrect(b2_fields);
                    if (allCorrect)
                    {
                        FinishedGame(true, 0);
                    }
                }
            });
        });

    }

    public bool CheckBatchCorrect(List<EngPlant2_Field> batch)
    {
        var allCorrect = true;

        for (int i = 0; i < batch.Count; i++)
        {
            if (!batch[i].isCorrect)
            {
                allCorrect = false;
                break;
            }
        }

        return allCorrect;
    }

    public void ForceToNextGame()
    {
        // to room hidden game
        GameManager.instance.SetTargetGame(SUBGAME_INDEX.ENG_PLANT_3);
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

    public class EngPlant2_WordData
    {

        public string mainWord;
        public string maskWord;
        public List<string> choices = new List<string>();
        public List<EngPlant2_DropData> dropDatas = new List<EngPlant2_DropData>();


        public EngPlant2_WordData(string mainWord, string maskWord)
        {
            this.mainWord = mainWord;
            this.maskWord = maskWord;
        }

    }

    public class EngPlant2_DropData
    {
        public int index;
        public Vector3 pos;
        public string correctChar;
        public EngPlant2_DropData(int index, Vector3 pos, string correctChar)
        {
            this.index = index;
            this.pos = pos;
            this.correctChar = correctChar;
        }
    }

}
