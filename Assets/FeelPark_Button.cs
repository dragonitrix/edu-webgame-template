using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeelPark_Button : MonoBehaviour
{
    public bool isCorrected = false;
    public TextMeshProUGUI text;
    FeelPark_GameController parent;

    Button button;

    public void Setup(string text, bool isCorrected, FeelPark_GameController parent)
    {
        this.text.text = text;
        this.isCorrected = isCorrected;
        this.parent = parent;

        button.onClick.AddListener(OnClick);

    }

    void OnClick()
    {
        parent.OnAnswerClick(this);
    }

}
