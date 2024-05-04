using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharHead_Part : MonoBehaviour
{
    public CHARHEAD_PART_TYPE type;

    public Image partImage;

    public Draggable draggable;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void InitPart()
    {
        var sprite = ((CharHead_GameController)GameController.instance).spriteKeyValuePairs[type.ToString()];
        partImage.sprite = sprite;
    }

}


public enum CHARHEAD_PART_TYPE
{
    chh_part_arc,
    chh_part_circle,
    chh_part_line_diagonal,
    chh_part_line_vertical,
    chh_part_spacial_dawchadaa,
    chh_part_spacial_dtawpadtak,
    chh_part_spacial_lawjulaa,
    chh_part_spacial_m,
    chh_part_spacial_maihun,
    chh_part_spacial_tawtaan,
}