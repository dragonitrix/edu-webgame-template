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

    void Awake()
    {
        bgImg = GetComponent<Image>();
        button = GetComponent<Button>();
        text = transform.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(OnClicked);
    }

    public void SetStatus(int index, bool tweenSize = true)
    {
        status = index;
        bgImg.DOColor(colors[status], tweenDuration);

        if (tweenSize) TweenSize(transform);
    }

    public void SetValue(string val, bool tweenSize = true)
    {
        value = val;
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

    void OnClicked()
    {
        AudioManager.instance.PlaySound("ui_click_1");
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

    public void HardReset()
    {
        SetValue("", false);
        SetEnableImage(false);
        SetEnableText(false);
        SetStatus(0);
    }

}
