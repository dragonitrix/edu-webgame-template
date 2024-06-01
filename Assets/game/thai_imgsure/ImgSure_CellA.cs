using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImgSure_CellA : MonoBehaviour
{
    [HideInInspector] public string answer;
    RectTransform rectTransform;
    ImgSure_GameController parent;
    TextMeshProUGUI text;
    public void InitCell(ImgSure_GameController parent, string answer)
    {
        rectTransform = GetComponent<RectTransform>();
        text = GetComponentInChildren<TextMeshProUGUI>();

        this.parent = parent;
        this.answer = answer;

        SetText(this.answer);

        rectTransform.localScale = Vector3.zero;
    }

    void SetText(string _text)
    {
        if (_text.Contains("-"))
        {
            _text = _text.Replace("-", "");

            switch (_text)
            {
                case "ุ":
                case "ู":
                    text.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, 80);
                    break;
                case "ำ":
                case "า":
                    text.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    break;
                case "้":
                case "๊":
                    text.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, -80);
                    break;
                default:
                    text.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, -50);
                    break;
            }

        }
        text.text = _text;
    }
    public void Show()
    {
        rectTransform.DOScale(Vector3.one, 0.25f);
    }

    public void Hide()
    {
        rectTransform.DOScale(Vector3.zero, 0.25f);
    }
}
