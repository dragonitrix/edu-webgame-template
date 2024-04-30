using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "wonderSound_level_0", menuName = "wonderSound/LevelData")]
public class WonderSound_LevelData : ScriptableObject
{
    public WonderSound_RoundData[] rounds;
}

[Serializable]
public class WonderSound_RoundData
{
    public string hint;
    public WonderSound_AnswerData correct_answer;
    public WonderSound_AnswerData[] wrong_answer;
}

[Serializable]
public class WonderSound_AnswerData
{
    public string text;
}
