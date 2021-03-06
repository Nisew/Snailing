﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drink : MonoBehaviour
{
    [SerializeField]
    bool falling;
    [SerializeField]
    float fallingTime = 4;
    public GameObject pukeObject;
    [SerializeField]
    int charge;

	void Update ()
    {
        if(falling)
        {
            fallingTime -= Time.deltaTime;

            if(fallingTime <= 0)
            {
                pukeObject.GetComponent<Puke>().Speed = Vector2.zero;
                Instantiate(pukeObject, new Vector3(this.transform.position.x, this.transform.position.y - 0.2f, 0), new Quaternion(0, 0, 0, 0));
                Destroy(this.gameObject);
            }
        }
	}

    public int Charge { get { return charge; } set { charge = value; } }
    public bool Falling { get { return falling; } set { falling = value; } }
    public float FallingTime { get { return fallingTime; } set { fallingTime = value; } }
}
