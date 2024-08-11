using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "engplant2_0", menuName = "Eng/EngPlant/02")]
public class EngPlant2_Datas : ScriptableObject
{
    public EngPlant2_Data[] datas;
}

[Serializable]
public class EngPlant2_Data
{
    public string mainWord;
    public string maskWord;
    public string choices;
}