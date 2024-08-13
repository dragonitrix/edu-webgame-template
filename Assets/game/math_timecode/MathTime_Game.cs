using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MathTime_Game : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    public virtual void Enter()
    {
        canvasGroup.TotalHide();
        InitGame();
        canvasGroup.DOFade(1, 1).OnComplete(() =>
        {
            canvasGroup.TotalShow();
        });
    }

    public virtual void Exit()
    {
        canvasGroup.TotalHide();
        canvasGroup.DOFade(0, 1).From(1).OnComplete(() =>
        {

        });
    }

    public virtual void InitGame()
    {

    }

    public virtual void Correct()
    {
        ((MathTime_GameController)GameController.instance).OnSubGameAnswerCorrect();
        Exit();
    }

    public virtual void OnChoiceClick(GameObject obj)
    {

    }

    protected string GetRandomTimeString()
    {
        var rTime = UnityEngine.Random.Range(0, 24 * 60) * 60;
        var timeString = GetTimer(rTime);
        return timeString;
    }

    public string GetTimer(float timer)
    {
        System.TimeSpan time = System.TimeSpan.FromSeconds(timer);
        return time.ToString("hh':'mm':'ss");
    }

    public string To12Format(string timeString)
    {
        var hour = int.Parse(timeString.Split(":")[0]);

        if (hour >= 12)
        {
            hour -= 12;
            return hour.ToString("00") + ":" + timeString.Split(":")[1] + ":" + timeString.Split(":")[2];
        }
        else
        {
            return timeString;
        }
    }

    public bool CheckTimeDupe(string input, string main)
    {
        return To12Format(input) == To12Format(main);
    }

}
