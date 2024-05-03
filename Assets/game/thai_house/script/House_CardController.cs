using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class House_CardController : MonoBehaviour
{

    public RectTransform card_back;
    public RectTransform card_front;

    public bool facing = false;

    bool isFliping = false;

    public float flipDuration = 0.5f;

    public delegate void OnFlipFinishedDelegate();
    public OnFlipFinishedDelegate onFlipFinished;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Flip();
        }
    }

    [ContextMenu("Flip")]
    public void Flip()
    {
        FlipTo(!facing);
    }
    public void Flip(float delay = 0, bool skip = false)
    {
        FlipTo(!facing, delay, skip);
    }

    public void ToFront(float delay = 0, bool skip = false)
    {
        FlipTo(true, delay, skip);
    }

    public void ToBack(float delay = 0, bool skip = false)
    {
        FlipTo(false, delay, skip);
    }


    public void FlipTo(bool val, float delay = 0, bool skip = false)
    {
        FlipTo(val, () => { }, delay, skip);
    }

    public void FlipTo(bool val, UnityAction callback, float delay = 0f, bool skip = false)
    {

        if (isFliping) return;
        isFliping = true;

        facing = val;
        var duration = skip ? 0 : flipDuration / 2;



        switch (val)
        {
            case true:
                card_front.localScale = new Vector3(0, 1, 1);
                card_back.DOScaleX(0, duration).SetDelay(delay).OnStart(() =>
                {
                    AudioManager.instance.PlaySound("ui_swipe");
                }).OnComplete(
                    () =>
                    {
                        card_front.DOScaleX(1, duration).OnComplete(() =>
                        {
                            OnFlipFinished();
                            callback?.Invoke();
                        });
                    }
                );
                break;
            case false:
                card_back.localScale = new Vector3(0, 1, 1);
                card_front.DOScaleX(0, duration).SetDelay(delay).OnStart(() =>
                {
                    AudioManager.instance.PlaySound("ui_swipe");
                }).OnComplete(
                    () =>
                    {
                        card_back.DOScaleX(1, duration).OnComplete(() =>
                        {
                            OnFlipFinished();
                            callback?.Invoke();
                        });
                    }
                );
                break;
        }
    }

    public void OnFlipFinished()
    {
        isFliping = false;
        // call back
        Debug.Log("OnFlipFinished");
        onFlipFinished?.Invoke();
    }

    public void SetFrontImage(Sprite sprite)
    {
        card_front.GetComponent<Image>().sprite = sprite;
    }
}
