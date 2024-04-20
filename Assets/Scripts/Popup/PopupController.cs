using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    Animator animator;

    [Header("Button")]
    public Button closeButton;

    public delegate void OnPopupEnterDelegate();
    public OnPopupEnterDelegate OnPopupEnter;

    public delegate void OnPopupExitDelegate();
    public OnPopupExitDelegate OnPopupExit;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (closeButton) closeButton.onClick.AddListener(Exit);
    }

    [ContextMenu("Enter")]
    public virtual void Enter()
    {
        animator.SetTrigger("FadeIn");
    }


    [ContextMenu("Exit")]
    public virtual void Exit()
    {
        animator.SetTrigger("FadeOut");
    }
}
