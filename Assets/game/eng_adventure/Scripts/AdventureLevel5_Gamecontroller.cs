using System.Collections;
using System.Collections.Generic;
using TMPro;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.UI;

public class AdventureLevel5_Gamecontroller : GameController
{
    [Header("Object Ref")]
    public Image imageAudio;
    public TextMeshProUGUI[] answerTexts;
    public TextMeshProUGUI headerText;
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
    public AdventureLevelFiveScriptableObject questionScriptableObject;
    List<AdventureLevelFiveQuestion> questions;
    Dictionary<string, AudioClip> alphabetsSounds = new Dictionary<string, AudioClip>();
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

        moneyText.text = "Money: " + ScoreManager.Instance.currentScore + "฿";

        level = (ADVENTURE_LEVEL)gameLevel;
        questions = new List<AdventureLevelFiveQuestion>(questionScriptableObject.questions);
        //questions.Shuffle();
        titleText.text = "Level:5";


        SetQuestion();
    }

    public void OnPlaySoundIconClick()
    {
        AudioManager.instance.PlaySound(questions[gameStage].questionClip);
    }

    public void SetQuestion()
    {
        imageAudio.sprite = questions[gameStage].questionSprites;
        headerText.text = questions[gameStage].headerText;
        List<string> choices = new List<string>(questions[gameStage].choiceTexts);
        choices.Shuffle();
        for (int i = 0; i < choices.Count; i++)
        {
            answerTexts[i].text = choices[i];
        }
    }

    public void OnCheckButtonClick(TextMeshProUGUI value)
    {
        CheckAnswer(value.text);
    }

    void CheckAnswer(string value)
    {
        if (value == questions[gameStage].correctAnswer)
        {
            //Debug.Log("answer corrected");
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(true, OnAnswerEffectComplete);
            ScoreManager.Instance.UpdateCurrentScore(20);
        }
        else
        {
            //Debug.Log("answer incorrect");
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(false, OnAnswerEffectComplete);
            ScoreManager.Instance.UpdateCurrentScore(-20);
        }
        moneyText.text = "Money: " + ScoreManager.Instance.currentScore + "฿";
    }

    void OnAnswerEffectComplete()
    {
        gameStage++;
        if (gameStage >= questions.Count)
        {
            gameState = GAME_STATE.ENDED;
        }

        if (gameState == GAME_STATE.ENDED)
        {
            winText.text = "คุณมีเงินสะสมทั้งหมด 20XX฿\r\nมีเงินซื้อของฝากให้เพื่อนแล้ว!";
            FinishedGame(true, 0);
        }
        else
        {
            SetQuestion();
        }
    }
}
