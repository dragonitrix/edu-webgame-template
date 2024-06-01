using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HobbiesTwoQuestion", menuName = "hobbiesjourney/HobbiesTwoQuestionScriptableObject", order = 2)]
public class HobbiesJourneyLevelTwoScriptableObject : ScriptableObject
{
    public List<HobbiesJourneyLevelTwoQuestion> questions;
}

[Serializable]
public class HobbiesJourneyLevelTwoQuestion
{
    public string correctAnswer;
    public string[] choiceText;
    public int maxLettersLength;
    public string extensionText;
    public string blankSlotText;
}
