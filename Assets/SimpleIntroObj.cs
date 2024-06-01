using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class SimpleIntroObj : MonoBehaviour
{
    public float duration;

    public string soundID;

    public void Show()
    {
        var rt = GetComponent<RectTransform>();
        rt.DOScale(Vector3.one, 0.5f);
        AudioManager.instance.PlaySound("ui_pop");
        if (soundID != "")
        {
            AudioManager.instance.PlaySound(soundID);
        }
    }

    public void Reset()
    {
        var rt = GetComponent<RectTransform>();
        rt.localScale = Vector3.zero;
    }

}
