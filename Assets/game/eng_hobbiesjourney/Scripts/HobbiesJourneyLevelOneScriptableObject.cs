using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HobbiesOneQuestion", menuName = "Eng/hobbiesjourney/HobbiesOneQuestionScriptableObject", order = 1)]
public class HobbiesJourneyLevelOneScriptableObject : ScriptableObject
{
    public List<HobbiesJourneyLevelOneQuestion> questions;
}

[Serializable]
public class HobbiesJourneyLevelOneQuestion
{
    public string correctAnswer;
    public List<string> choiceTexts;
    public string extensionText;
}