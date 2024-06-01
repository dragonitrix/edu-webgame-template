using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "yummy_level_0", menuName = "yummy/LevelData")]
public class Yummy_LevelData : ScriptableObject
{
    public Yummy_RoundData[] rounds;
}
[Serializable]
public class Yummy_RoundData
{
    public Yummy_PieceData[] pieceDatas;
}
