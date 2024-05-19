using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AdventureFiveQuestion", menuName = "adventure/AdventureFiveQuestionScriptableObject", order = 1)]
public class AdventureLevelFiveScriptableObject : ScriptableObject
{
    public List<AdventureLevelFiveQuestion> questions;
}

[Serializable]
public class AdventureLevelFiveQuestion
{
    public string headerText;
    public Sprite questionSprites;
    public string correctAnswer;
    public AudioClip questionClip;
    public List<string> choiceTexts;
}