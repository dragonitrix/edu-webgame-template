using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MathTime_Game3 : MathTime_Game
{


    [Header("Obj ref")]
    public ClockUI_Controller clock;
    public MathTime_Digit hour1;
    public MathTime_Digit hour2;
    public MathTime_Digit minute1;
    public MathTime_Digit minute2;

    //data
    public string mainTimeString;

    bool isAnswering = false;
    public override void InitGame()
    {
        base.InitGame();

        hour1.SetValue(0);
        hour2.SetValue(0);
        minute1.SetValue(0);
        minute2.SetValue(0);

        var rTime = GetRandomTimeString();
        mainTimeString = rTime;
        clock.SetClock(rTime);

        isAnswering = false;
    }

    public void OnCheck()
    {
        if (isAnswering) return;
        isAnswering = true;
        var hourString = hour1.value + "" + hour2.value;
        var minuteString = minute1.value + "" + minute2.value;

        var answerString = hourString + ":" + minuteString + ":00";


        if (To12Format(answerString) == To12Format(mainTimeString))
        {
            SimpleEffectController.instance.SpawnAnswerEffect(true, () =>
            {
                Correct();
            });
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffect(false, () =>
            {
                isAnswering = false;
            });
        }

    }

}
