using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HouseCardController : MonoBehaviour
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
    public void Flip(bool skip = false)
    {
        FlipTo(!facing, skip);
    }

    public void ToFront(bool skip = false)
    {
        FlipTo(true, skip);
    }

    public void ToBack(bool skip = false)
    {
        FlipTo(false, skip);
    }

    public void FlipTo(bool val, bool skip = false)
    {

        if (isFliping) return;
        isFliping = true;

        facing = val;
        var duration = skip ? 0 : flipDuration / 2;

        switch (val)
        {
            case true:
                card_front.localScale = new Vector3(0, 1, 1);
                card_back.DOScaleX(0, duration).OnComplete(
                    () =>
                    {
                        card_front.DOScaleX(1, duration).OnComplete(OnFlipFinished);
                    }
                );
                break;
            case false:
                card_back.localScale = new Vector3(0, 1, 1);
                card_front.DOScaleX(0, duration).OnComplete(
                    () =>
                    {
                        card_back.DOScaleX(1, duration).OnComplete(OnFlipFinished);
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

}
