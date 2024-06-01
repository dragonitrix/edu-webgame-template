using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "feelpark_q_0", menuName = "feelpark/Data")]
public class FeelPark_Data : ScriptableObject
{
    public string question;

    public string answerPrefix;

    public string[] choices;

    public int GetIndex()
    {
        return int.Parse(name.Split("_")[2]);
    }

}
