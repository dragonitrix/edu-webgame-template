using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HobbiesThreeQuestion", menuName = "Eng/hobbiesjourney/HobbiesThreeQuestionScriptableObject", order = 3)]
public class HobbiesJourneyLevelThreeScriptableObject : ScriptableObject
{
    public List<HobbiesJourneyLevelThreeQuestion> questions;
}

[Serializable]
public class HobbiesJourneyLevelThreeQuestion
{
    public string soundID;
    public string correctAnswer;
    public string[] choiceText;
}
