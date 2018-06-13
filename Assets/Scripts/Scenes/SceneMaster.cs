using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMaster : MonoBehaviour
{
    Fade blackScreen;
    Fade logo;
    float counter = 5;
    bool oneTimeBool;

    public CurrentScene currentScene;
    public enum CurrentScene
    {
        Logo,
        Title,
        Gameplay,
        Ending,
    }

	void Start ()
    {
        if(currentScene == CurrentScene.Logo)
        {
            logo = GameObject.Find("Logo").GetComponent<Fade>();
        }
    }

	void Update ()
    {
        switch(currentScene)
        {
            case CurrentScene.Logo:
                Logo();
                break;
            case CurrentScene.Title:
                Title();
                break;
            case CurrentScene.Gameplay:
                Gameplay();
                break;
            case CurrentScene.Ending:
                Ending();
                break;
            default:
                break;
        }
    }

    #region UPDATE METHODS

    void Logo()
    {
        counter -= Time.deltaTime;

        if(counter <= 0)
        {
            oneTimeBool = false;
            TitleState();
            SceneManager.LoadScene(1);
        }
    }

    void Title()
    {
        counter -= Time.deltaTime;

        if (oneTimeBool == false)
        {
            blackScreen = GameObject.Find("BlackScreen").GetComponent<Fade>();
            blackScreen.FadeOut();
            oneTimeBool = true;
        }
    }

    void Gameplay()
    {
        counter -= Time.deltaTime;

        if (oneTimeBool == false)
        {
            blackScreen = GameObject.Find("BlackScreen").GetComponent<Fade>();
            blackScreen.FadeOut();
            oneTimeBool = true;
        }

    }

    void Ending()
    {
        counter -= Time.deltaTime;

        if(counter <= 0)
        {
            SceneManager.LoadScene(3);
        }
    }

    #endregion

    #region STATE METHODS

    void LogoState()
    {
        currentScene = CurrentScene.Logo;
    }

    void TitleState()
    {
        currentScene = CurrentScene.Title;
    }

    void GameplayState()
    {
        currentScene = CurrentScene.Gameplay;
    }

    void EndingState()
    {
        currentScene = CurrentScene.Ending;
    }

    #endregion

    #region SCENE CHANGE METHODS

    public void GameplayScreen()
    {
        SceneManager.LoadScene(2);
    }

    public void EndingScreen()
    {
        blackScreen = GameObject.Find("BlackScreen").GetComponent<Fade>();
        blackScreen.FadeIn();
        counter = 5;
        EndingState();
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    #endregion

}
