using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    public Vector2 jumpForce;

    Player player;
    Animator anim;

    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == ("Player") && player.falling)
        {
            anim.SetTrigger("Red");
            player.ShroomJump(jumpForce);
        }
    }
}
