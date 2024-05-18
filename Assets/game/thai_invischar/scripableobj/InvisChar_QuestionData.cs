using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "invisChar_q0", menuName = "invisChar/QuestionData")]
public class InvisChar_QuestionData : ScriptableObject
{
    public string fulltext = "";
    public string hintText = "";
    public InvisChar_PartData[] data;

    public string GetCompleteName()
    {
        string name = fulltext;

        var filteredData = data.Where(x => x.type == InvisChar_PartData.TYPE.DROP).ToList();

        for (int i = 0; i < filteredData.Count; i++)
        {
            var d = filteredData[i];
            name = name.Replace(i.ToString(), d.text);
        }
        return name;
    }

}

[System.Serializable]
public class InvisChar_PartData
{
    public enum TYPE
    {
        DROP,
        FIX
    }
    public TYPE type;
    public string text;
}