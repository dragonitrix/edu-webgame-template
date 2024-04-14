using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SimpleEffect : MonoBehaviour
{
    public delegate void EnterCompleteCallbackDelegate();
    public delegate void ExitCompleteCallbackDelegate();

    public EnterCompleteCallbackDelegate enterCompleteCallback;
    public ExitCompleteCallbackDelegate exitCompleteCallback;

    float enterDuration = 0.5f;
    float enterDelay = 0f;
    float exitDuration = 0.5f;
    float exitDelay = 0f;
    bool chainToExit = true;

    bool rotate = false;
    float rotateSpeed = 45f;

    public RectTransform main_transform;

    public void Init(float enterDuration, float enterDelay, float exitDuration, float exitDelay, bool chainToExit = true, bool autoStart = true)
    {
        this.enterDuration = enterDuration;
        this.enterDelay = enterDelay;
        this.exitDuration = exitDuration;
        this.exitDelay = exitDelay;
        main_transform.localScale = Vector3.zero;
        this.chainToExit = chainToExit;

        if (autoStart) Enter();
    }

    public void SetRotate(float speed)
    {
        rotate = true;
        rotateSpeed = speed;
    }

    public virtual void Enter()
    {
        var tween1 = main_transform.DOScale(Vector3.one, enterDuration).SetDelay(enterDelay);

        if (chainToExit) tween1.OnComplete(() =>
        {
            Exit();
        });
    }

    public virtual void Exit()
    {
        var tween1 = main_transform.DOScale(Vector3.zero, exitDuration).SetDelay(exitDelay);
        tween1.OnComplete(() =>
        {
            exitCallback?.Invoke();
            Destroy(gameObject);
        });
    }

    void Update()
    {
        if (rotate)
        {
            main_transform.Rotate(new Vector3(0, 0, rotateSpeed * Time.deltaTime));
        }
    }

    UnityAction exitCallback;

    public void SetExitCallback(UnityAction callback)
    {
        exitCallback = callback;
    }

}
