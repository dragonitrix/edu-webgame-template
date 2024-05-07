using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House_LevelPresetController : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject bigHouse_prefabs;
    public GameObject smallHouse_prefabs;
    [Header("Obj Ref")]
    public RectTransform bigHouseGroup;
    public RectTransform smallHouseGroup;
    public CanvasGroup smallHouseCanvasGroup;
    public CanvasGroup smallHouseCanvasGroup_spacial;

    public GameObject bigHouse_spacial;
    public GameObject smallHouse_spacial;

    public List<House_HouseBig> houseBigs = new();
    public List<House_HouseBig> houseBigs_spacial = new();
    public List<House_HouseSmall> houseSmalls = new();
    public List<House_HouseSmall> houseSmalls_spacial = new();

    public void SetUpHouses(List<HouseData> houseData)
    {
        houseBigs.Clear();
        houseSmalls.Clear();
        for (int i = 0; i < houseData.Count; i++)
        {
            var cloneBig = Instantiate(bigHouse_prefabs, bigHouseGroup);
            var bigHouse = cloneBig.GetComponent<House_HouseBig>();
            bigHouse.SetData(houseData[i]);
            bigHouse.Exit(true);
            houseBigs.Add(bigHouse);

            var cloneSmall = Instantiate(smallHouse_prefabs, smallHouseGroup);
            var smallHouse = cloneSmall.GetComponent<House_HouseSmall>();
            smallHouse.index = i;
            smallHouse.SetData(houseData[i]);
            houseSmalls.Add(smallHouse);
        }
        smallHouseCanvasGroup.alpha = 0;
    }

    public void SetUpHousesSpacial(List<HouseData> houseData)
    {

        bigHouse_spacial.SetActive(true);
        smallHouse_spacial.SetActive(true);

        houseBigs.Clear();
        houseSmalls.Clear();

        smallHouseCanvasGroup = smallHouseCanvasGroup_spacial;
        
        for (int i = 0; i < houseData.Count; i++)
        {
            var bigHouse = houseBigs_spacial[i];
            bigHouse.SetText(houseData[i].text);
            bigHouse.Exit(true);
            houseBigs.Add(bigHouse);

            var smallHouse = houseSmalls_spacial[i];
            smallHouse.index = i;
            smallHouse.SetText(houseData[i].text);
            //smallHouse.SetData(houseData[i]);
            houseSmalls.Add(smallHouse);
        }
        smallHouseCanvasGroup.alpha = 0;
    }

    public List<Droppable> GetDropArea()
    {
        var dropareas = new List<Droppable>();

        foreach (var item in houseSmalls)
        {
            dropareas.Add(item.droppable);
        }
        return dropareas;
    }

}
