using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs : MonoBehaviour
{
    float AxisX;

	void Start ()
    {
		
	}

	void Update ()
    {
        AxisX = Input.GetAxis("horizontal");
	}
}
