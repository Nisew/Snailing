using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Game elements")]
    InputManager inputScript;
    [SerializeField] GameObject puke;
    PukePath pukePath;
    GameObject tile;
    Rigidbody2D rb;

    [Header("Ground detection")]
    [SerializeField] Vector2 topOrigin;
    [SerializeField] float topDistance;

    [SerializeField] Vector2 leftUpOrigin;
    [SerializeField] Vector2 leftDownOrigin;
    [SerializeField] float leftDistance;

    [SerializeField] Vector2 bottomUpOrigin;
    [SerializeField] Vector2 bottomDownOrigin;
    [SerializeField] float bottomDistance;

    [SerializeField] Vector2 rightUpOrigin;
    [SerializeField] Vector2 rightDownOrigin;
    [SerializeField] float rightDistance;

    public bool topWalled;
    public bool leftUpWalled;
    public bool leftDownWalled;
    public bool rightUpWalled;
    public bool rightDownWalled;
    public bool bottomUpWalled;
    public bool bottomDownWalled;

    public bool snailingInLeftWall;
    public bool snailingInRightWall;

    RaycastHit2D[] rayHit = new RaycastHit2D[1];
    int numHits;
    [SerializeField] ContactFilter2D contactFilter;
    [SerializeField] ContactFilter2D drinkFilter;

    [Header("Player properties")]
    float speed = 1;
    bool falling;

    public Vector2 pukePointTopLeft;
    public Vector2 pukePointTopRight;
    public Vector2 pukePointLeftUp;
    public Vector2 pukePointLeftDown;
    public Vector2 pukePointRightUp;
    public Vector2 pukePointRightDown;

    int numPukes;
    public float pukeCharge;
    float maxPukeChare = 10;
    bool canDrink;
    bool drinking;
    float drinkDistance = 0.01f;
    float idleTime;
    bool facingRight = true;

    Vector2 goingToPos;
    bool goingUpLeft;
    bool goingDownLeft;
    bool goingUpRight;
    bool goingDownRight;
    bool arrived;

    public PlayerState currentPlayerState;
    public enum PlayerState
    {
        Idle,
        Move,
        Spit,
        Drink,
        Dead,
        Freeze,
    }

	void Start ()
    {
        inputScript = GameObject.FindGameObjectWithTag("Input").GetComponent<InputManager>();
        pukePath = GetComponent<PukePath>();
        rb = GetComponent<Rigidbody2D>();
        tile = this.transform.GetChild(0).gameObject;
        IdleState(0f);
	}

    void FixedUpdate()
    {
        TopWallDetection();
        LeftWallDetection();
        BottomWallDetection();
        RightWallDetection();
    }

    void Update ()
    {
        switch(currentPlayerState)
        {
            case PlayerState.Idle:
                Idle();
                break;
            case PlayerState.Move:
                Move();
                break;
            case PlayerState.Spit:
                Spit();
                break;
            case PlayerState.Drink:
                Drink();
                break;
            case PlayerState.Dead:
                Dead();
                break;
            case PlayerState.Freeze:
                Freeze();
                break;
            default:
                break;
        }
    }

    #region UPDATE METHODS

    void Idle()
    {
        idleTime -= Time.deltaTime;

        if (idleTime < 0)
        {
            MoveState();
        }
    }

    void Move()
    {
        if(!snailingInLeftWall && !snailingInRightWall && (bottomUpWalled || bottomDownWalled))
        {
            if(inputScript.PressingLeft && !leftUpWalled && bottomUpWalled) //MOVE LEFT
            {
                if (facingRight) Flip();
                Vector2 provisionalPos = this.transform.position;

                provisionalPos.x -= speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingLeft && leftUpWalled) //SNAIL LEFT WALL
            {
                if (facingRight) Flip();
                tile.transform.localRotation = Quaternion.Euler(0, 0, -90);
                snailingInLeftWall = true;
                rb.gravityScale = 0;
                IdleState(0.5f);
            }

            if(inputScript.PressingLeft && !bottomUpWalled) //GOES DOWN TO RIGHT WALL
            {
                goingToPos = new Vector2(this.transform.position.x - 0.75f, this.transform.position.y - 1f);
                tile.transform.localRotation = Quaternion.Euler(0, 0, 90);
                goingDownLeft = true;
                snailingInRightWall = true;
                rb.gravityScale = 0;
                FreezeState();
            }

            if(inputScript.PressingRight && !rightUpWalled) //MOVE RIGHT
            {
                if (!facingRight) Flip();

                Vector2 provisionalPos = this.transform.position;

                provisionalPos.x += speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingRight && rightUpWalled) //SNAIL RIGHT WALL
            {
                if (!facingRight) Flip();
                tile.transform.localRotation = Quaternion.Euler(0, 0, 90);
                snailingInRightWall = true;
                rb.gravityScale = 0;
                IdleState(0.5f);
            }

            if (inputScript.PressingRight && !bottomDownWalled) //GOES DOWN TO LEFT WALL
            {
                goingToPos = new Vector2(this.transform.position.x + 0.75f, this.transform.position.y - 1f);
                tile.transform.localRotation = Quaternion.Euler(0, 0, -90);
                goingDownRight = true;
                snailingInLeftWall = true;
                rb.gravityScale = 0;
                FreezeState();
            }
        }

        if(snailingInLeftWall)
        {
            if(inputScript.PressingUp && leftUpWalled && !topWalled)
            {
                if (facingRight) Flip();

                Vector2 provisionalPos = this.transform.position;

                provisionalPos.y += speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingUp && !leftUpWalled) //GOES UP LEFT WALL
            {
                goingToPos = new Vector2(this.transform.position.x - 1, this.transform.position.y + 0.5f);
                tile.transform.localRotation = Quaternion.Euler(0, 0, 0);
                goingUpLeft = true;
                snailingInLeftWall = false;
                FreezeState();
            }

            if(inputScript.PressingDown && !bottomDownWalled)
            {
                if (!facingRight) Flip();

                Vector2 provisionalPos = this.transform.position;

                provisionalPos.y -= speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingDown && bottomDownWalled)
            {
                if (!facingRight) Flip();
                snailingInLeftWall = false;
                tile.transform.localRotation = Quaternion.Euler(0, 0, 0);
                rb.gravityScale = 1;
                IdleState(0.5f);
            }

            if (inputScript.PressingSpace)
            {
                rb.gravityScale = 1;
                rb.AddForce(new Vector2(1, 0), ForceMode2D.Impulse);
                tile.transform.localRotation = Quaternion.Euler(0, 0, 0);
                snailingInLeftWall = false;
                falling = true;
                FreezeState();
            }
        }

        if(snailingInRightWall)
        {
            if(inputScript.PressingUp && rightUpWalled && !topWalled)
            {
                if (!facingRight) Flip();

                Vector2 provisionalPos = this.transform.position;

                provisionalPos.y += speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingUp && !rightUpWalled) //GOES UP RIGHT WALL
            {
                goingToPos = new Vector2(this.transform.position.x + 1, this.transform.position.y + 0.5f);
                tile.transform.localRotation = Quaternion.Euler(0, 0, 0);
                goingUpRight = true;
                snailingInRightWall = false;
                FreezeState();
            }

            if(inputScript.PressingDown && !bottomDownWalled)
            {
                if (facingRight) Flip();

                Vector2 provisionalPos = this.transform.position;

                provisionalPos.y -= speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingDown && bottomDownWalled)
            {
                if (facingRight) Flip();
                snailingInRightWall = false;
                tile.transform.localRotation = Quaternion.Euler(0, 0, 0);
                rb.gravityScale = 1;
                IdleState(0.5f);
            }

            if (inputScript.PressingSpace)
            {
                rb.gravityScale = 1;
                rb.AddForce(new Vector2(-1, 0), ForceMode2D.Impulse);
                tile.transform.localRotation = Quaternion.Euler(0, 0, 0);
                snailingInRightWall = false;
                falling = true;
                FreezeState();
            }
        }
    }

    void Spit()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if(!snailingInLeftWall && !snailingInRightWall)
        {
            if (worldPoint.x > transform.position.x && !facingRight)
            {
                Flip();
            }
            if (worldPoint.x < transform.position.x && facingRight)
            {
                Flip();
            }
        }
        if(snailingInLeftWall)
        {
            if(worldPoint.y < transform.position.y + 0.5f && !facingRight)
            {
                Flip();
            }
            if (worldPoint.y > transform.position.y + 0.5f && facingRight)
            {
                Flip();
            }
        }
        if (snailingInRightWall)
        {
            if (worldPoint.y > transform.position.y + 0.5f && !facingRight)
            {
                Flip();
            }
            if (worldPoint.y < transform.position.y + 0.5f && facingRight)
            {
                Flip();
            }
        }

        pukePath.DrawProjectileTrajectory();

        if (Input.GetMouseButtonUp(0))
        {
            puke.GetComponent<Puke>().Speed = pukePath.GetForceFrom(GetPukePoint(), Camera.main.ScreenToWorldPoint(Input.mousePosition));
            pukeCharge = 0;

            Instantiate(puke, GetPukePoint(), new Quaternion(0, 0, 0, 0));

            IdleState(0.5f);
        }
    }

    void Drink()
    {
        TryDrink();
        IdleState(0);
    }

    void Dead()
    {

    }

    void Freeze()
    {
        if(goingUpLeft) GoToUpLeft();

        if(goingUpRight) GoToUpRight();

        if(goingDownRight) GoToDownRight();

        if(goingDownLeft) GoToDownLeft();

        if(falling)
        {
           if(bottomUpWalled || bottomDownWalled)
           {
                falling = false;
                IdleState(0.5f);
           }
        }
    }

    #endregion

    #region WALL TRANSITION METHODS

    void GoToUpLeft()
    {
        if(this.transform.position.y <= goingToPos.y)
        {
            Vector2 provisionalPos = this.transform.position;

            provisionalPos.y += Time.deltaTime*3;

            this.gameObject.transform.position = provisionalPos;

            if(this.transform.position.y >= goingToPos.y && this.transform.position.x > goingToPos.x)
            {
                provisionalPos.y = goingToPos.y;

                provisionalPos.x -= Time.deltaTime * 3;

                this.gameObject.transform.position = provisionalPos;

                if(this.transform.position.x <= goingToPos.x)
                {
                    this.gameObject.transform.position = goingToPos;
                    goingUpLeft = false;
                    rb.gravityScale = 1;
                    IdleState(0.5f);
                }
            }
        }
    }

    void GoToUpRight()
    {
        if(this.transform.position.y <= goingToPos.y)
        {
            Vector2 provisionalPos = this.transform.position;

            provisionalPos.y += Time.deltaTime * 3;

            this.gameObject.transform.position = provisionalPos;

            if(this.transform.position.y >= goingToPos.y && this.transform.position.x < goingToPos.x)
            {
                provisionalPos.y = goingToPos.y;

                provisionalPos.x += Time.deltaTime * 3;

                this.gameObject.transform.position = provisionalPos;

                if(this.transform.position.x >= goingToPos.x)
                {
                    this.gameObject.transform.position = goingToPos;
                    rb.gravityScale = 1;
                    goingUpRight = false;
                    IdleState(0.5f);
                }
            }
        }
    }

    void GoToDownRight()
    {
        Vector2 provisionalPos = this.transform.position;

        if(this.transform.position.x <= goingToPos.x && !arrived)
        {
            provisionalPos.x += Time.deltaTime * 3;

            this.gameObject.transform.position = provisionalPos;

            if(this.transform.position.x >= goingToPos.x && this.transform.position.y > goingToPos.y)
            {
                provisionalPos.x = goingToPos.x;

                provisionalPos.y -= Time.deltaTime * 3;

                this.gameObject.transform.position = provisionalPos;

                if(this.transform.position.y <= goingToPos.y)
                {
                    arrived = true;
                }
            }
        }

        if (arrived && !leftUpWalled)
        {
            provisionalPos.x -= Time.deltaTime * 3;

            this.gameObject.transform.position = provisionalPos;
        }
        else if (arrived && leftUpWalled)
        {
            goingDownRight = false;
            arrived = false;
            IdleState(0.5f);
        }
    }

    void GoToDownLeft()
    {
        Vector2 provisionalPos = this.transform.position;

        if (this.transform.position.x >= goingToPos.x && !arrived)
        {
            provisionalPos.x -= Time.deltaTime * 3;

            this.gameObject.transform.position = provisionalPos;

            if (this.transform.position.x <= goingToPos.x && this.transform.position.y > goingToPos.y)
            {
                provisionalPos.x = goingToPos.x;

                provisionalPos.y -= Time.deltaTime * 3;

                this.gameObject.transform.position = provisionalPos;

                if (this.transform.position.y <= goingToPos.y)
                {
                    arrived = true;
                }
            }
        }

        if (arrived && !rightUpWalled)
        {
            provisionalPos.x += Time.deltaTime * 3;

            this.gameObject.transform.position = provisionalPos;
        }
        else if (arrived && rightUpWalled)
        {
            goingDownLeft = false;
            arrived = false;
            IdleState(0.5f);
        }
    }

    #endregion

    #region WALL DETECTION METHODS

    void TopWallDetection()
    {
        topWalled = false;

        numHits = Physics2D.Raycast(new Vector2(this.transform.position.x + topOrigin.x, this.transform.position.y + topOrigin.y), new Vector2(0, 1), contactFilter, rayHit, topDistance);

        if(numHits > 0)
        {
            topWalled = true;
        }        
    }

    void LeftWallDetection()
    {
        leftUpWalled = false;
        leftDownWalled = false;

        numHits = Physics2D.Raycast(new Vector2(this.transform.position.x + leftUpOrigin.x, this.transform.position.y + leftUpOrigin.y), new Vector2(-1, 0), contactFilter, rayHit, leftDistance);

        if(numHits > 0)
        {
            leftUpWalled = true;
        }

        numHits = Physics2D.Raycast(new Vector2(this.transform.position.x + leftDownOrigin.x, this.transform.position.y + leftDownOrigin.y), new Vector2(-1, 0), contactFilter, rayHit, leftDistance);

        if(numHits > 0)
        {
            leftDownWalled = true;
        }
    }

    void BottomWallDetection()
    {
        bottomUpWalled = false;
        bottomDownWalled = false;

        numHits = Physics2D.Raycast(new Vector2(this.transform.position.x + bottomUpOrigin.x, this.transform.position.y + bottomUpOrigin.y), new Vector2(0, -1), contactFilter, rayHit, bottomDistance);
        
        if(numHits > 0)
        {
            bottomUpWalled = true;
        }

        numHits = Physics2D.Raycast(new Vector2(this.transform.position.x + bottomDownOrigin.x, this.transform.position.y + bottomDownOrigin.y), new Vector2(0, -1), contactFilter, rayHit, bottomDistance);

        if(numHits > 0)
        {
            bottomDownWalled = true;
        }
    }

    void RightWallDetection()
    {
        rightUpWalled = false;
        rightDownWalled = false;

        numHits = Physics2D.Raycast(new Vector2(this.transform.position.x + rightUpOrigin.x, this.transform.position.y + rightUpOrigin.y), new Vector2(1, 0), contactFilter, rayHit, rightDistance);

        if(numHits > 0)
        {
            rightUpWalled = true;
        }

        numHits = Physics2D.Raycast(new Vector2(this.transform.position.x + rightDownOrigin.x, this.transform.position.y + rightDownOrigin.y), new Vector2(1, 0), contactFilter, rayHit, rightDistance);

        if(numHits > 0)
        {
            rightDownWalled = true;
        }
    }

    #endregion

    #region MECHANICS METHODS

    void TryDrink()
    {
        RaycastHit2D[] drinkResults = new RaycastHit2D[1];

        int drinks = rb.Cast(new Vector2(1.0f, 0f), drinkFilter, drinkResults, drinkDistance);

        if(drinks > 0)
        {
            if(!snailingInLeftWall && !snailingInRightWall)
            {
                float drinkPosX = drinkResults[0].collider.transform.position.x;

                if ((drinkPosX > this.transform.position.x) && !facingRight)
                {
                    Flip();
                }
                else if ((drinkPosX < this.transform.position.x) && facingRight)
                {
                    Flip();
                }
            }

            if (snailingInRightWall)
            {
                float drinkPosY = drinkResults[0].collider.transform.position.y;
                float thisPosY = this.transform.position.y + 0.3f;

                if ((drinkPosY > thisPosY) && !facingRight)
                {
                    Flip();
                }
                else if ((drinkPosY < thisPosY) && facingRight)
                {
                    Flip();
                }
            }

            if (snailingInLeftWall)
            {
                float drinkPosY = drinkResults[0].collider.transform.position.y;
                float thisPosY = this.transform.position.y + 0.3f;

                if ((drinkPosY > thisPosY) && facingRight)
                {
                    Flip();
                }
                else if ((drinkPosY < thisPosY) && !facingRight)
                {
                    Flip();
                }
            }

            numPukes += drinkResults[0].collider.GetComponent<Drink>().Charge;
            Destroy(drinkResults[0].collider.gameObject);
            return;
        }
    }

    void Flip()
    {
        float scaleX = tile.transform.localScale.x;
        scaleX = scaleX * -1;
        tile.transform.localScale = new Vector3(scaleX, 1, 1);
        facingRight = !facingRight;
    }

    #endregion

    #region STATE METHODS

    void IdleState(float time)
    {
        idleTime = time;

        currentPlayerState = PlayerState.Idle;
    }

    void MoveState()
    {
        currentPlayerState = PlayerState.Move;
    }

    public void SpitState()
    {
        currentPlayerState = PlayerState.Spit;
    }

    public void DrinkState()
    {
        currentPlayerState = PlayerState.Drink;
    }

    void DeadState()
    {
        currentPlayerState = PlayerState.Dead;
    }

    void FreezeState()
    {
        currentPlayerState = PlayerState.Freeze;
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Mushroom")
        {
            if(falling)
            {
                Debug.Log(rb.velocity.y);

                if (rb.velocity.y < -5)
                {
                    rb.AddForce(new Vector2(rb.velocity.x/2, -rb.velocity.y * 2), ForceMode2D.Impulse);
                }
                if (rb.velocity.y < -7)
                {
                    rb.AddForce(new Vector2(rb.velocity.x / 5, -rb.velocity.y/2), ForceMode2D.Impulse);
                }
            }
        }
    }

    public Vector2 GetPukePoint()
    {
        Vector2 pukePoint = Vector2.zero;

        if (!snailingInLeftWall && !snailingInRightWall && !facingRight)
        {
            pukePoint = new Vector2(this.transform.position.x + pukePointTopLeft.x, this.transform.position.y + pukePointTopLeft.y);
        }
        if (!snailingInLeftWall && !snailingInRightWall && facingRight)
        {
            pukePoint = new Vector2(this.transform.position.x + pukePointTopRight.x, this.transform.position.y + pukePointTopRight.y);
        }
        if (snailingInLeftWall && !facingRight)
        {
            pukePoint = new Vector2(this.transform.position.x + pukePointRightUp.x, this.transform.position.y + pukePointRightUp.y);
        }
        if (snailingInLeftWall && facingRight)
        {
            pukePoint = new Vector2(this.transform.position.x + pukePointRightDown.x, this.transform.position.y + pukePointRightDown.y);
        }
        if (snailingInRightWall && !facingRight)
        {
            pukePoint = new Vector2(this.transform.position.x + pukePointLeftDown.x, this.transform.position.y + pukePointLeftDown.y);
        }
        if (snailingInRightWall && facingRight)
        {
            pukePoint = new Vector2(this.transform.position.x + pukePointLeftUp.x, this.transform.position.y + pukePointLeftUp.y);
        }

        return pukePoint;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Vector2(this.transform.position.x + topOrigin.x, this.transform.position.y + topOrigin.y), new Vector2(0, topDistance));
        Gizmos.DrawRay(new Vector2(this.transform.position.x + leftUpOrigin.x, this.transform.position.y + leftUpOrigin.y), new Vector2(-leftDistance, 0));
        Gizmos.DrawRay(new Vector2(this.transform.position.x + leftDownOrigin.x, this.transform.position.y + leftDownOrigin.y), new Vector2(-leftDistance, 0));
        Gizmos.DrawRay(new Vector2(this.transform.position.x + bottomUpOrigin.x, this.transform.position.y + bottomUpOrigin.y), new Vector2(0, -bottomDistance));
        Gizmos.DrawRay(new Vector2(this.transform.position.x + bottomDownOrigin.x, this.transform.position.y + bottomDownOrigin.y), new Vector2(0, -bottomDistance));
        Gizmos.DrawRay(new Vector2(this.transform.position.x + rightUpOrigin.x, this.transform.position.y + rightUpOrigin.y), new Vector2(rightDistance, 0));
        Gizmos.DrawRay(new Vector2(this.transform.position.x + rightDownOrigin.x, this.transform.position.y + rightDownOrigin.y), new Vector2(rightDistance, 0));
        Gizmos.DrawIcon(new Vector2(this.transform.position.x + pukePointTopLeft.x, this.transform.position.y + pukePointTopLeft.y), "");
        Gizmos.DrawIcon(new Vector2(this.transform.position.x + pukePointTopRight.x, this.transform.position.y + pukePointTopRight.y), "");
        Gizmos.DrawIcon(new Vector2(this.transform.position.x + pukePointLeftUp.x, this.transform.position.y + pukePointLeftUp.y), "");
        Gizmos.DrawIcon(new Vector2(this.transform.position.x + pukePointLeftDown.x, this.transform.position.y + pukePointLeftDown.y), "");
        Gizmos.DrawIcon(new Vector2(this.transform.position.x + pukePointRightUp.x, this.transform.position.y + pukePointRightUp.y), "");
        Gizmos.DrawIcon(new Vector2(this.transform.position.x + pukePointRightDown.x, this.transform.position.y + pukePointRightDown.y), "");
    }

}
