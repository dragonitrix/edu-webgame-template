using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SimpleEffectController : MonoBehaviour
{
    public static SimpleEffectController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public List<EffectData> effectDatas = new List<EffectData>();
    Dictionary<string, GameObject> effectDataDictionary;

    public RectTransform effectRect;

    void Start()
    {
        effectDataDictionary = effectDatas.ToDictionary(x => x.id, x => x.prefab);
    }

    public void SpawnAnswerEffect(bool isCorrected, UnityAction callback)
    {

        SimpleEffect effect = null;

        switch (isCorrected)
        {
            case true:
                var flare = SpawnEffect("effect_flare", 0.5f, 0.1f, 0.2f, 1.6f);
                flare.SetRotate(45f);
                effect = SpawnEffect("effect_correct", 0.2f, 0f, 0.2f, 1.8f);
                break;
            case false:
                effect = SpawnEffect("effect_incorrect", 0.2f, 0f, 0.2f, 1.8f);
                break;
        }

        effect.SetExitCallback(callback);
    }

    public void SpawnWaitPopup(UnityAction callback)
    {
        var effect = SpawnEffect("effect_popup1", 0.2f, 0f, 0.2f, 0f, false);
        effect.SetExitCallback(callback);
    }

    SimpleEffect SpawnEffect(string id, float enterDuration, float enterDelay, float exitDuration, float exitDelay, bool chainToExit = true, bool autoStart = true)
    {
        var prefab = effectDataDictionary[id];
        var clone = Instantiate(prefab, effectRect);
        var simpleEffect = clone.GetComponent<SimpleEffect>();
        simpleEffect.Init(enterDuration, enterDelay, exitDuration, exitDelay, chainToExit, autoStart);
        return simpleEffect;
    }



}

[Serializable]
public class EffectData
{
    public string id;
    public GameObject prefab;
}
