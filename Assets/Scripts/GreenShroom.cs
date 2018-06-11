using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenShroom : MonoBehaviour
{
    Player player;
    Animator anim;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == ("Player"))
        {
            anim.SetTrigger("Green");
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
