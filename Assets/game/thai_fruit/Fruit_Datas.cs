using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "fruit_", menuName = "Thai/fruit/Data")]
public class Fruit_Datas : ScriptableObject
{
    public Fruit_Data[] datas;
}

[Serializable]
public class Fruit_Data
{
    public Fruit_Data_TYPE type;
    public string mainWord;
    public string maskWord;
    public string choices;

}

public enum Fruit_Data_TYPE{
    KON,
    KOK,
    KOB,
    KOD
}