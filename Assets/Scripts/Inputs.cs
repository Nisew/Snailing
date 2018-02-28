using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs : MonoBehaviour
{
    bool pressingUp;
    bool pressingLeft;
    bool pressingDown;
    bool pressingRight;

	void Update ()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            pressingUp = true;
        }
        else pressingUp = false;

        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            pressingLeft = true;
        }
        else pressingLeft = false;

        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            pressingDown = true;
        }
        else pressingDown = false;

        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            pressingRight = true;
        }
        else pressingRight = false;
    }

    public bool PressingUp { get { return pressingUp; } }
    public bool PressingLeft { get { return pressingLeft; } }
    public bool PressingDown { get { return pressingDown; } }
    public bool PressingRight { get { return pressingRight; } }

}
