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
                AudioManager.instance.PlaySound("ui_win_2");
                var flare = SpawnEffect("effect_flare", 0.5f, 0.1f, 0.2f, 1.6f);
                flare.SetRotate(45f);
                effect = SpawnEffect("effect_correct", 0.2f, 0f, 0.2f, 1.8f);
                break;
            case false:
                AudioManager.instance.PlaySound("ui_fail_1");
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

    public void SpawnNoAnswerPopup_tictactoe(UnityAction callback)
    {
        AudioManager.instance.PlaySound("ui_fail_1");
        SimpleEffect effect = null;
        switch (GameController.instance.playerCount)
        {
            case PLAYER_COUNT._1_PLAYER:
                effect = SpawnEffect("effect_noanswer_single", 0.2f, 0f, 0.2f, 0f, false);
                break;
            case PLAYER_COUNT._2_PLAYER:
                effect = SpawnEffect("effect_noanswer_multi", 0.2f, 0f, 0.2f, 0f, false);
                break;
        }
        effect.SetExitCallback(callback);
    }

    public void SpawnAnswerEffect_tictactoe(bool isCorrected, UnityAction callback)
    {
        SimpleEffect effect = null;

        switch (isCorrected)
        {
            case true:
                AudioManager.instance.PlaySound("ui_win_2");
                var flare = SpawnEffect("effect_flare", 0.5f, 0.1f, 0.2f, 1.6f);
                flare.SetRotate(45f);
                effect = SpawnEffect("effect_correct", 0.2f, 0f, 0.2f, 1.8f);
                break;
            case false:
                AudioManager.instance.PlaySound("ui_fail_1");
                switch (GameController.instance.playerCount)
                {
                    case PLAYER_COUNT._1_PLAYER:
                        effect = SpawnEffect("effect_incorrect_single", 0.2f, 0f, 0.2f, 0f, false);
                        break;
                    case PLAYER_COUNT._2_PLAYER:
                        effect = SpawnEffect("effect_incorrect_multi", 0.2f, 0f, 0.2f, 0f, false);
                        break;
                }
                break;
        }
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
