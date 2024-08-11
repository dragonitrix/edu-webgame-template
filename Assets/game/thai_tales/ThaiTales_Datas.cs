using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "thaitales_", menuName = "Thai/Tales/Data")]


public class ThaiTales_Datas : ScriptableObject
{
    public ThaiTales_Data[] datas;
}

[Serializable]
public class ThaiTales_Data
{
    public string[] questions;

    public string[] choices;
}
