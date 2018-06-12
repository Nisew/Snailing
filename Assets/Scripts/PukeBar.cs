using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PukeBar : MonoBehaviour
{
    float counter;
    bool doEasing;
    float startValue;
    float endValue;
    public bool powered;
    public GameObject parent;
    Color barColor;

    void Update()
    {
        if(doEasing)
        {
            counter += Time.deltaTime;

            this.gameObject.transform.localPosition = new Vector3(Easing.ExpoEaseOut(counter, startValue, endValue - startValue, 1f), this.transform.localPosition.y, 0);

            if(counter >= 1.2f)
            {
                this.gameObject.transform.localPosition = new Vector3(endValue, this.transform.localPosition.y, 0);
                counter = 0;
                doEasing = false;
            }
        }

        if (powered)
        {
            Destroy(this.gameObject);
            Destroy(parent);
        }
    }

    public void ChangePukeBar(int puke)
    {
        if(puke == 0)
        {
            startValue = this.gameObject.transform.localPosition.x;
            endValue = -40;
            doEasing = true;
        }
        if (puke == 1)
        {
            startValue = this.gameObject.transform.localPosition.x;
            endValue = -36;
            doEasing = true;
        }
        if (puke == 2)
        {
            startValue = this.gameObject.transform.localPosition.x;
            endValue = -32;
            doEasing = true;
        }
        if (puke == 3)
        {
            startValue = this.gameObject.transform.localPosition.x;
            endValue = -28;
            doEasing = true;
        }
        if (puke == 4)
        {
            startValue = this.gameObject.transform.localPosition.x;
            endValue = -24;
            doEasing = true;
        }
        if (puke == 5)
        {
            startValue = this.gameObject.transform.localPosition.x;
            endValue = -20;
            doEasing = true;
        }
        if (puke == 6)
        {
            startValue = this.gameObject.transform.localPosition.x;
            endValue = -16;
            doEasing = true;
        }
        if (puke == 7)
        {
            startValue = this.gameObject.transform.localPosition.x;
            endValue = -12;
            doEasing = true;
        }
        if (puke == 8)
        {
            startValue = this.gameObject.transform.localPosition.x;
            endValue = -8;
            doEasing = true;
        }
        if (puke == 9)
        {
            startValue = this.gameObject.transform.localPosition.x;
            endValue = -4;
            doEasing = true;
        }
        if (puke == 10)
        {
            startValue = this.gameObject.transform.localPosition.x;
            endValue = 0;
            doEasing = true;
        }
    }
}
