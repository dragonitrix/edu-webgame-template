using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SimpleIntroPage : MonoBehaviour
{

    public SimpleIntroObj[] objs;

    public delegate void OnIntroPageStartDelegate();
    public delegate void OnIntroPageFinishedDelegate();

    public OnIntroPageStartDelegate onIntroPageStart;
    public OnIntroPageFinishedDelegate onIntroPageFinished;

    CanvasGroup canvasGroup;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    public void StartIntroPage()
    {
        ResetObjs();
        StartCoroutine(_StartIntroPage());
    }

    IEnumerator _StartIntroPage()
    {

        onIntroPageStart?.Invoke();

        canvasGroup.DOFade(1f, 0.5f);
        yield return new WaitForSeconds(0.5f);

        foreach (var obj in objs)
        {
            obj.Show();
            yield return new WaitForSeconds(obj.duration);
        }

        onIntroPageFinished?.Invoke();

        yield return null;
    }

    public void ResetObjs()
    {
        foreach (var obj in objs)
        {
            obj.Reset();
        }
    }

    public void ResetCallback(){
        onIntroPageStart = ()=>{};
        onIntroPageFinished = ()=>{};
    }

}
