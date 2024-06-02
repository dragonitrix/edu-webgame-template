using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HowmuchQuestion", menuName = "howmuch/HowmuchQuestionScriptableObject", order = 1)]
public class HowmuchQuestionScriptableObject : ScriptableObject
{
    public List<HowmuchQuestion> questions;
}

[Serializable]
public class HowmuchQuestion
{
    public Sprite questionSprite;
    public Sprite questionMiniIcon;
    public int correctAnswer;
    [Range(2,9)]
    public int dividedBox;
    [TextArea(2, 3)]
    public string questionText;
    public GameObject coinsPrefab;
}
