#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
[ExecuteInEditMode]
public class ThaiFontAdjustHelper : MonoBehaviour
{
    public TextMeshProUGUI textMesh;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.AdjustTMPThaiText();
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
    }

    private void OnEnable()
    {
    }

    void ON_TEXT_CHANGED(UnityEngine.Object obj)
    {
        if (obj == textMesh)
        {
            textMesh.AdjustTMPThaiText();
            //LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }
    }

    [ContextMenu("AdjustThaiText")]
    public void AdjustText()
    {
        textMesh.AdjustTMPThaiText();
    }

}
#endif
