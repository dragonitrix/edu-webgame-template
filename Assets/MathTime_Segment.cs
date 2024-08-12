using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class MathTime_Segment : MonoBehaviour
{
    public bool isCorrect = false;
    public int levelIndex;
    public int roundIndex;
    public bool glow = false;

    TextMeshProUGUI text;
    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        button.onClick.AddListener(OnClick);
    }

    public void SetCorrect()
    {
        if (isCorrect) return;
        isCorrect = true;

        var textrt = text.rectTransform;
        textrt.DOScale(0, 0.3f);

        var image = GetComponent<Image>();
        ColorUtility.TryParseHtmlString("#68C378", out Color color);
        image.DOColor(glow ? color : Color.white, 1);

    }

    void OnClick()
    {
        ((MathTime_GameController)GameController.instance).OnSegmentClick(this);
    }


}
