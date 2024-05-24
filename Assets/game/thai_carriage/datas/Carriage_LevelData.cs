using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "carriage_level_0", menuName = "carriage/LevelData")]
public class Carriage_LevelData : ScriptableObject
{
    public string question;
    public string[] choices;
}
