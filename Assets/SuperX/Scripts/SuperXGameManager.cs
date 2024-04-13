using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuperXGameManager : MonoBehaviour
{
    int[] Super7Member = { 4, 5, 6, 7 };
    int[] Super7Roulette = { 0, 1, 2, 3 };

    int[] Super8Member = { 4, 5, 6, 7, 8 };
    int[] Super8Roulette = { 0, 1, 2, 3, 4 };

    int[] Super9Member = { 5, 6, 7, 8, 9 };
    int[] Super9Roulette = { 0, 1, 2, 3, 4 };
    
    int[] Super10Member = { 0, 1, 2, 3, 4 };
    int[] Super10Roulette = { 6, 7, 8, 9, 10 };

    public static SuperXGameManager Instance;
    public List<ButtonInGrid> buttonInGrids = new List<ButtonInGrid>();
    public List<GameObject> helperBoards = new List<GameObject>();
    public TextMeshProUGUI gameTitle;
    public TextMeshProUGUI rouletteResultNumber;
    public TextMeshProUGUI correctionText;

    public List<int> Roulette;
    GameObject currentHelperBoard;
    int currentTarget = 7;
    int correctAnswer;

    [Header("Testing Parameter")]
    public int textInitX = 7;

    private void Awake()
    {
        if (!Instance) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        InitiateGrid(textInitX);
    }

    void InitiateGrid(int x)
    {
        int[] gridMember;
        int[] rouletteMember;
        string SuperX = "Super7";
        currentTarget = x;

        for (int i = 0; i < helperBoards.Count; i++)
        {
            helperBoards[i].SetActive(false);
        }

        switch (x)
        {
            default:
            case 7:
                gridMember = Super7Member;
                rouletteMember = Super7Roulette;
                helperBoards[0].SetActive(true);
                currentHelperBoard = helperBoards[0];
                SuperX = "Super7";
                break;
            case 8:
                gridMember = Super8Member;
                rouletteMember = Super8Roulette;
                helperBoards[1].SetActive(true);
                currentHelperBoard = helperBoards[1];
                SuperX = "Super8";
                break;
            case 9:
                gridMember = Super9Member;
                rouletteMember = Super9Roulette;
                helperBoards[2].SetActive(true);
                currentHelperBoard = helperBoards[2];
                SuperX = "Super9";
                break;
            case 10:
                gridMember = Super10Member;
                rouletteMember = Super10Roulette;
                helperBoards[3].SetActive(true);
                currentHelperBoard = helperBoards[3];
                SuperX = "Super10";
                break;
        }
        gameTitle.text = SuperX;

        List<int> memberForInit = new List<int>();

        for (int i = 0; i < 40 / gridMember.Length; i++)
        {
            for (int j = 0; j < gridMember.Length; j++)
            {
                memberForInit.Add(gridMember[j]);
            }
        }

        for (int i = 0; i < buttonInGrids.Count; i++)
        {
            int index =  Random.Range(0, memberForInit.Count);
            buttonInGrids[i].initiateButton(memberForInit[index]);
            memberForInit.RemoveAt(index);
        }

        Roulette = new List<int>();

        for (int i = 0; i < rouletteMember.Length; i++)
        {
            Roulette.Add(rouletteMember[i]);
        }
    }

    public void randomRoulette()
    {
        int randomValue = Roulette[Random.Range(0, Roulette.Count)];
        rouletteResultNumber.text = randomValue.ToString();
        helperButtonActivation(randomValue);
        correctAnswer = currentTarget - randomValue;
        correctionText.text = "";
    }

    public void helperButtonActivation(int value)
    {
        for (int i = 0; i < currentHelperBoard.transform.childCount; i++)
        {
            if (i < value)
            {
                currentHelperBoard.transform.GetChild(i).GetComponent<HelperButton>().isActivated = true;
            }
            else
            {
                currentHelperBoard.transform.GetChild(i).GetComponent<HelperButton>().isActivated = false;
            }
        }
    }

    public void submitAnswer(ButtonInGrid button)
    {
        if(button.buttonNumber == correctAnswer)
        {
            correctionText.text = "Correct!";
            correctionText.color = Color.green;
        }
        else
        {
            correctionText.text = "Incorrect!";
            correctionText.color = Color.red;
        }
    }

    //test
    public void ReInit()
    {
        InitiateGrid(textInitX);
    }
}
