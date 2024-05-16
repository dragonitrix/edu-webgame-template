using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SimpleIntroController : MonoBehaviour
{
    public SimpleIntroPage[] pages;

    CanvasGroup canvasGroup;

    int pageIndex;

    public Button nextPageButton;

    public delegate void OnIntroFinishedDelegate();
    public OnIntroFinishedDelegate onIntroFinished;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        if (nextPageButton)
        {
            nextPageButton.onClick.AddListener(OnNextPageClick);
            var btn_cg = nextPageButton.GetComponent<CanvasGroup>();
            btn_cg.TotalHide();
        }

    }

    public void Show()
    {
        // setup page
        foreach (var page in pages)
        {
            page.onIntroPageStart += OnPageStart;
            page.onIntroPageFinished += OnPageFinshed;
        }

        pageIndex = 0;
        canvasGroup.DOFade(1f, 1f).OnComplete(() =>
        {
            canvasGroup.TotalShow();
            ToPage(0);
        });
    }

    public void Hide()
    {
        canvasGroup.DOFade(0f, 1f).OnComplete(() =>
        {
            canvasGroup.TotalHide();
        });
    }

    void NextPage()
    {
        ToPage(pageIndex + 1);
    }
    void ToPage(int index)
    {
        if (index < 0 || index >= pages.Length)
        {
            OnInvalidPage();
            return;
        }

        pageIndex = index;
        pages[pageIndex].StartIntroPage();

        if (nextPageButton)
        {
            var btn_cg = nextPageButton.GetComponent<CanvasGroup>();
            btn_cg.TotalHide();
        }
    }

    void OnInvalidPage()
    {
        FinishIntro();
    }

    void FinishIntro()
    {
        onIntroFinished?.Invoke();
        Hide();
    }

    void OnNextPageClick()
    {
        NextPage();
    }

    void OnPageStart()
    {

    }

    void OnPageFinshed()
    {
        if (nextPageButton)
        {
            var btn_cg = nextPageButton.GetComponent<CanvasGroup>();
            btn_cg.TotalShow();
            btn_cg.DOFade(1f, 1f).From(0f);
        }
    }

}
