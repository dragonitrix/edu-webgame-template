using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    Animator animator;

    [Header("Button")]
    public Button closeButton;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        if (closeButton) closeButton.onClick.AddListener(Exit);
    }

    public void Enter()
    {
        animator.SetTrigger("FadeIn");
    }

    public void Exit()
    {
        animator.SetTrigger("FadeOut");
    }
}
