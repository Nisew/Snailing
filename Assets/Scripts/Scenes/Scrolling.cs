using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrolling : MonoBehaviour
{
    public float speed;
       
	void Start ()
    {
		
	}
	
	void Update ()
    {
        Vector3 provisionalPos = this.transform.position;

        provisionalPos.x -= speed * Time.deltaTime;

        if(provisionalPos.x <= -20)
        {
            provisionalPos.x = 26;
        }

        this.transform.position = provisionalPos;
	}
}
