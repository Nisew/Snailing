using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    int lives = 5;
    Transform target;
    public Transform player;

    public GameObject flyPrefab;

    public bool active;
    float spawnTimer;
    public Transform[] waveSpawnPoints;
    public Transform[] bossMovementPoints;
    SpriteRenderer sprite;
    Animator anim;

    public BossState bossState;
    public enum BossState
    {
        Targeting,
        Moving,
    }

    void Start ()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }
	
	void Update ()
    {
        if(active)
        {
            switch (bossState)
            {
                case BossState.Targeting:
                    Targeting();
                    break;
                case BossState.Moving:
                    Moving();
                    break;
                default:
                    break;
            }

            spawnTimer += Time.deltaTime;

            if(spawnTimer >= Random.Range(7, 9))
            {
                spawnTimer = 0;

                FlyWave(Random.Range(4, 6));
            }

        }
    }

    void Targeting()
    {
        target = bossMovementPoints[Random.Range(0, bossMovementPoints.Length)];

        if(Vector3.Distance(this.transform.position, target.position) > 1)
        {
            bossState = BossState.Moving;
        }
    }

    void Moving()
    {
        float smooth = 1 * Time.deltaTime;

        this.transform.position = Vector3.MoveTowards(this.transform.position, target.position, smooth);

        if(Vector3.Distance(this.transform.position, target.position) <= 0.7f)
        {
            bossState = BossState.Targeting;
        }

        if (transform.position.x > target.position.x && sprite.flipX == true)
        {
            Flip();
        }
        if (transform.position.x < target.position.x && sprite.flipX == false)
        {
            Flip();
        }
    }

    public void FlyWave(int num)
    {
        for(int i = 0; i < num; i++)
        {
            Transform randomPoint = waveSpawnPoints[Random.Range(0, waveSpawnPoints.Length)];

            flyPrefab.GetComponent<Enemy>().followPlayer = true;
            flyPrefab.GetComponent<Enemy>().enemyTarget = player;

            Instantiate(flyPrefab, new Vector3(randomPoint.position.x, randomPoint.position.y), Quaternion.Euler(0, 0, 0));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Puke"))
        {
            RecieveDamage();
            Destroy(collision.gameObject);
        }
    }

    void RecieveDamage()
    {
        lives--;

        if(lives > 0)
        {
            anim.SetTrigger("Hit");
        }

        if(lives <= 0)
        {
            active = false;
            anim.SetTrigger("Die");

            GameObject[] flies = GameObject.FindGameObjectsWithTag("Fly");

            foreach(GameObject fly in flies)
            {
                fly.GetComponentInChildren<Enemy>().Die();
            }
        }

    }

    void Flip()
    {
        if (sprite.flipX)
        {
            sprite.flipX = false;
        }
        else
        {
            sprite.flipX = true;
        }
    }
}
