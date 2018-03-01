using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    bool pressingUp;
    bool pressingLeft;
    bool pressingDown;
    bool pressingRight;
    bool spit;

	void Update ()
    {
        float vAxis = Input.GetAxisRaw("Vertical");
        float hAxis = Input.GetAxisRaw("Horizontal");
        bool spit = Input.GetButtonDown("Spit");

        if (vAxis == 1)
        {
            pressingUp = true;
        }
        else pressingUp = false;

        if(hAxis == -1)
        {
            pressingLeft = true;
        }
        else pressingLeft = false;

        if(vAxis == -1)
        {
            pressingDown = true;
        }
        else pressingDown = false;

        if(hAxis == 1)
        {
            pressingRight = true;
        }
        else pressingRight = false;
    }

    public bool PressingUp { get { return pressingUp; } }
    public bool PressingLeft { get { return pressingLeft; } }
    public bool PressingDown { get { return pressingDown; } }
    public bool PressingRight { get { return pressingRight; } }
    public bool Spit { get { return spit; } }

}
