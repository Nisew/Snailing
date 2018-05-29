using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    Player player;
    bool pressingUp;
    bool pressingLeft;
    bool pressingDown;
    bool pressingRight;
    bool spit;
    bool pressingSpace;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

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

        if(Input.GetMouseButtonDown(0))
        {
            if(!player.falling)
            {
                player.SpitState();
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            pressingSpace = true;
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            pressingSpace = false;
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            player.DrinkState();
        }
    }

    public bool PressingUp { get { return pressingUp; } }
    public bool PressingLeft { get { return pressingLeft; } }
    public bool PressingDown { get { return pressingDown; } }
    public bool PressingRight { get { return pressingRight; } }
    public bool PressingSpace { get { return pressingSpace; } }
    public bool Spit { get { return spit; } }

}
