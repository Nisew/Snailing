using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTile : MonoBehaviour
{
    Player player;
    PlaySound playSound;

    void Awake()
    {
        player = GetComponentInParent<Player>();
        playSound = GameObject.Find("GameMaster").GetComponent<PlaySound>();
    }

    public void Puke()
    {
        player.Puke();
    }

    public void Drink()
    {
        player.TryDrink();
    }

    public void Rotate()
    {
        player.Rotate();
    }
}
