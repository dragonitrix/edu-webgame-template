using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThaiTales_Choice : MonoBehaviour
{

    public Button button;
    public TextMeshProUGUI text;

    public int index;

    public ThaiTales_GameController parent;

    void Awake()
    {
        button = GetComponent<Button>();
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        parent.OnChoiceClick(this);
    }

    public void InitChoice(ThaiTales_GameController parent, int index, string text)
    {
        this.parent = parent;
        this.index = index;
        this.text.text = text;
    }

}
