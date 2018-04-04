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

	void Start ()
    {
		
	}
	
	void Update ()
    {
		
	}

    public void GetPuked()
    {
        lives--;
        Debug.Log(lives);

        if(lives <= 0)
        {
            MeltIntoDrink();
        }
    }

    void MeltIntoDrink()
    {
        Debug.Log("MELTED");

        if(drinkable)
        {
            SpawnDrink();
        }

        Destroy(this.gameObject);
    }

    void SpawnDrink()
    {
        drink.GetComponent<Drink>().Charge = pukeCharge;

        if(onRoof)
        {
            drink.GetComponent<Drink>().Falling = true;
        }

        Instantiate(drink, new Vector3(this.transform.position.x, this.transform.position.y, 0), new Quaternion(0, 0, 0, 0));
    }

}
