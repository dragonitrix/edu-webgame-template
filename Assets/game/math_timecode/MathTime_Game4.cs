using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MathTime_Game4 : MathTime_Game
{
    [Header("Obj ref")]
    public ClockMoveable_Controller clock;
    public TextMeshProUGUI timeText;
    //data
    public string mainTimeString;
    bool isAnswering = false;
    public override void InitGame()
    {
        base.InitGame();

        var rTime = GetRandomTimeString();

        mainTimeString = rTime;

        var _timeString = rTime.Split(":");
        timeText.text = _timeString[0] + ":" + _timeString[1];

        isAnswering = false;
    }

    public void OnCheck()
    {
        if (isAnswering) return;
        isAnswering = true;

        if (To12Format(clock.timeString) == To12Format(mainTimeString))
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
