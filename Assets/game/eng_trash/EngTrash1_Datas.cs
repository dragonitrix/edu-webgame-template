using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "engTrash_", menuName = "Eng/trash/1")]
public class EngTrash1_Datas : ScriptableObject
{
    public EngTrash1_Data[] datas;
}

[Serializable]
public class EngTrash1_Data
{
    public string[] choices;
}