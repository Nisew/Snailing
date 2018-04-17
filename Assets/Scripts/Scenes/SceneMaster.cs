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

    CurrentScene currentScene;
    enum CurrentScene
    {
        Logo,
        Title,
        Gameplay,
        Ending,
    }

	void Start ()
    {
        DontDestroyOnLoad(this.gameObject);
        logo = GameObject.Find("Logo").GetComponent<Fade>();
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

        if(oneTimeBool == false && counter < 4)
        {
            logo.FadeIn();
            oneTimeBool = true;
        }

        if(counter <= 0 && !logo.Transparent)
        {
            logo.FadeOut();
        }

        if(counter <= 0 && logo.Transparent)
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

    }

    void Ending()
    {

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

}
