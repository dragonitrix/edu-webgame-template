using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AdventureFourQuestion", menuName = "Eng/adventure/AdventureFourQuestionScriptableObject", order = 1)]
public class AdventureLevelFourQuestionScriptableObject : ScriptableObject
{
    public List<AdventureLevelFourQuestion> questions;
}

[Serializable]
public class AdventureLevelFourQuestion
{
    public AudioClip readingClip;
    public AudioClip fullWordClip;
    public string correctAnswer;
    public Sprite sprite;
}