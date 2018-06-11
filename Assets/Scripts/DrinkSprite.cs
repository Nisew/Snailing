using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkSprite : MonoBehaviour
{
    public void Drinked()
    {
        Destroy(transform.parent.gameObject);
    }

    public void Melt()
    {
        GetComponentInParent<Pukable>().MeltIntoDrink();
    }
}
