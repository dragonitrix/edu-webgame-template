using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "subject_", menuName = "Thai/subject/Data")]
public class ThaiSubject_Datas : ScriptableObject
{
    public ThaiSubject_Data[] datas;
}

[System.Serializable]
public class ThaiSubject_Data
{
    public int[] answers;
}
