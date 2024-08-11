using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "engplant1_0", menuName = "Eng/EngPlant/01")]
public class EngPlant1_Datas : ScriptableObject
{
    public EngPlant1_Data[] datas;
}

[Serializable]
public class EngPlant1_Data
{
    public string[] texts;
}