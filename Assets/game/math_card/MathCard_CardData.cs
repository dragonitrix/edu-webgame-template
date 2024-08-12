using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "mathCard_0", menuName = "Math/Card")]
public class MathCard_CardData : ScriptableObject
{
    public Sprite sprite;
    public MATHCARD_CARD_TYPE type;
    public int value;
}

public enum MATHCARD_CARD_TYPE
{
    ADD_DOG,
    ADD_CAT,
    ADD_POINT,
    DEDUCT_POINT,
    MULTIPLY_DOG,
    MULTIPLY_CAT,
}
