using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using TransitionsPlus;
using DG.Tweening;
using UnityEngine.Events;

public class Yummy_GameController : GameController
{
    [Header("Prefab")]
    public GameObject piece_prefab;
    public GameObject choice_prefab;

    [Header("Obj ref")]
    public VerticalLayoutGroup pieceLayout;
    public RectTransform gameRect;
    public RectTransform pieceGroup;
    public RectTransform choiceGroup;
    public RectTransform plateRect;
    public RectTransform plateHighlight;

    public RectTransform sandwichRect;
    public Image sandwichImage;

    [Header("Data")]
    public Yummy_PieceData breadData;
    public Yummy_LevelData levelData;
    int roundIndex = -1;
    Yummy_RoundData currentRoundData;
    Yummy_Piece currentPiece;
    string correctAnswer = "";
    public List<Sprite> levelSprites;
    public Dictionary<string, Sprite> spriteKeyValuePairs = new Dictionary<string, Sprite>();
    public List<Yummy_Sandwich> sandwiches = new();
    public List<Yummy_Piece> breads = new();
    public List<Yummy_Piece> pieces = new();
    public List<Yummy_Choice> choices = new();

    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {

        base.InitGame(gameLevel, playerCount);
        spriteKeyValuePairs = levelSprites.ToDictionary(x => x.name, x => x);

        for (int i = 0; i < sandwiches.Count; i++)
        {
            sandwiches[i].SetSandwich(i, this);
        }

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
        pieceLayout.spacing = 10;

    }

    void NewRound(int index)
    {
        roundIndex = index;
        foreach (var p in pieces.Concat(breads))
        {
            DestroyImmediate(p.gameObject);
        }
        pieces.Clear();

        currentRoundData = levelData.rounds[index];

        var bread1 = CreatePiece(breadData);
        bread1.index = 1;
        breads.Add(bread1);

        for (int i = currentRoundData.pieceDatas.Length - 1; i >= 0; i--)
        {
            var pieceData = currentRoundData.pieceDatas[i];
            var piece = CreatePiece(pieceData);
            piece.index = i + 2;
            pieces.Add(piece);
        }

        var bread2 = CreatePiece(breadData);
        bread2.index = 1;
        breads.Add(bread2);

        SetPhase(GAME_PHASE.ROUND_WAITING);

        gameRect.DOAnchorPos(Vector2.zero, 0.5f);

        AudioManager.instance.PlaySound("ui_swipe");

    }

    Yummy_Piece CreatePiece(Yummy_PieceData pieceData)
    {
        var clone = Instantiate(piece_prefab, pieceGroup);
        var piece = clone.GetComponent<Yummy_Piece>();
        piece.SetPieceData(pieceData, this);
        return piece;
    }

    void OnEnterRoundWaiting()
    {

    }

    void OnEnterRoundAnswering()
    {
        var allCorrect = CheckCorrect();

        if (!allCorrect)
        {
            foreach (var piece in pieces.Concat(breads))
            {
                if (!piece.isAnswered) piece.SetEnable(true);
            }
            SetPhase(GAME_PHASE.ROUND_WAITING);
        }
        else
        {
            // gameRect.DOAnchorPos(new Vector2(0, -1080f), 0.5f);
            // SetPhase(GAME_PHASE.ROUND_START);

            plateRect.DOAnchorPos(Vector2.zero, 0.5f);
            plateHighlight.DOScale(Vector3.one, 0.2f).SetDelay(0.2f);
        }
    }

    void CreatChoice(Yummy_PieceData pieceData)
    {
        var pool = new List<string>();
        pool.Add(pieceData.correctWord);
        foreach (var p in pieceData.wrongWords)
        {
            pool.Add(p);
        }

        pool.Shuffle();

        foreach (var p in pool)
        {
            var clone = Instantiate(choice_prefab, choiceGroup);
            var script = clone.GetComponent<Yummy_Choice>();
            script.SetChoice(p, this);
            choices.Add(script);
        }
        correctAnswer = pieceData.correctWord;
    }
    public void OnSandwichClick(Yummy_Sandwich sandwich)
    {
        AudioManager.instance.PlaySound("ui_pop");
        NewRound(sandwich.index);
    }

    public void OnPieceClick(Yummy_Piece piece)
    {
        currentPiece = piece;
        var isBread = piece.pieceData.correctWord == "bread";
        if (isBread)
        {
            foreach (var p in breads)
            {
                p.Fade2Color();
            }
        }
        else
        {
            piece.Fade2Color();
        }

        foreach (var p in pieces.Concat(breads))
        {
            p.SetEnable(false);
        }

        // spawn choice
        CreatChoice(piece.pieceData);

        AudioManager.instance.PlaySound("ui_pop");

    }

    public void OnChoiceClick(Yummy_Choice choice)
    {
        foreach (var c in choices)
        {
            c.SetEnable(false);
        }
        if (choice.text == correctAnswer)
        {
            SimpleEffectController.instance.SpawnAnswerEffect(true, AnswerCorrect);
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, () =>
            {
                foreach (var c in choices)
                {
                    c.SetEnable(true);
                }
            });
        }
    }

    public void OnPlateClick()
    {
        AudioManager.instance.PlaySound("ui_pop");
        plateHighlight.DOScale(Vector3.zero, 0.2f);
        plateRect.DOAnchorPos(new Vector2(700, 0), 0.5f);

        foreach (var item in pieces.Concat(breads))
        {
            item.HideTextRect();
        }

        sandwichImage.sprite = spriteKeyValuePairs["yum_sandwich_" + (roundIndex + 1).ToString("00")];
        DOTween.To(() => pieceLayout.spacing, x => pieceLayout.spacing = x, -90, 1f).OnComplete(() =>
        {
            sandwichRect.DOAnchorPos(Vector2.zero, 0.5f).OnComplete(() =>
            {
                AudioManager.instance.PlaySound("ui_win_2");
                DoDelayAction(2f, () =>
                {
                    sandwichRect.DOAnchorPos(new Vector2(0, -1080f), 0.5f);
                    gameRect.DOAnchorPos(new Vector2(0, -1080f), 0.5f);
                    sandwiches[roundIndex].SetEnable(false);

                    // API_END_GAME
                    var totalCorrect = CheckTotalCorrect();
                    if (totalCorrect)
                    {
                        FinishedGame(true, 0);
                    }
                    else
                    {
                        SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
                        {
                            SetPhase(GAME_PHASE.ROUND_START);
                        });
                    }
                });
            });
        });
    }

    public bool CheckCorrect()
    {
        var result = true;
        foreach (var piece in pieces.Concat(breads))
        {
            if (!piece.isAnswered) result = false;
        }

        return result;
    }
    public bool CheckTotalCorrect()
    {
        var result = true;
        foreach (var sandwich in sandwiches)
        {
            if (!sandwich.isCorrected) result = false;
        }

        return result;
    }


    void AnswerCorrect()
    {
        StartCoroutine(_AnswerCorrect());
    }

    IEnumerator _AnswerCorrect()
    {
        if (currentPiece.pieceData.correctWord == "bread")
        {
            foreach (var item in breads)
            {
                item.isAnswered = true;
                item.ShowTextRect();
            }
        }
        else
        {
            currentPiece.isAnswered = true;
            currentPiece.ShowTextRect();
        }
        for (int i = 0; i < choices.Count; i++)
        {
            choices[i].Kill(i * 0.1f);
            Destroy(choices[i].gameObject, 1 + i * 0.1f);
        }
        choices.Clear();

        var soundID = "yum_sand_" + (roundIndex + 1).ToString("00") + "_" + currentPiece.index.ToString("00");
        var soundLength = AudioManager.instance.GetClipLength(soundID);
        AudioManager.instance.PlaySpacialSound(soundID);

        yield return new WaitForSeconds(soundLength + 1);
        AudioManager.instance.PlaySpacialSound(soundID);
        yield return new WaitForSeconds(soundLength + 1);

        SetPhase(GAME_PHASE.ROUND_ANSWERING);
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

}
