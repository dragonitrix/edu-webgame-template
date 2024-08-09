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

public class EngPlant1_GameController : GameController
{
    [Header("Prefab")]
    public GameObject choice;

    [Header("Obj ref")]

    public Button startButton;
    public RectTransform startButtonLabel;
    public RectTransform arrow;
    public CanvasGroup mainGameCanvas;

    public Image mainImage;
    public RectTransform choicesRect;
    public TextMeshProUGUI numberText;

    [Header("Data")]

    public int roundIndex = -1;

    public EngPlant1_Datas engPlant1_Datas;
    public List<EngPlant1_Data> pools;

    public EngPlant1_Data currentQuestion;

    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    public int score = 0;

    public List<EngPlant1_Choice> choices = new List<EngPlant1_Choice>();

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        pools = engPlant1_Datas.datas.ToList<EngPlant1_Data>();

        tutorialPopup.Enter();

        startButton.onClick.AddListener(() =>
        {
            arrow.DOScale(Vector3.zero, 0.2f);
            startButtonLabel.DOScale(Vector3.zero, 0.2f);
            startButton.interactable = false;

            mainGameCanvas.DOFade(1f, 0.5f);
            mainGameCanvas.interactable = true;
            mainGameCanvas.blocksRaycasts = true;

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
        NewRound(roundIndex + 1);
    }

    void NewRound(int index)
    {
        Debug.Log("round start");

        roundIndex = index;

        currentQuestion = pools[roundIndex];

        mainImage.sprite = spriteKeyValuePairs["01-" + (roundIndex + 1).ToString("00")];

        mainImage.rectTransform.DOAnchorPosX(0, 0.3f).From(new Vector2(2000f, 90f));

        AudioManager.instance.PlaySound("ui_swipe");
        //clear old data
        foreach (var choice in choices)
        {
            DestroyImmediate(choice.gameObject);
        }
        choices.Clear();

        // init choices
        for (int i = 0; i < currentQuestion.texts.Length; i++)
        {
            var clone = Instantiate(choice, choicesRect);
            var script = clone.GetComponent<EngPlant1_Choice>();
            script.SetData(this, i, currentQuestion.texts[i]);
            choices.Add(script);
        }

        choices.Shuffle();
        foreach (var choice in choices)
        {
            choice.rectTransform.SetAsLastSibling();
        }

        numberText.text = (roundIndex + 1).ToString() + "/" + engPlant1_Datas.datas.Length;

        SetPhase(GAME_PHASE.ROUND_WAITING);
    }

    public void OnChoiceClick(EngPlant1_Choice choice)
    {
        if (gamePhase != GAME_PHASE.ROUND_WAITING) return;

        if (choice.text == currentQuestion.texts[0])
        {
            SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
            {
                score++;
                SetPhase(GAME_PHASE.ROUND_ANSWERING);
            });
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, () =>
            {
                //SetPhase(GAME_PHASE.ROUND_ANSWERING);
            });
        }
    }

    void OnEnterRoundWaiting()
    {


    }

    void OnEnterRoundAnswering()
    {
        mainImage.rectTransform.DOAnchorPosX(-2000, 0.3f).OnComplete(() =>
        {
            if (roundIndex >= engPlant1_Datas.datas.Length - 1)
            {
                FinishedGame(true, 0);
            }
            else
            {
                SetPhase(GAME_PHASE.ROUND_START);
            }
        });

    }

    public void ForceToNextGame()
    {
        // to room hidden game
        GameManager.instance.SetTargetGame(SUBGAME_INDEX.ENG_PLANT_2);
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
