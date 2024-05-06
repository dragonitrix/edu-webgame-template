using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Wannayuuk_Cell : MonoBehaviour
{
    [HideInInspector]
    public RectTransform rectTransform;

    [HideInInspector]
    public Button button;

    [HideInInspector]
    public Wannayuuk_GameController parent;

    public WANNAYUUK_TYPE type;
    public Image image;
    public RectTransform correctRect;

    [HideInInspector]
    public bool isCorrected = false;

    public void Initcell(WANNAYUUK_TYPE type, Sprite image)
    {
        this.type = type;
        this.image.sprite = image;
        rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.zero;
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void SetOffset(float offsetY)
    {
        rectTransform.anchoredPosition = new Vector2(0, offsetY);
    }

    public void Show(float delay = 0f)
    {
        rectTransform.DOScale(Vector2.one, 0.2f).SetDelay(delay).OnPlay(() =>
        {
            AudioManager.instance.PlaySound("ui_pop");
        });
    }

    public void Kill(bool force = false)
    {
        if (force)
        {
            DestroyImmediate(transform.parent.gameObject);
        }
        else
        {
            Destroy(transform.parent.gameObject);
        }
    }

    void OnClick()
    {
        parent.OnCellClick(this);
    }

    public void SetCorrect()
    {
        isCorrected = true;
        correctRect.DOScale(Vector2.one, 0.2f);
        button.interactable = false;
    }

}
