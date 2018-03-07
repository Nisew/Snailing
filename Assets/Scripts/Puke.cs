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
        rb.AddForce((Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position).normalized * speed, ForceMode2D.Impulse);
    }

    void Update ()
    {
		
	}
}
