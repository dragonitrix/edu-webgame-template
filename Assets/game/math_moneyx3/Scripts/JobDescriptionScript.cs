using UnityEngine;

[CreateAssetMenu(fileName = "JobDescription", menuName = "Math/ScriptableObjects/MoneyMoneyMoney", order = 1)]
public class JobDescriptionScript : ScriptableObject
{
    public string index;
    [SerializeField]
    private Sprite[] jobImages;
    public Sprite jobImage { get { return jobImages[jobImageIndex]; } }
    public Sprite jobDescription;

    int jobImageIndex = 0;

    public void randomizedImageIndex()
    {
        jobImageIndex = Random.Range(0, jobImages.Length);
    }
}
