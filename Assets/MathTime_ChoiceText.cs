using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MathTime_ChoiceText : MonoBehaviour
{
    public string timeString;

    public TextMeshProUGUI text;

    public Button button;

    public MathTime_Game controller;

    void Awake()
    {
        button = GetComponent<Button>();
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        button.onClick.AddListener(OnClick);
    }

    public void InitData(MathTime_Game controller, string timeString)
    {
        this.controller = controller;
        this.timeString = timeString;
        var _timeString = timeString.Split(":");
        text.text = _timeString[0] + ":" + _timeString[1];
    }

    void OnClick()
    {
        controller.OnChoiceClick(this.gameObject);
    }

}
