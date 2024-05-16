using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "invisChar_q0", menuName = "invisChar/QuestionData")]
public class InvisChar_QuestionData : ScriptableObject
{
    public string fulltext = "";
    public string hintText = "";
    public InvisChar_PartData[] data;
}

[System.Serializable]
public class InvisChar_PartData
{
    public enum TYPE
    {
        DROP,
        FIX
    }
    public TYPE type;
    public string text;
}