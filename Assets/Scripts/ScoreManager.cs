using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int finalScore { get { return _finalScore; } }
    int _finalScore = 0;
    public int finalMaxScore { get { return _finalMaxScore; } }
    int _finalMaxScore = 0;

    public int currentScore { get { return _currentScore; } }
    int _currentScore = 0;
    public int currentMaxScore { get { return _currentMaxScore; } }
    int _currentMaxScore = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(Instance);
    }
    public void UpdateFinalScore()
    {
        _finalScore += _currentScore;
    }
    public void UpdateFinalMaxScore()
    {
        _finalMaxScore += _currentMaxScore;
    }
    public void UpdateFinalScoreAndMaxScore()
    {
        UpdateFinalScore();
        UpdateFinalMaxScore();
    }
    public void UpdateCurrentScore(int value)
    {
        _currentScore += value;
    }
    public void UpdateCurrentScore(bool isCorrect)
    {
        _currentScore = isCorrect ? _currentScore + 1 : _currentScore;
    }
    public void UpdateCurrentScore(bool isCorrect, int value)
    {
        if (isCorrect)
            UpdateCurrentScore(value);
    }
    public void UpdateCurrentMaxScore(int value)
    {
        _currentMaxScore = value;
    }
    public void ResetCurrentScore()
    {
        _currentScore = 0;
    }
    public void ResetCurrentMaxScore()
    {
        _currentMaxScore = 0;
    }
    public void ResetFinalScore()
    {
        _finalScore = 0;
    }
    public void ResetFinalMaxScore()
    {
        _finalMaxScore = 0;
    }

    public void HardReset()
    {
        ResetCurrentScore();
        ResetCurrentMaxScore();
        ResetFinalScore();
        ResetFinalMaxScore();
    }
}
