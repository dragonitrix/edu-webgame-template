using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelector : MonoBehaviour
{
    public Image cardImage;
    public Button cardButton;

    public Sprite sprite;
    public MATHCARD_CARD_TYPE type;
    public MATHCARD_CARD_EFFECT effect;
    public int value;

    public CardSelector InitCard(MathCard_CardData data)
    {
        sprite = data.sprite;
        type = data.type;
        effect = data.effect;
        value = data.value;
        cardButton = GetComponent<Button>();
        cardButton.onClick.RemoveAllListeners();

        cardImage.sprite = sprite;

        return this;
    }
}
