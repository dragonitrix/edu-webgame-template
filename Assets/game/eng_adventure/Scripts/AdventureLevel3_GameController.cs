using System.Collections;
using System.Collections.Generic;
using TMPro;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.UI;

public class AdventureLevel3_GameController : GameController
{
    [Header("Object Ref")]
    public GridController mainGridController;
    public AudioSource questionAudioSource;
    public Button goNextButton;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI winText;

    [Header("Transition")]
    public TransitionProfile transitionProfile;
    public TransitionAnimator transitionAnimator;
    public RawImage transitionRenderer;
    public Image bgImage;

    [Header("Game Settings")]
    public TextMeshProUGUI titleText;
    public ADVENTURE_LEVEL level;

    [Header("Game Value")]
    public AudioClip[] phonicsClips;
    Dictionary<string,AudioClip> answerList = new Dictionary<string,AudioClip>();
    List<string> currentAnswer = new List<string>();
    int gameStage = 0;
    List<string> alphabets = new List<string>();

    //debuging purpose only
    protected override void Start()
    {
        base.Start();
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);
        goNextButton.onClick.RemoveAllListeners();
        goNextButton.onClick.AddListener(OnNextButtonClick);
        level = (ADVENTURE_LEVEL)gameLevel;
        mainGridController.InitGrid();
        var mainGridCells = mainGridController.cells;
        char[] alphabetsChar = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        moneyText.text = "Money: " + ScoreManager.Instance.currentScore + "฿";
        titleText.text = "Level:3";
        foreach (char c in alphabetsChar)
        {
            alphabets.Add(c.ToString());
        }

        currentAnswer = new List<string>(alphabets);
        currentAnswer.Shuffle();

        for (int i = 0; i < alphabets.Count; i++)
        {
            answerList.Add(alphabets[i], phonicsClips[i]);
        }

        //alphabets.Shuffle();

        for (int i = 0; i < mainGridCells.Count; i++)
        {
            mainGridCells[i].SetValue("", false);
            mainGridCells[i].SetEnableText(true);
            mainGridCells[i].onClicked += OnMainGridButtonClick;
        }
        SetQuestion();
    }

    void SetMainGridCellValue(int index, string value)
    {
        mainGridController.cells[index].SetValue(value, false);
    }

    public void OnMainGridButtonClick(CellController cell)
    {

        // check answer
        if (cell.value == currentAnswer[gameStage])
        {
            //Debug.Log("answer corrected");
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(true, OnAnswerEffectComplete);
            ScoreManager.Instance.UpdateCurrentScore(10);
            // increase score
        }
        else
        {
            //Debug.Log("answer incorrect");
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(false, OnAnswerEffectComplete);
            ScoreManager.Instance.UpdateCurrentScore(-10);
            // minus score
        }
        moneyText.text = "Money: " + ScoreManager.Instance.currentScore + "฿";
    }

    public void OnPlaySoundIconClick()
    {
        questionAudioSource.PlayOneShot(answerList[currentAnswer[gameStage]]);
    }

    public void SetQuestion()
    {
        //alphabets.Shuffle();
        for (int i = 0; i < alphabets.Count; i++)
        {
            SetMainGridCellValue(i, alphabets[i]);
        }
    }

    void OnAnswerEffectComplete()
    {
        gameStage++;
        //if (gameStage >= answersList.Count)
        //{
        //    gameState = GAME_STATE.ENDED;
        //}

        if (gameState == GAME_STATE.ENDED)
        {
            winText.text = "เก่งมาก\nคุณมีเงินสะสม " + ScoreManager.Instance.currentScore + "฿";
            FinishedGame(true, 0);
        }
        else
        {
            SetQuestion();
        }
    }

    public void OnNextButtonClick()
    {
        GameManager.instance.NextScene();
        ScoreManager.Instance.UpdateFinalScore();
    }
}
