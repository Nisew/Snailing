using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField] bool startOpaque;
    [SerializeField] float fadeTime;

    float fadeCounter;
    float alpha;
    bool transparent;
    bool fadeIn;
    bool fadeOut;
    Image sprite;

    void Start ()
    {
        sprite = GetComponent<Image>();
        fadeCounter = fadeTime;

        if (startOpaque)
        {
            transparent = false;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
        }
        else
        {
            transparent = true;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0);
        }
    }
	
	void Update ()
    {
		if(fadeIn)
        {
            if(fadeCounter > 0)
            {
                fadeCounter -= Time.deltaTime;
                alpha = Easing.ExpoEaseOut(fadeCounter, 1, -1, fadeTime);
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
            }
            else
            {
                fadeIn = false;
                fadeCounter = fadeTime;
                transparent = false;
            }
        }

        if(fadeOut)
        {
            if (fadeCounter > 0)
            {
                fadeCounter -= Time.deltaTime;
                alpha = Easing.ExpoEaseOut(fadeCounter, 0, 1, fadeTime);
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
            }
            else
            {
                fadeOut = false;
                fadeCounter = fadeTime;
                transparent = true;
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
