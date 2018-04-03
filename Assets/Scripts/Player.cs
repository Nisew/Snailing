using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Game elements")]
    InputManager inputScript;
    public GameObject puke;
    GameObject tile;
    Rigidbody2D rb;

    [Header("Ground detection")]
    public Vector2 topOrigin;
    public float topDistance;

    public Vector2 leftUpOrigin;
    public Vector2 leftDownOrigin;
    public float leftDistance;

    public Vector2 bottomUpOrigin;
    public Vector2 bottomDownOrigin;
    public float bottomDistance;

    public Vector2 rightUpOrigin;
    public Vector2 rightDownOrigin;
    public float rightDistance;

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
    public ContactFilter2D contactFilter;
    public ContactFilter2D drinkFilter;

    [Header("Player properties")]
    float speed = 1;
    bool falling;
    public float pukeCharge;
    float maxPukeChare = 10;
    bool canDrink;
    bool drinking;
    float drinkTime = 1;
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
                goingToPos = new Vector2(this.transform.position.x - 0.7f, this.transform.position.y - 1f);
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
                goingToPos = new Vector2(this.transform.position.x + 0.6f, this.transform.position.y - 1f);
                tile.transform.localRotation = Quaternion.Euler(0, 0, -90);
                goingDownRight = true;
                snailingInLeftWall = true;
                rb.gravityScale = 0;
                FreezeState();
            }
        }

        if(snailingInLeftWall)
        {
            if(inputScript.PressingUp && leftUpWalled)
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
            if(inputScript.PressingUp && rightUpWalled)
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
        pukeCharge += Time.deltaTime * 10;

        if(pukeCharge >= maxPukeChare) pukeCharge = maxPukeChare;

        if(Input.GetMouseButtonUp(0))
        {
            puke.GetComponent<Puke>().speed = pukeCharge;
            Instantiate(puke, new Vector3(this.transform.position.x + 0.5f, this.transform.position.y + 0.8f, 0), new Quaternion(0, 0, 0, 0));
            pukeCharge = 0;
            IdleState(0.5f);
        }
    }

    void Drink()
    {
        drinkTime -= Time.deltaTime;

        if(drinkTime < 0)
        {
            drinkTime = 1;
            TryDrink();
            IdleState(0);
        }
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
    }

}
