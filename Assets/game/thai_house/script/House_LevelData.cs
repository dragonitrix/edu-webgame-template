using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "house_level_0", menuName = "Thai/house/LevelData")]
public class House_LevelData : ScriptableObject
{
    public string[] houseTexts;
    public House_RoundData[] rounds;
}

[Serializable]
public class House_RoundData
{
    public string text;
    public int answer;
}
