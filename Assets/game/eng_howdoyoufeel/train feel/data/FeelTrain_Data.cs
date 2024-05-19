using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "feeltrain_q_0", menuName = "feeltrain/Data")]
public class FeelTrain_Data : ScriptableObject
{
    public string subject;
    public string[] verbs;
    public string[] adjs;

    public int GetIndex(){
        return int.Parse(name.Split("_")[2]);
    }

}
