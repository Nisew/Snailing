﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	[SerializeField]
    float speed = 1;
    [SerializeField]
    float patrolTime;
    float counter;
    bool dead;
    SpriteRenderer sprite;
    
    [SerializeField] GameObject drinkObject;

    void Start ()
    {
        counter = patrolTime;
        sprite = GetComponentInChildren<SpriteRenderer>();

    }
	
	void Update ()
    {
        if (dead)
        {

        }
        else Patrol();
    }

    void Patrol()
    {
        counter -= Time.deltaTime;

        if (counter <= 0)
        {
            speed *= -1;
            Flip();
            counter = patrolTime;
        }

        Vector2 provisionalPos = this.transform.position;

        provisionalPos.x += speed * Time.deltaTime;

        this.gameObject.transform.position = provisionalPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (!dead)
            {
                collision.gameObject.GetComponent<Player>().Die();
            }
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Puke"))
        {
            GetComponentInChildren<Animator>().SetTrigger("Die");
            Destroy(collision.gameObject);
            dead = true;
            GetComponent<Rigidbody2D>().gravityScale = 0.5f;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Instantiate(drinkObject, new Vector2(this.transform.position.x, this.transform.position.y - (GetComponent<CapsuleCollider2D>().size.y/2) + 0.02f), Quaternion.Euler(0, 0, 0));
            Destroy(this.gameObject);
        }
    }

    void Flip()
    {
        if(sprite.flipX)
        {
            sprite.flipX = false;
        }
        else
        {
            sprite.flipX = true;
        }
    }

}