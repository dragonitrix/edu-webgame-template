using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "thaileadword_1", menuName = "Thai/leadword/Data")]
public class ThaiLeadword_Datas : ScriptableObject
{
    public ThaiLeadword_Data[] datas;
}

[System.Serializable]
public class ThaiLeadword_Data
{
    public string[] correct_chars;
    public ThaiLeadword_Wannayuk[] correct_wannayuks;
    public string[] spare_chars;
    public string[] spare_saras;
    public string[] spare_wannayuks;
}

[System.Serializable]
public class ThaiLeadword_Wannayuk
{
    public int index;
    public string up2;
    public string up1;
    public string down1;
}