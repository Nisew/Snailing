using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    Fade blackScreen;
    float logoCounter = 5;
    bool logoBool;

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
        blackScreen = GameObject.Find("BlackScreen").GetComponent<Fade>();
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
        if(logoBool == false)
        {
            blackScreen.FadeOut();
            logoBool = true;
        }

        logoCounter -= Time.deltaTime;

        if(logoCounter <= 0)
        {
            blackScreen.FadeIn();
        }
    }

    void Title()
    {

    }

    void Gameplay()
    {

    }

    void Ending()
    {

    }

    #endregion

}
