using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkSprite : MonoBehaviour
{
    public GameObject BigPuke;
    PlaySound playSound;

    void Start()
    {
        playSound = GameObject.Find("GameMaster").GetComponent<PlaySound>();
    }

    public void Drinked()
    {
        Destroy(transform.parent.gameObject);
    }

    public void Melt()
    {
        GetComponentInParent<Pukable>().MeltIntoDrink();
    }

    public void SpawnBigPuke()
    {
        BigPuke.SetActive(true);
    }

    public void Tss()
    {
        playSound.PlayPitch(0);
    }
}
