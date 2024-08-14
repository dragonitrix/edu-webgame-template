using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "engshare1_", menuName = "Eng/Share/1")]

public class EngShare1_Datas : ScriptableObject
{
    public EngShare1_Data[] datas;
}

[System.Serializable]
public class EngShare1_Data
{
    public string[] choices;
}