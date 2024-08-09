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

public class EngPlant3_GameController : GameController
{
    [Header("Prefab")]
    public GameObject wordDrop_prefab;
    public GameObject wordDrag_prefab;

    [Header("Obj ref")]

    public Button startButton;
    public RectTransform startButtonLabel;
    public RectTransform arrow;
    public CanvasGroup mainGameCanvas;

    public Image mainImage;
    public TextMeshProUGUI mainText;

    public RectTransform dropRect;
    public RectTransform dragRect1;
    public RectTransform dragRect2;

    public Vector2 dropOffset;

    [Header("Data")]

    public int roundIndex = 2;

    public EngPlant3_Datas engPlant3_Datas;
    public List<EngPlant3_Data> pools;

    public EngPlant3_Data currentQuestion;
    public EngPlant3_WordData currentWord;

    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    public int score = 0;

    public List<EngPlant3_WordDrop> drops = new();
    public List<EngPlant3_WordDrag> drags = new();
    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        pools = engPlant3_Datas.datas.ToList<EngPlant3_Data>();

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

        mainImage.sprite = spriteKeyValuePairs["03-" + (roundIndex + 1).ToString("00")];

        mainImage.rectTransform.DOAnchorPosX(0, 0.3f).From(new Vector2(-2000f, 90f));

        AudioManager.instance.PlaySound("ui_swipe");

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

        var charsize = 40;

        foreach (var dropData in word.dropDatas)
        {
            var clone = Instantiate(wordDrop_prefab, dropRect);
            var drop_rt = clone.GetComponent<RectTransform>();
            drop_rt.sizeDelta = new Vector2(3 * charsize, drop_rt.sizeDelta.y);
            drop_rt.anchoredPosition = dropData.pos + (Vector3)dropOffset;

            var dropScript = clone.GetComponent<EngPlant3_WordDrop>();
            dropScript.droppable.onDropped += OnCharDrop;
            dropScript.text = dropData.correctChar;
            dropScript.index = dropData.index;
            dropScript.charIndex = dropData.charIndex;
            drops.Add(dropScript);
        }

        var i = 0;
        foreach (var dragData in word.choices)
        {
            var dragRect = i <= 3 ? dragRect1 : dragRect2;
            var clone = Instantiate(wordDrag_prefab, dragRect);
            var dragScript = clone.GetComponent<EngPlant3_WordDrag>();
            dragScript.text = dragData;
            dragScript.textMesh.text = dragData;
            drags.Add(dragScript);
            i++;
        }

        this.currentWord = word;

    }
    public EngPlant3_WordData CreateWordData(EngPlant3_Data raw)
    {
        var mainWord = raw.mainWord;
        var maskWord = raw.maskWord;
        var mainWord_split = raw.mainWord.Split(" ");
        var maskWord_split = raw.maskWord.Split(" ");

        var wordData = new EngPlant3_WordData(mainWord, maskWord);

        mainText.text = maskWord;
        mainText.ForceMeshUpdate();

        var textInfo = mainText.textInfo;

        var wordIndex = 0;
        var dropIndex = 0;
        var flag_count = 0;
        for (int i = 0; i < maskWord.Length; i++)
        {
            var charString = maskWord[i].ToString();
            if (charString == "_")
            {
                if (flag_count == 0)
                {
                    var charInfo = textInfo.characterInfo[i];
                    var pos = charInfo.bottomLeft;
                    var correctChar = mainWord_split[wordIndex].ToString();
                    var dropData = new EngPlant3_DropData(dropIndex, i, pos, correctChar);
                    wordData.dropDatas.Add(dropData);
                    dropIndex++;
                }
                flag_count++;
            }
            else
            {
                if (charString == " ")
                {
                    wordIndex++;
                }
                flag_count = 0;
            }
        }

        // var wordIndex = 0;
        // var flag_count = 0;
        // for (int i = 0; i < maskWord.Length; i++)
        // {
        //     var charString = maskWord[i].ToString();
        //     if (charString == "_")
        //     {
        //         flag_count++;
        //         if (flag_count == 1)
        //         {
        //             var charInfo = textInfo.characterInfo[i];
        //             var pos = charInfo.bottomLeft;
        //             var correctChar = mainWord.Split(" ")[wordIndex].ToString();
        //             var dropData = new EngPlant3_DropData(i, pos, correctChar);
        //             wordData.dropDatas.Add(dropData);
        //         }
        //     }
        //     else
        //     {
        //         if (charString == " ")
        //         {
        //             wordIndex++;
        //         }
        //         flag_count = 0;
        //     }
        // }

        for (int i = 0; i < raw.choices.Length; i++)
        {
            wordData.choices.Add(raw.choices[i].ToString());
        }

        return wordData;
    }
    void OnCharDrop(Droppable droppable, Draggable draggable)
    {
        var dropScript = droppable.GetComponent<EngPlant3_WordDrop>();
        var dragScript = draggable.GetComponent<EngPlant3_WordDrag>();

        if (dropScript.isCorrect) return;

        Debug.Log(dropScript.text);
        Debug.Log(dragScript.text);

        if (dropScript.text != dragScript.text)
        {
            AudioManager.instance.PlaySound("ui_fail_1");
            return;
        }

        AudioManager.instance.PlaySound("ui_ding");

        StringBuilder sb = new(mainText.text);
        //sb[dropScript.index] = "".ToCharArray()[0];
        //sb[dropScript.index + 1] = "".ToCharArray()[0];
        //sb[dropScript.index + 2] = "".ToCharArray()[0];

        sb.Remove(dropScript.charIndex, 3);
        sb.Insert(dropScript.charIndex, dropScript.text);

        mainText.text = sb.ToString();
        dropScript.isCorrect = true;
        dropScript.canvasGroup.TotalHide();
        dragScript.canvasGroup.TotalHide();

        mainText.ForceMeshUpdate();
        var textInfo = mainText.textInfo;

        foreach (var drop in drops)
        {
            //var charInfo = textInfo.characterInfo[drop.index];
            //var pos = charInfo.bottomLeft + (Vector3)dropOffset;
            //drop.GetComponent<RectTransform>().anchoredPosition = pos;
        }

        foreach (var drop in drops)
        {
            if (drop.index >= dropScript.index) drop.index--;
            if (drop.index < 0) continue;

            var wordIndex = 0;
            var dropIndex = 0;
            var flag_count = 0;
            for (int i = 0; i < mainText.text.Length; i++)
            {
                var charString = mainText.text[i].ToString();
                if (charString == "_")
                {
                    if (flag_count == 0)
                    {
                        if (dropIndex == drop.index)
                        {
                            var charInfo = textInfo.characterInfo[i];
                            var pos = charInfo.bottomLeft + (Vector3)dropOffset;
                            drop.GetComponent<RectTransform>().anchoredPosition = pos;
                            drop.charIndex = i;
                            break;
                        }
                        dropIndex++;
                    }
                    flag_count++;
                }
                else
                {
                    if (charString == " ")
                    {
                        wordIndex++;
                    }
                    flag_count = 0;
                }
            }
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
    void OnEnterRoundWaiting()
    {


    }

    void OnEnterRoundAnswering()
    {
        mainImage.rectTransform.DOAnchorPosX(-2000, 0.3f).OnComplete(() =>
        {
            if (roundIndex >= engPlant3_Datas.datas.Length - 1)
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

    public class EngPlant3_WordData
    {

        public string mainWord;
        public string maskWord;
        public List<string> choices = new List<string>();
        public List<EngPlant3_DropData> dropDatas = new List<EngPlant3_DropData>();


        public EngPlant3_WordData(string mainWord, string maskWord)
        {
            this.mainWord = mainWord;
            this.maskWord = maskWord;
        }

    }

    public class EngPlant3_DropData
    {
        public int index;
        public int charIndex;
        public Vector3 pos;
        public string correctChar;
        public EngPlant3_DropData(int index, int charIndex, Vector3 pos, string correctChar)
        {
            this.index = index;
            this.charIndex = charIndex;
            this.pos = pos;
            this.correctChar = correctChar;
        }
    }
}
