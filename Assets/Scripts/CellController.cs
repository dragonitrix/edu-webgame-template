using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CellController : MonoBehaviour
{

    Image bgImg;
    Button button;
    TextMeshProUGUI text;
    Image img;

    [Header("Settings")]
    public float tweenDuration = 0.2f;
    public List<Color> colors = new List<Color>();
    public int status = 0;
    public string value = "-";
    public int index = 0;
    public bool disableButton = false;

    public delegate void OnClickedDelegate(CellController cell);

    public OnClickedDelegate onClicked;

    void Awake()
    {
        bgImg = GetComponent<Image>();
        button = GetComponent<Button>();
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        img = transform.GetChild(1).GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(OnClickedDefault);
        SetEnableButton(!disableButton);
    }

    public void SetStatus(int index, bool tween = true)
    {
        status = index;
        if (tween)
        {
            TweenSize(transform);
            bgImg.DOColor(colors[status], tweenDuration);
        }
        else
        {
            bgImg.color = colors[status];
        }
    }

    public void SetValue(string val, bool tweenSize = true)
    {
        value = val;
        text.text = value;
        if (tweenSize) TweenSize(text.transform);
    }

    public void SetImage(Sprite sprite, bool tweenSize = true)
    {
        img.sprite = sprite;
        if (tweenSize) TweenSize(img.transform);
    }

    void TweenSize(Transform t)
    {
        t.DOScale(Vector3.zero, tweenDuration / 2f).OnComplete(
            () =>
            {
                t.DOScale(Vector3.one, tweenDuration / 2f);
            }
        );
    }

    void OnClickedDefault()
    {
        AudioManager.instance.PlaySound("ui_click_1");
        //DebugButtonClick();
        onClicked(this);
    }


    void DebugButtonClick()
    {
        status++;
        if (status >= colors.Count) status -= colors.Count;
        SetStatus(status);

        GameController.instance.CheckWinCondition();

    }

    public void SetEnableText(bool val)
    {
        text.color = new Color(
            text.color.r,
            text.color.g,
            text.color.b,
            val ? 1 : 0
        );
    }

    public void SetEnableImage(bool val)
    {
        img.color = new Color(
            img.color.r,
            img.color.g,
            img.color.b,
            val ? 1 : 0
        );
    }

    public void SetEnableButton(bool val)
    {
        button.interactable = val;
    }

    public void HardReset()
    {
        SetValue("", false);
        SetStatus(0, false);
        SetEnableImage(false);
        SetEnableText(false);
    }

}
