using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InvisChar_ButtonNumber : MonoBehaviour
{
    public int index;
    public TextMeshProUGUI text;
    InvisChar_GameController parent;
    Button button;
    CanvasGroup canvasGroup;

    public void Setup(int index, InvisChar_GameController parent)
    {
        this.index = index;
        this.parent = parent;

        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        text.text = (index + 1).ToString();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnClick()
    {
        parent.OnNumberClick(this);
    }

    void SetEnable(bool val)
    {
        if (val)
        {
            button.interactable = true;
            canvasGroup.alpha = 1;
        }
        else
        {
            button.interactable = false;
            canvasGroup.alpha = 0.5f;
        }
    }

}
