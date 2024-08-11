using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "engplant3_0", menuName = "Eng/EngPlant/03")]
public class EngPlant3_Datas : ScriptableObject
{
    public EngPlant3_Data[] datas;
}

[Serializable]
public class EngPlant3_Data
{
    public string mainWord;
    public string maskWord;
    public string[] choices;
}