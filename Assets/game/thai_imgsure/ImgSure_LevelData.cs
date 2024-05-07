using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "imgSure_level_0", menuName = "imgSure/LevelData")]
public class ImgSure_LevelData : ScriptableObject
{
    public ImgSure_RoundData[] rounds;
}


[Serializable]
public class ImgSure_RoundData
{
    public string[] answers;

    public ImgSure_ChoicesRow[] choices;

}

[Serializable]
public class ImgSure_ChoicesRow
{
    public string[] choices;
}
