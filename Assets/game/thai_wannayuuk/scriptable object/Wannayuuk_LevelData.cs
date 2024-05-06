using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "wannayuuk_level_0", menuName = "wannayuuk/LevelData")]
public class Wannayuuk_LevelData : ScriptableObject
{
    public Wannayuuk_RoundData[] roundDatas;
}

[Serializable]
public class Wannayuuk_RoundData
{
    public WANNAYUUK_TYPE target;

    public int TOH_Count;
    public int TRI_Count;

    public List<Sprite> TOHs;
    public List<Sprite> TRIs;
    public int themeIndex;
}

public enum WANNAYUUK_TYPE
{
    // AEK,
    TOH,
    TRI,
    // JUTTAWA
}