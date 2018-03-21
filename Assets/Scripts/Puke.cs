using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puke : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;
    public GameObject pukeOnGroundObject;
    RaycastHit2D rayCastHit;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("collision");

            rayCastHit = Physics2D.Raycast(this.transform.position, Vector2.down, 50f);

            if(rayCastHit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Instantiate(pukeOnGroundObject, this.transform.position, new Quaternion(0, 0, 0, 0));
            }
            else if(rayCastHit.collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
            {
                rayCastHit = Physics2D.Raycast(this.transform.position, Vector2.down, 1f);
            }

            Destroy(this.gameObject);            
        }        
    }
}
