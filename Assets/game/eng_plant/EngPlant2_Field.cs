using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EngPlant2_Field : MonoBehaviour
{
    public int index;
    public EngPlant2_GameController parent;
    public Droppable droppable;
    public Image pole;
    public RectTransform check;
    public bool isCorrect = false;

    void Start()
    {
        droppable.onDropped += OnDropped;
    }

    public void InitField()
    {
        pole.sprite = parent.spriteKeyValuePairs["02-q-" + (index + 1).ToString("00")];
    }

    void OnDropped(Droppable droppable, Draggable draggable)
    {
        if (isCorrect) return;

        var seed = draggable.GetComponent<EngPlant2_Seed>();

        if (seed.index != index)
        {
            SimpleEffectController.instance.SpawnAnswerEffectMinimal(false, () =>
            {
            }, 0.5f);
        }
        else
        {
            SimpleEffectController.instance.SpawnAnswerEffectMinimal(true, () =>
            {
                parent.OnFieldDrop(index, this);
            }, 0.5f);
        }

    }

    public void SetCorrect()
    {
        isCorrect = true;
        check.DOScale(Vector3.one, 0.5f);
    }

}
