using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TransitionsPlus;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayBGM("bgm");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NextScene()
    {
        ExitSceneTransition(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        });
    }

    public void JumpToScene(string sceneNameToLoad)
    {
        ExitSceneTransition(() =>
        {
            SceneManager.LoadScene(sceneNameToLoad);
        });
    }

    public void ExitSceneTransition(UnityAction callback)
    {
        AudioManager.instance.PlaySound("ui_transition_1", AudioManager.Channel.SFX_2);
        var animator = TransitionAnimator.Start(
            TransitionType.SeaWaves, // transition type
            color: Color.white,
            duration: 1f // transition duration in seconds
        );
        animator.onTransitionEnd.AddListener(callback);
    }

}
