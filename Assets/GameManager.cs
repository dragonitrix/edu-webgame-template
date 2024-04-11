using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TransitionsPlus;
using System;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void NextScene()
    {
        var animator = TransitionAnimator.Start(
            TransitionType.SeaWaves, // transition type
            color: Color.white,
            duration: 1f, // transition duration in seconds
            sceneNameToLoad: "sc_sandbox_test2"
        );

        void callback()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        animator.onTransitionEnd.AddListener(callback);
    }

}
