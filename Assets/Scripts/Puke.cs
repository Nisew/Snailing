using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puke : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] Vector2 speed;
    [SerializeField]
    public int pukeCharge;
    Player player;
    PlaySound playSound;
    [SerializeField] GameObject drinkObject;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = worldPoint - (Vector2)this.transform.localPosition;
        dir.Normalize();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playSound = GameObject.Find("GameMaster").GetComponent<PlaySound>();


        if (player.powered)
        {
            if (speed.x >= 10) speed.x = 10;
            if (speed.y >= 8) speed.y = 8;
            if (speed.x <= -10) speed.x = -10;
            if (speed.y <= -6) speed.y = -6;
            GetComponentInChildren<SpriteRenderer>().color = new Color(255, 25, 255, 255);
        }
        else
        {
            if(speed.x >= 2) speed.x = 2;
            if(speed.y >= 4) speed.y = 4;
            if(speed.x <= -2) speed.x = -2;
            if(speed.y <= -2.5f) speed.y = -2.5f;
        }

        rb.AddForce(speed, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D[] contacts = collision.contacts;

        if (!player.powered)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && contacts != null)
            {
                drinkObject.GetComponent<Drink>().Charge = 1;

                if (contacts[0].normal == Vector2.left)
                {
                    drinkObject.GetComponent<Drink>().Falling = false;
                    drinkObject.GetComponent<Drink>().Charge = pukeCharge;
                    Instantiate(drinkObject, new Vector2(this.transform.position.x + 0.05f, this.transform.position.y), Quaternion.Euler(0, 0, 90));
                }
                if (contacts[0].normal == Vector2.right)
                {
                    drinkObject.GetComponent<Drink>().Falling = false;
                    drinkObject.GetComponent<Drink>().Charge = pukeCharge;
                    Instantiate(drinkObject, new Vector2(this.transform.position.x - 0.05f, this.transform.position.y), Quaternion.Euler(0, 0, -90));
                }
                if (contacts[0].normal == Vector2.up)
                {
                    drinkObject.GetComponent<Drink>().Falling = false;
                    drinkObject.GetComponent<Drink>().Charge = pukeCharge;
                    Instantiate(drinkObject, new Vector2(this.transform.position.x, this.transform.position.y - 0.05f), Quaternion.Euler(0, 0, 0));
                }
                if (contacts[0].normal == Vector2.down)
                {
                    drinkObject.GetComponent<Drink>().Falling = true;
                    drinkObject.GetComponent<Drink>().Charge = pukeCharge;
                    Instantiate(drinkObject, new Vector2(this.transform.position.x, this.transform.position.y + 0.05f), Quaternion.Euler(0, 0, 180));
                }
                playSound.Play(5, 1, Random.Range(0.8f, 1.2f));
                Destroy(this.gameObject);
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Pukable") && contacts != null)
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
        else
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && contacts != null)
            {
                Destroy(this.gameObject);
            }
        }
    }        

    public Vector2 Speed { get { return speed; } set { speed = value; } }
}
