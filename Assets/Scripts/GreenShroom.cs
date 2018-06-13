using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenShroom : MonoBehaviour
{
    Player player;
    Animator anim;
    PlaySound playSound;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();
        playSound = GameObject.Find("GameMaster").GetComponent<PlaySound>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == ("Player"))
        {
            anim.SetTrigger("Green");
            playSound.Play(8, 1, 1);
            collision.gameObject.GetComponent<Player>().Die();
        }
        if(collision.tag == ("Puke"))
        {
            anim.SetTrigger("Puked");
            GetComponent<BoxCollider2D>().enabled = false;
            Destroy(collision.gameObject);
        }
    }
}
