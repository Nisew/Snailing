using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puke : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;

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
}
