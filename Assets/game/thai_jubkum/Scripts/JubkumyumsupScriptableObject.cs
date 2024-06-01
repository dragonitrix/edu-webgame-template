using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JubkumQuestion", menuName = "Jubkumyumsup/JubkumQuestionScriptableObject", order = 1)]
public class JubkumyumsupScriptableObject : ScriptableObject
{
    public List<JubkumQuestion> questions;
}

[Serializable]
public class JubkumQuestion
{
    public string correctAnswer;
    public string checkText;
}
