﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTile : MonoBehaviour
{
    Player player;

    void Awake()
    {
        player = GetComponentInParent<Player>();
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
