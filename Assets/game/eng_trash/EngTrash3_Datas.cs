using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EngTrash3_", menuName = "Eng/EngTrash/3")]
public class EngTrash3_Datas : ScriptableObject
{
    public EngTrash3_Data[] datas;
}

[Serializable]
public class EngTrash3_Data
{
    public string mainWord;
    public string maskWord;
    public string choices;
}