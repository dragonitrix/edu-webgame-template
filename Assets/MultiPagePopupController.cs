
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MultiPagePopupController : PopupController
{
    public PageController pageController;

    protected override void Start()
    {
        base.Start();
    }

    public override void Enter()
    {
        base.Enter();
        pageController.ToPage(0);
    }
}
