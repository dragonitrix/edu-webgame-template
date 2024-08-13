using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MathTime_Game1 : MathTime_Game
{
    [Header("Prefabs")]

    public GameObject choice_prefab;

    [Header("Obj ref")]

    public ClockUI_Controller mainClock;
    public RectTransform choicesRect;


    //data
    string mainTimeString;

    List<MathTime_ChoiceText> choices = new();

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

        mainClock.SetClock(timeString_12);

        var clone = Instantiate(choice_prefab, choicesRect);
        var script = clone.GetComponent<MathTime_ChoiceText>();
        script.InitData(this, timeString);
        choices.Add(script);

        for (int i = 0; i < 2; i++)
        {
            var subclone = Instantiate(choice_prefab, choicesRect);
            var subscript = subclone.GetComponent<MathTime_ChoiceText>();
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
        var choice = obj.GetComponent<MathTime_ChoiceText>();
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

}
