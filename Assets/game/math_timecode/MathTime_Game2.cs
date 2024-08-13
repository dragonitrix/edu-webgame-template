using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MathTime_Game2 : MathTime_Game
{

    [Header("Prefabs")]

    public GameObject choice_prefab;

    [Header("Obj ref")]

    public TextMeshProUGUI mainText;
    public RectTransform choicesRect;


    //data
    string mainTimeString;

    List<MathTime_ChoiceClock> choices = new();

    bool isAnswering = false;

    public override void InitGame()
    {
        base.InitGame();

        foreach (var choice in choices)
        {
            DestroyImmediate(choice.gameObject);
        }
        choices.Clear();

        var timeString = GetRandomTimeString();
        var timeString_12 = To12Format(timeString);

        mainTimeString = timeString_12;

        var _timeString = timeString.Split(":");
        mainText.text = _timeString[0] + ":" + _timeString[1];

        var clone = Instantiate(choice_prefab, choicesRect);
        var script = clone.GetComponent<MathTime_ChoiceClock>();
        script.InitData(this, timeString);
        choices.Add(script);

        for (int i = 0; i < 2; i++)
        {
            var subclone = Instantiate(choice_prefab, choicesRect);
            var subscript = subclone.GetComponent<MathTime_ChoiceClock>();
            string subTimeString;
            do
            {
                subTimeString = GetRandomTimeString();
            } while (CheckTimeDupe(subTimeString, mainTimeString));
            subscript.InitData(this, subTimeString);
            choices.Add(subscript);
        }

        choices.Shuffle();

        foreach (var choice in choices)
        {
            choice.transform.SetAsLastSibling();
        }

        isAnswering = false;

    }

    public override void OnChoiceClick(GameObject obj)
    {
        if (isAnswering) return;
        var choice = obj.GetComponent<MathTime_ChoiceClock>();
        isAnswering = true;

        if (To12Format(choice.timeString) == To12Format(mainTimeString))
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

    public void SwitchMarker()
    {
        foreach (var choice in choices)
        {
            choice.clock.SwitchMarker();
        }
    }

}
