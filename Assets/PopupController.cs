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

    // Start is called before the first frame update
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        if (closeButton) closeButton.onClick.AddListener(Exit);
    }

    public virtual void Enter()
    {
        animator.SetTrigger("FadeIn");
    }

    public virtual void Exit()
    {
        animator.SetTrigger("FadeOut");
    }
}
