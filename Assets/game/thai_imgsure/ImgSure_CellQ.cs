using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ImgSure_CellQ : MonoBehaviour
{
    [HideInInspector] public bool isCorrect;
    RectTransform rectTransform;
    ImgSure_GameController parent;
    RectTransform outline;
    Image image;
    Button button;
    [HideInInspector] public Droppable droppable;
    Sprite qSprite;
    Sprite aSprite;
    string answer;
    string id;

    public void InitCell(ImgSure_GameController parent, string id, string answer, Sprite qSprite, Sprite aSprite)
    {
        rectTransform = GetComponent<RectTransform>();
        outline = transform.GetChild(0).GetComponent<RectTransform>();
        image = transform.GetChild(1).GetComponent<Image>();
        button = GetComponent<Button>();
        droppable = GetComponent<Droppable>();

        this.parent = parent;
        this.id = id;
        this.answer = answer;
        this.qSprite = qSprite;
        this.aSprite = aSprite;

        isCorrect = false;

        outline.localScale = Vector3.zero;
        image.sprite = this.qSprite;
        rectTransform.localScale = Vector3.zero;

        button.onClick.AddListener(OnClick);
        droppable.onDropped += OnDrop;
    }

    public void Show(float delay = 0)
    {
        rectTransform.DOScale(Vector3.one, 0.25f).SetDelay(delay);
    }

    public void Hide()
    {
        rectTransform.DOScale(Vector3.zero, 0.25f);
    }

    public void SetCorrect()
    {
        if (isCorrect) return;

        isCorrect = true;

        AudioManager.instance.PlaySound("ui_ding");

        rectTransform.DOScale(Vector3.zero, 0.25f).OnComplete(() =>
        {
            image.sprite = aSprite;
            rectTransform.DOScale(Vector3.one, 0.25f);
        });

    }

    void OnClick()
    {
        if (AudioManager.instance.GetSource(AudioManager.Channel.SPECIAL).isPlaying) return;
        outline.DOScale(Vector3.one, 0.5f);
        AudioManager.instance.PlaySpacialSound("ims_" + id, () =>
        {
            outline.DOScale(Vector3.zero, 1f);
        });
    }

    void OnDrop(Droppable dropable, Draggable dragable)
    {
        //compare
        var cell = dragable.GetComponent<ImgSure_CellA>();

        Debug.Log(this.answer + " " + cell.answer);
        if (this.answer == cell.answer)
        {
            SetCorrect();
            parent.CheckCorrect();
        }
        else
        {
            AudioManager.instance.PlaySound("ui_fail_1");
        }
    }

}
