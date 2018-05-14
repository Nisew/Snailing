  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pukable : MonoBehaviour
{
    [SerializeField] bool drinkable;
    [SerializeField] bool onRoof;
    [SerializeField] int lives;
    [SerializeField] int pukeCharge;
    [SerializeField] GameObject drink;

    Animator anim;

	void Start ()
    {
        anim = GetComponentInChildren<Animator>();
	}
	
	void Update ()
    {
		
	}

    public void GetPuked()
    {
        lives--;
        anim.SetTrigger("Puked");

        if(lives <= 0)
        {
            MeltIntoDrink();
        }
    }

    void MeltIntoDrink()
    {
        if(drinkable)
        {
            drink.GetComponent<Drink>().Falling = true;
            drink.GetComponent<Drink>().FallingTime = 0;
            SpawnDrink();
        }

        Destroy(this.gameObject);
    }

    void SpawnDrink()
    {
        drink.GetComponent<Drink>().Charge = pukeCharge;

        Instantiate(drink, new Vector3(this.transform.position.x, this.transform.position.y, 0), new Quaternion(0, 0, 0, 0));
    }

}
