using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plutonium : MonoBehaviour
{
    Animator anim;
    public bool melted;
    public bool drinked;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Puke"))
        {
            GetComponentInChildren<Animator>().SetTrigger("Melt");
            melted = true;
            GetComponent<CircleCollider2D>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = true;
            Destroy(collision.gameObject);
        }

    }
}
