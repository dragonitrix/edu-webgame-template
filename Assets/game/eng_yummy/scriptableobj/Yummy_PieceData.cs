using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "yummy_piece_0", menuName = "yummy/PieceData")]
public class Yummy_PieceData : ScriptableObject
{
    public string correctWord;
    public string[] wrongWords;
}
