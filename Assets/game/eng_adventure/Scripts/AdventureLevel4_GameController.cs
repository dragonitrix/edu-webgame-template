using System.Collections;
using System.Collections.Generic;
using TMPro;
using TransitionsPlus;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class AdventureLevel4_GameController : GameController
{
    [Header("Object Ref")]
    public GridController mainGridController;
    public AudioSource questionAudioSource;
    public AudioSource completeAudioSource;
    public AudioSource dropzoneAudioSource;
    public Droppable dropzone;
    public TextMeshProUGUI dropzoneText;

    [Header("Transition")]
    public TransitionProfile transitionProfile;
    public TransitionAnimator transitionAnimator;
    public RawImage transitionRenderer;
    public Image bgImage;

    [Header("Game Settings")]
    public TextMeshProUGUI titleText;
    public ADVENTURE_LEVEL level;

    [Header("Game Value")]
    public AudioClip[] alphabetsAudioClips;
    public AdventureLevelFourQuestionScriptableObject questionScriptableObject;
    List<AdventureLevelFourQuestion> questions;
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

        level = (ADVENTURE_LEVEL)gameLevel;
        mainGridController.InitGrid();
        var mainGridCells = mainGridController.cells;
        char[] alphabetsChar = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        questions = new List<AdventureLevelFourQuestion>(questionScriptableObject.questions);
        questions.Shuffle();
        titleText.text = "Level:4";
        foreach (char c in alphabetsChar)
        {
            alphabets.Add(c.ToString());
        }

        for (int i = 0; i < alphabets.Count; i++)
        {
            alphabetsSounds.Add(alphabets[i], alphabetsAudioClips[i]);
        }

        for (int i = 0; i < mainGridCells.Count; i++)
        {
            mainGridCells[i].SetValue(alphabets[i], false);
            mainGridCells[i].SetEnableText(true);

        }
        dropzone.onDropped += OnDroppingCell;
        SetQuestion();
    }

    void OnDroppingCell(Droppable droppable, Draggable draggable)
    {
        CellController draggingCell = draggable.GetComponent<CellController>();
        if (dropzoneText.text.Length < 5)
        {
            dropzoneText.text += draggingCell.value;
            dropzoneAudioSource.PlayOneShot(alphabetsSounds[draggingCell.value]);
        }
    }

    public void OnPlaySoundIconClick()
    {
        questionAudioSource.PlayOneShot(questions[gameStage].readingClip);
    }

    public void SetQuestion()
    {
        ClearDropzoneText();
    }

    void ClearDropzoneText()
    {
        dropzoneText.text = "";
    }

    public void OnCheckButtonClick()
    {
        CheckAnswer();
    }

    public void OnResetButtonClick()
    {
        ClearDropzoneText();
    }

    void CheckAnswer()
    {
        if (dropzoneText.text == questions[gameStage].correctAnswer)
        {
            //Debug.Log("answer corrected");
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(true, OnAnswerTrueEffectComplete);
            // increase score
        }
        else
        {
            //Debug.Log("answer incorrect");
            SimpleEffectController.instance.SpawnAnswerEffect_tictactoe(false, OnAnswerEffectComplete);
            // minus score
        }
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
            FinishedGame(true, 0);
        }
        else
        {
            SetQuestion();
        }
    }
    void OnAnswerTrueEffectComplete()
    {
        completeAudioSource.PlayOneShot(questions[gameStage].fullWordClip);
        OnAnswerEffectComplete();
    }
}
