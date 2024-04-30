using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInGrid : MonoBehaviour
{
    public int buttonNumber = 0;
    public TextMeshProUGUI buttonText;

    public void initiateButton(int number)
    {
        buttonNumber = number;
        buttonText.text = buttonNumber.ToString();
    }
}
