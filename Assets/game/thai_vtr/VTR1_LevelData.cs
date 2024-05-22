using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "vtr1_level_0", menuName = "vtr/LevelData1")]
public class VTR1_LevelData : ScriptableObject
{
    public TYPE type;
    public string[] texts;
    public enum TYPE
    {
        IMG,
        SOUND
    }
}
