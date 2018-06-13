using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	[SerializeField]
    float speed = 1;
    [SerializeField]
    float patrolTime;
    [SerializeField]
    public bool followPlayer;
    [SerializeField]
    public Transform enemyTarget;
    float counter;
    public bool dead;
    SpriteRenderer sprite;
    PlaySound playSound;
    
    [SerializeField] GameObject drinkObject;

    void Start ()
    {
        counter = patrolTime;
        sprite = GetComponentInChildren<SpriteRenderer>();
        playSound = GameObject.Find("GameMaster").GetComponent<PlaySound>();
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
        if(!followPlayer)
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
        else
        {
            float smooth = 1 * Time.deltaTime;

            this.transform.position = Vector3.MoveTowards(this.transform.position, enemyTarget.position, smooth);

            if(transform.position.x > enemyTarget.position.x && sprite.flipX == true)
            {
                Flip();
            }
            if (transform.position.x < enemyTarget.position.x && sprite.flipX == false)
            {
                Flip();
            }
        }
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

            if(!dead)
            {
                Destroy(collision.gameObject);
            }

            dead = true;
            GetComponent<Rigidbody2D>().gravityScale = 0.7f;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            drinkObject.GetComponent<Drink>().Charge = 2;
            playSound.Play(5, 1, Random.Range(0.8f, 1.2f));
            Instantiate(drinkObject, new Vector2(this.transform.position.x, this.transform.position.y - (GetComponent<CapsuleCollider2D>().size.y/2) + 0.02f), Quaternion.Euler(0, 0, 0));
            Destroy(this.gameObject);
        }
    }

    public void Die()
    {
        dead = true;
        GetComponentInChildren<Animator>().SetTrigger("Die");
        GetComponent<Rigidbody2D>().gravityScale = 0.7f;
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
