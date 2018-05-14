﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puke : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] Vector2 speed;
    [SerializeField] GameObject drinkObject;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = worldPoint - (Vector2)this.transform.localPosition;
        dir.Normalize();

        rb.AddForce(speed, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D[] contacts =  collision.contacts;

        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground") && contacts != null)
        {
            drinkObject.GetComponent<Drink>().Charge = 1;

            if (contacts[0].normal == Vector2.left)
            {
                drinkObject.GetComponent<Drink>().Falling = false;
                Instantiate(drinkObject, new Vector2(this.transform.position.x + 0.05f, this.transform.position.y), Quaternion.Euler(0,0,-90));
            }
            if(contacts[0].normal == Vector2.right)
            {
                drinkObject.GetComponent<Drink>().Falling = false;
                Instantiate(drinkObject, new Vector2(this.transform.position.x - 0.05f, this.transform.position.y), Quaternion.Euler(0, 0, 90));
            }
            if(contacts[0].normal == Vector2.up)
            {
                drinkObject.GetComponent<Drink>().Falling = false;
                Instantiate(drinkObject, new Vector2(this.transform.position.x, this.transform.position.y - 0.05f), Quaternion.Euler(0, 0, 0));
            }
            if(contacts[0].normal == Vector2.down)
            {
                drinkObject.GetComponent<Drink>().Falling = true;
                Instantiate(drinkObject, new Vector2(this.transform.position.x, this.transform.position.y + 0.05f), Quaternion.Euler(0, 0, 180));
            }
            Destroy(this.gameObject);
        }

        if(collision.gameObject.layer == LayerMask.NameToLayer("Pukable") && contacts != null)
        {
            contacts[0].collider.GetComponent<Pukable>().GetPuked();
            Destroy(this.gameObject);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle") && contacts != null)
        {
            contacts[0].collider.GetComponent<Pukable>().GetPuked();
            Destroy(this.gameObject);
        }

    }

    public Vector2 Speed { get { return speed; } set { speed = value; } }
}
