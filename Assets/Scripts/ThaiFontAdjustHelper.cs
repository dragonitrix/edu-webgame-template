using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThaiFontAdjustHelper : MonoBehaviour
{
    
    public TextMeshProUGUI textMesh;


    [ContextMenu("AdjustThaiText")]
    public void AdjustText(){
        textMesh.AdjustTMPThaiText();
    }

}
