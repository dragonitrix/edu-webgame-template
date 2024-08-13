using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MathTime_Digit : MonoBehaviour
{
    public int value;
    public int max;
    public TextMeshProUGUI text;
    public Button up;
    public Button down;

    void Awake()
    {
        up.onClick.AddListener(Up);
        down.onClick.AddListener(Down);
    }

    public void SetValue(int value)
    {
        if (value < 0 || value > max) return;
        this.value = value;

        text.text = value.ToString();

        up.interactable = !(value >= max);
        down.interactable = !(value <= 0);
    }

    public void Up()
    {
        AudioManager.instance.PlaySound("ui_click_1");
        SetValue(value + 1);
    }

    public void Down()
    {
        AudioManager.instance.PlaySound("ui_click_1");
        SetValue(value - 1);
    }

}
