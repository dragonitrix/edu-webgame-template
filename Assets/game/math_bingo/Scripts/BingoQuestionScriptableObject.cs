using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BingoQuestion", menuName = "bingo/BingoQuestionScriptableObject", order = 1)]
public class BingoQuestionScriptableObject : ScriptableObject
{
    public List<BingoQuestion> questions;
}

[Serializable]
public class BingoQuestion
{
    public int answer;

    public List<Vector2> equations;
}