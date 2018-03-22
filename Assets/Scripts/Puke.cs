using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puke : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;
    public GameObject pukeOnGroundObject;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = worldPoint - (Vector2)this.transform.localPosition;
        dir.Normalize();

        rb.AddForce(dir * speed, ForceMode2D.Impulse);
    }

    void Update ()
    {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D[] contacts =  collision.contacts;

        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground") && contacts != null)
        {
            if(contacts[0].normal == Vector2.left)
            {
                Instantiate(pukeOnGroundObject, new Vector2(this.transform.position.x + 0.05f, this.transform.position.y), Quaternion.Euler(0,0,-90));
            }
            if(contacts[0].normal == Vector2.right)
            {
                Instantiate(pukeOnGroundObject, new Vector2(this.transform.position.x - 0.05f, this.transform.position.y), Quaternion.Euler(0, 0, 90));
            }
            if(contacts[0].normal == Vector2.up)
            {
                Instantiate(pukeOnGroundObject, new Vector2(this.transform.position.x, this.transform.position.y - 0.05f), Quaternion.Euler(0, 0, 0));
            }
            if(contacts[0].normal == Vector2.down)
            {
                Instantiate(pukeOnGroundObject, new Vector2(this.transform.position.x, this.transform.position.y + 0.05f), Quaternion.Euler(0, 0, 180));
            }
            Destroy(this.gameObject);
        }

    }

}
