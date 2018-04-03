using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drink : MonoBehaviour
{
    public bool falling;
    float fallingCounter = 4;
    public GameObject pukeObject;

	void Start ()
    {
		
	}

	void Update ()
    {
		if(falling)
        {
            fallingCounter -= Time.deltaTime;

            if(fallingCounter <= 0)
            {
                pukeObject.GetComponent<Puke>().speed = 0;
                Instantiate(pukeObject, new Vector3(this.transform.position.x, this.transform.position.y - 0.2f, 0), new Quaternion(0, 0, 0, 0));
                Destroy(this.gameObject);
            }
        }
	}
}
