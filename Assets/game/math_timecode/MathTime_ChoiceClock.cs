using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MathTime_ChoiceClock : MonoBehaviour
{
    public string timeString;
    public ClockUI_Controller clock;
    public Button button;

    public MathTime_Game controller;

    void Awake()
    {
        button = GetComponent<Button>();
        clock = transform.GetChild(0).GetComponent<ClockUI_Controller>();
        button.onClick.AddListener(OnClick);
    }

    public void InitData(MathTime_Game controller, string timeString)
    {
        this.controller = controller;
        this.timeString = timeString;
        clock.SetClock(timeString);
    }

    void OnClick()
    {
        controller.OnChoiceClick(this.gameObject);
    }

}
