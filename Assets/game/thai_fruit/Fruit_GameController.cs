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

public class Fruit_GameController : GameController
{
    [Header("Prefab")]
    public GameObject drop_prefab;
    public GameObject drag_prefab;


    [Header("Obj ref")]
    public TextMeshProUGUI mainText;
    public RectTransform dropRect;
    public RectTransform dragRect;
    public RectTransform matraRect;
    public Droppable matraDrop;

    public RectTransform fruitRect;
    public RectTransform basketRect;

    public TextMeshProUGUI numberText;

    [Header("Setting")]
    public Vector2 dropOffset;

    [Header("Data")]

    public int roundIndex = -1;

    public Fruit_Datas fruit_Datas;
    public List<Fruit_Data> pools;

    public Fruit_WordData currentWord;

    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    public List<Fruit_Drag> drags = new List<Fruit_Drag>();
    public List<Fruit_Drop> drops = new List<Fruit_Drop>();

    public int score = 0;

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        pools = fruit_Datas.datas.ToList<Fruit_Data>();
        pools.Shuffle();
        matraDrop.onDropped += OnMatraDrop;


        tutorialPopup.Enter();

        tutorialPopup.OnPopupExit += () =>
        {
            tutorialPopup.OnPopupExit = () => { };
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
        NewRound(roundIndex + 1);
    }

    void NewRound(int index)
    {
        Debug.Log("round start");

        roundIndex = index;

        numberText.text = (roundIndex + 1).ToString() + "/" + fruit_Datas.datas.Length;

        InitNewWord();

        matraRect.DOAnchorPosX(600, 0.2f);

        fruitRect.anchoredPosition = new Vector2(-730, 118);
        fruitRect.localScale = Vector2.zero;

    }
    public void InitNewWord()
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

        //var word = CreateWordData(fruit_Datas.datas[0]);

        var word = CreateWordData(pools[0]);
        pools.RemoveAt(0);

        foreach (var dropData in word.dropDatas)
        {
            var clone = Instantiate(drop_prefab, dropRect);
            clone.GetComponent<RectTransform>().anchoredPosition = dropData.pos;
            var dropScript = clone.GetComponent<Fruit_Drop>();
            dropScript.droppable.onDropped += OnCharDrop;
            dropScript.text = dropData.correctChar;
            dropScript.index = dropData.index;
            drops.Add(dropScript);
        }

        foreach (var dragData in word.choices)
        {
            var clone = Instantiate(drag_prefab, dragRect);
            var dragScript = clone.GetComponent<Fruit_Drag>();
            dragScript.text = dragData;
            dragScript.textMesh.text = dragData;
            drags.Add(dragScript);
        }

        this.currentWord = word;

    }

    public Fruit_WordData CreateWordData(Fruit_Data raw)
    {

        var mainWord = raw.mainWord;
        var maskWord = raw.maskWord;

        var wordData = new Fruit_WordData(raw.type, mainWord, maskWord);

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
                var dropData = new Fruit_DropData(i, pos, correctChar);
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


    void OnEnterRoundWaiting()
    {


    }

    void OnCharDrop(Droppable droppable, Draggable draggable)
    {
        var dropScript = droppable.GetComponent<Fruit_Drop>();
        var dragScript = draggable.GetComponent<Fruit_Drag>();

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

            matraRect.DOAnchorPosX(0, 0.2f);

        }

    }

    void OnMatraDrop(Droppable droppable, Draggable draggable)
    {
        var dragScript = draggable.GetComponent<Fruit_MatraDrag>();

        if (dragScript.type == currentWord.type)
        {
            SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
            {
                score++;
                TweenFruitDrop();
                //SetPhase(GAME_PHASE.ROUND_ANSWERING);
            });
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, () =>
            {
                SetPhase(GAME_PHASE.ROUND_ANSWERING);
            });
        }
    }

    void OnEnterRoundAnswering()
    {
        if (roundIndex >= fruit_Datas.datas.Length - 1)
        {
            FinishedGame(true, 0);
        }
        else
        {
            SetPhase(GAME_PHASE.ROUND_START);
        }
    }

    void TweenFruitDrop()
    {

        fruitRect.anchoredPosition = new Vector2(-730, 118);
        fruitRect.localScale = Vector2.one;

        fruitRect.DOAnchorPosY(-266, 0.3f).OnComplete(() =>
        {
            if (AudioManager.instance) AudioManager.instance.PlaySound("drop_pop");
        });
        fruitRect.DORotate(new Vector3(0, 0, 180), 1f);

        basketRect.DOScale(Vector2.one * 1.1f, 0.1f).SetDelay(0.3f).OnComplete(() =>
            {
                basketRect.DOScale(Vector2.one * 1f, 0.1f).OnComplete(() =>
                {
                    SetPhase(GAME_PHASE.ROUND_ANSWERING);
                });
            }
        );

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

    public class Fruit_WordData
    {
        public Fruit_Data_TYPE type;
        public string mainWord;
        public string maskWord;
        public List<string> choices = new List<string>();
        public List<Fruit_DropData> dropDatas = new List<Fruit_DropData>();


        public Fruit_WordData(Fruit_Data_TYPE type, string mainWord, string maskWord)
        {
            this.type = type;
            this.mainWord = mainWord;
            this.maskWord = maskWord;
        }

    }

    public class Fruit_DropData
    {
        public int index;
        public Vector3 pos;
        public string correctChar;
        public Fruit_DropData(int index, Vector3 pos, string correctChar)
        {
            this.index = index;
            this.pos = pos;
            this.correctChar = correctChar;
        }
    }

}
