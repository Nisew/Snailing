  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pukable : MonoBehaviour
{
    [SerializeField] bool drinkable;
    [SerializeField] bool onRoof;
    bool puked;
    [SerializeField] float counter = 1;
    [SerializeField] int charge;
    [SerializeField] GameObject drink;

    Animator anim;

	void Start ()
    {
        anim = GetComponentInChildren<Animator>();

        if(onRoof)
        {
            GetComponent<Rigidbody2D>().gravityScale = 0;
        }
	}
	
	void Update ()
    {
		if(puked)
        {
            counter -= Time.deltaTime;

            if(counter <= 0)
            {
                MeltIntoDrink();
            }
        }
	}

    public void GetPuked()
    {
        anim.SetTrigger("Puked");
        if (onRoof)
        {
            GetComponent<Rigidbody2D>().gravityScale = 1;
        }
        puked = true;
    }

    void MeltIntoDrink()
    {
        if(drinkable)
        {
            SpawnDrink();
        }

        Destroy(this.gameObject);
    }

    void SpawnDrink()
    {
        drink.GetComponent<Puke>().pukeCharge = charge;
        drink.GetComponent<Puke>().Speed = Vector2.zero;
        Instantiate(drink, new Vector3(this.transform.position.x, this.transform.position.y, 0), new Quaternion(0, 0, 0, 0));
    }

}
