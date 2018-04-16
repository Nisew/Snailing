using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    bool transparent;
    bool fadeIn;
    bool fadeOut;

    float counter = 2;

    void Start ()
    {
		
	}
	
	void Update ()
    {
		if(fadeIn)
        {
            if(counter > 0)
            {
                counter -= Time.deltaTime;
            }
            else
            {
                fadeIn = false;
                counter = 2;
            }
        }
	}

    public void FadeIn()
    {
        fadeIn = true;
    }

    public void FadeOut()
    {
        fadeOut = true;
    }

    public bool Transparent { get { return transparent; } }
}
