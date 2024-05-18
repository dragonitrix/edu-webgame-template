using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysHouse_MiniGameCondition : MonoBehaviour
{
    MysHouse_MiniGame parent;
    public bool isCorrected = false;

    public void SetCorrect()
    {
        if (isCorrected) return;
        isCorrected = true;
        parent.OnConditionChange();
    }
    public void SetIncorrect()
    {
        if(!isCorrected)
        isCorrected = false;
        parent.OnConditionChange();
    }

    public void SetParent(MysHouse_MiniGame parent)
    {
        this.parent = parent;
    }

}
