using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ptw_level_0", menuName = "ptw/LevelData")]
public class PTW_LeveData : ScriptableObject
{
    public PTW_Data[] datas;
}

[System.Serializable]
public class PTW_Data
{
    public string text;
    public Sprite sprite;
}