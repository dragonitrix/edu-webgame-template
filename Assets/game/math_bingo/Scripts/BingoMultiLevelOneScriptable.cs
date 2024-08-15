using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BingoMultiQuestion", menuName = "bingo/BingoMultiQuestionScriptableObject", order = 2)]
public class BingoMultiLevelOneScriptable : ScriptableObject
{
    public List<BingoMultiLevelOneQuestion> questions;
}


[Serializable]
public class BingoMultiLevelOneQuestion
{
    public string answer;

    public Vector2 equations;

    public Sprite equationSprite;
}
