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
public class EngTrash3_GameController : GameController
{
    [Header("Prefab")]
    public GameObject wordDrop_prefab;
    public GameObject wordDrag_prefab;

    [Header("Obj ref")]
    public TextMeshProUGUI mainText;
    public RectTransform dropRect;
    public RectTransform dragRect;
    public Image mainImage;

    [Header("Setting")]
    public Vector2 dropOffset;

    [Header("Data")]

    public int roundIndex = -1;

    public EngTrash3_Datas engTrash3_Datas;
    public List<EngTrash3_Data> pools;

    public EngTrash3_Data currentQuestion;
    public EngTrash3_WordData currentWord;

    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();

    public int score = 0;

    public List<EngTrash3_WordDrag> drags = new();
    public List<EngTrash3_WordDrop> drops = new();

    bool isAnswering = false;
    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        pools = engTrash3_Datas.datas.ToList<EngTrash3_Data>();

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

        currentQuestion = pools[roundIndex];

        mainImage.sprite = spriteKeyValuePairs["trash_03_" + (roundIndex + 1).ToString("00")];

        mainImage.rectTransform.DOAnchorPosY(251, 0.3f).From(new Vector2(0, 1000));

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

            var dropScript = clone.GetComponent<EngTrash3_WordDrop>();
            dropScript.droppable.onDropped += OnCharDrop;
            dropScript.text = dropData.correctChar;
            dropScript.index = dropData.index;
            dropScript.charIndex = dropData.charIndex;
            drops.Add(dropScript);
        }

        var i = 0;
        foreach (var dragData in word.choices)
        {
            var clone = Instantiate(wordDrag_prefab, dragRect);
            var dragScript = clone.GetComponent<EngTrash3_WordDrag>();
            dragScript.text = dragData;
            dragScript.textMesh.text = dragData;
            drags.Add(dragScript);
            i++;
        }

        this.currentWord = word;

    }
    public EngTrash3_WordData CreateWordData(EngTrash3_Data raw)
    {
        var mainWord = raw.mainWord;
        var maskWord = raw.maskWord;
        var mainWord_split = raw.mainWord.Split(" ");
        var maskWord_split = raw.maskWord.Split(" ");

        var wordData = new EngTrash3_WordData(mainWord, maskWord);

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
                    var dropData = new EngTrash3_DropData(dropIndex, i, pos, correctChar);
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

        for (int i = 0; i < raw.choices.Split(" ").Length; i++)
        {
            var word = raw.choices.Split(" ")[i].ToString();
            if (word != "" && word != " ") wordData.choices.Add(word);
        }

        return wordData;
    }
    void OnCharDrop(Droppable droppable, Draggable draggable)
    {
        var dropScript = droppable.GetComponent<EngTrash3_WordDrop>();
        var dragScript = draggable.GetComponent<EngTrash3_WordDrag>();

        if (dropScript.isCorrect) return;

        if (dropScript.text != dragScript.text)
        {
            AudioManager.instance.PlaySound("ui_fail_1");
            return;
        }


        StringBuilder sb = new(mainText.text);
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

            DoDelayAction(0.5f, () =>
            {
                SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
                {
                    score++;
                    SetPhase(GAME_PHASE.ROUND_ANSWERING);
                });
            });

        }
        else
        {
            AudioManager.instance.PlaySound("ui_ding");
        }

    }
    void OnEnterRoundWaiting()
    {


    }
    void OnEnterRoundAnswering()
    {
        if (roundIndex >= engTrash3_Datas.datas.Length - 1)
        {
            FinishedGame(true, 0);
        }
        else
        {
            SetPhase(GAME_PHASE.ROUND_START);
        }
    }

    public void ForceToNextGame()
    {
        // to room hidden game
        GameManager.instance.SetTargetGame(SUBGAME_INDEX.ENG_TRASH4);
    }

    public void OnChoiceClick(EngTrash2_Choice choice)
    {
        if (isAnswering) return;
        isAnswering = true;
        if (choice.index == 0)
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
                isAnswering = false;
                SetPhase(GAME_PHASE.ROUND_WAITING);
            });
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

    public class EngTrash3_WordData
    {
        public string mainWord;
        public string maskWord;
        public List<string> choices = new List<string>();
        public List<EngTrash3_DropData> dropDatas = new List<EngTrash3_DropData>();

        public EngTrash3_WordData(string mainWord, string maskWord)
        {
            this.mainWord = mainWord;
            this.maskWord = maskWord;
        }

    }

    public class EngTrash3_DropData
    {
        public int index;
        public int charIndex;
        public Vector3 pos;
        public string correctChar;
        public EngTrash3_DropData(int index, int charIndex, Vector3 pos, string correctChar)
        {
            this.index = index;
            this.charIndex = charIndex;
            this.pos = pos;
            this.correctChar = correctChar;
        }
    }

}
