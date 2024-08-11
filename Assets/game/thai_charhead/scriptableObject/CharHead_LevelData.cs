using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "charHead_level_0", menuName = "Thai/charHead/LevelData")]
public class CharHead_LevelData : ScriptableObject
{
    public CharHead_RoundData[] rounds;
}

[Serializable]
public class CharHead_RoundData
{
    public GameObject[] chars;
}
