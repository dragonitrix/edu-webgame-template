using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "mathCard_0", menuName = "Math/Card")]
public class MathCard_CardData : ScriptableObject
{
    public Sprite sprite;
    public MATHCARD_CARD_TYPE type;
    public MATHCARD_CARD_EFFECT effect;
    public int value;
}

public enum MATHCARD_CARD_TYPE
{
    CAT,
    DOG,
    NEUTRAL
}
public enum MATHCARD_CARD_EFFECT
{
    ADD_NUMBER,
    ADD_POINT,
    DEDUCT_POINT,
    MULTIPLY_POINT,
}
