using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drink : MonoBehaviour
{
    [SerializeField]
    bool falling;
    float fallingCounter = 4;
    public GameObject pukeObject;
    int charge;

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
                pukeObject.GetComponent<Puke>().Speed = Vector2.zero;
                Instantiate(pukeObject, new Vector3(this.transform.position.x, this.transform.position.y - 0.2f, 0), new Quaternion(0, 0, 0, 0));
                Destroy(this.gameObject);
            }
        }
	}

    public bool Falling { get { return falling; } set { falling = value; } }
    public int Charge { get { return charge; } set { charge = value; } }

}
