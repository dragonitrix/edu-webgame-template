using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TransitionsPlus;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BingoMultiLevel1_GameController : GameController
{
    [Header("Object Ref")]
    public Transform[] mainGridPositionHolder;
    public TMP_InputField equationXText;
    public TMP_InputField equationYText;
    public TMP_InputField equationInputField;
    public Button informationButton;
    public Image equationImage;

    [Header("Game Settings")]
    public TextMeshProUGUI titleText;
    public Bingo_LevelSettings levelSettings;

    [Header("Game Value")]
    //[HideInInspector]
    public string correctAnswer = "";
    public BingoMultiLevelOneScriptable questions;
    List<BingoMultiLevelOneQuestion> bingoQuestions = new List<BingoMultiLevelOneQuestion>();
    int questionIndex;
    Vector2 currentEquation = Vector2.zero;
    int currentQuestionCorrect = 0;
    int consequtiveCorrect = 0;

    //debuging purpose only
    protected override void Start()
    {
        if (GameManager.instance == null) InitGame(0, PLAYER_COUNT._1_PLAYER);
        base.Start();
    }

    public override void InitGame(int gameLevel, PLAYER_COUNT playerCount)
    {
        base.InitGame(gameLevel, playerCount);

        titleText.text = "Multiplication: Level 1";
        BingoMultiLevelOneScriptable currentLevelQuestion = questions;
        bingoQuestions = new List<BingoMultiLevelOneQuestion>();

        foreach (var item in currentLevelQuestion.questions)
        {
            bingoQuestions.Add(item);
        }

        informationButton.onClick.AddListener(() =>
        {
            tutorialPopup.Enter();
        }
        );
        GetCurrentAnswer();
    }

    public void OnNextButtonClick()
    {
        GameManager.instance.NextScene();
    }

    public void OnCheckPlusButtonClick()
    {
        // check answer
        if (equationInputField.text == correctAnswer.ToString())
        {
            //Debug.Log("answer corrected");
            currentQuestionCorrect++;
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(true, OnAnswerEffectComplete);
        }
        else
        {
            //Debug.Log("answer incorrect");
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(false, OnAnswerEffectComplete);
        }
        //SetPhase(GAME_PHASE.ANSWER_2_SELECTNUMBER);
    }
    public void OnCheckMultiButtonClick()
    {
        // check answer
        if (equationXText.text == currentEquation.x.ToString() && equationYText.text == currentEquation.y.ToString())
        {
            //Debug.Log("answer corrected");
            currentQuestionCorrect++;
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(true, OnAnswerEffectComplete);
        }
        else
        {
            //Debug.Log("answer incorrect");
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(false, OnAnswerEffectComplete);
        }
        //SetPhase(GAME_PHASE.ANSWER_2_SELECTNUMBER);
    }

    void GetCurrentAnswer()
    {
        equationInputField.text = "";
        equationXText.text = "";
        equationYText.text = "";
        questionIndex = Random.Range(0, bingoQuestions.Count);
        string chosenAnswer = bingoQuestions[questionIndex].answer;
        correctAnswer = chosenAnswer;

        Vector2 equation = bingoQuestions[questionIndex].equations;
        currentEquation = equation;

        equationImage.sprite = bingoQuestions[questionIndex].equationSprite;
        equationImage.SetNativeSize();
    }

    void OnAnswerEffectComplete()
    {
        if (currentQuestionCorrect >= 2)
        {
            bingoQuestions.Remove(bingoQuestions[questionIndex]);
            currentQuestionCorrect = 0;
            consequtiveCorrect++;
            CheckWinCondition();
            if (gameState != GAME_STATE.ENDED)
            {
                GetCurrentAnswer();
            }
        }
    }

    public override void CheckWinCondition()
    {
        base.CheckWinCondition();
        if (consequtiveCorrect >= 5)
        {
            FinishedGame(true, 0);
        }

    }
}
