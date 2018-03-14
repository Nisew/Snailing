using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Game elements")]
    InputManager inputScript;
    public GameObject puke;
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
    public ContactFilter2D contactfilter;

    [Header("Player properties")]
    float speed = 1;
    public float pukeCharge;

    Vector2 goingToPos;
    bool goingUpLeft;
    bool goingDownLeft;
    bool goingUpRight;
    bool goingDownRight;

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
        IdleState();
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
        if(inputScript.PressingUp || inputScript.PressingLeft || inputScript.PressingDown || inputScript.PressingRight)
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
                Vector2 provisionalPos = this.transform.position;

                provisionalPos.x -= speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingLeft && leftUpWalled) //SNAIL LEFT WALL
            {
                snailingInLeftWall = true;
                rb.gravityScale = 0;
            }

            if(inputScript.PressingLeft && !bottomUpWalled) //GOES DOWN RIGHT WALL
            {

            }

            if(inputScript.PressingRight && !rightUpWalled) //MOVE RIGHT
            {
                Vector2 provisionalPos = this.transform.position;

                provisionalPos.x += speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingRight && rightUpWalled) //SNAIL RIGHT WALL
            {
                snailingInRightWall = true;
                rb.gravityScale = 0;
            }

            if(inputScript.PressingRight && !bottomDownWalled) //GOES DOWN TO LEFT WALL
            {
                goingToPos = new Vector2(this.transform.position.x + 0.55f, this.transform.position.y - 1f);
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
                Vector2 provisionalPos = this.transform.position;

                provisionalPos.y += speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingUp && !leftUpWalled) //GOES UP LEFT WALL
            {
                goingToPos = new Vector2(this.transform.position.x - 1, this.transform.position.y + 0.6f);
                goingUpLeft = true;
                snailingInLeftWall = false;
                rb.gravityScale = 1;
                FreezeState();
            }

            if(inputScript.PressingDown && !bottomDownWalled)
            {
                Vector2 provisionalPos = this.transform.position;

                provisionalPos.y -= speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingDown && bottomDownWalled)
            {
                snailingInLeftWall = false;
                rb.gravityScale = 1;
            }

            if(inputScript.PressingSpace)
            {
                rb.gravityScale = 1;
                rb.AddForce(new Vector2(1, 0), ForceMode2D.Impulse);
                snailingInLeftWall = false;
            }
        }

        if(snailingInRightWall)
        {
            if(inputScript.PressingUp && rightUpWalled)
            {
                Vector2 provisionalPos = this.transform.position;

                provisionalPos.y += speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingUp && !rightUpWalled) //TODO: GOES UP THE RIGHT PLATFORM
            {
                goingToPos = new Vector2(this.transform.position.x + 1, this.transform.position.y + 0.6f);
                goingUpRight = true;
                snailingInRightWall = false;
                rb.gravityScale = 1;
                FreezeState();
            }

            if(inputScript.PressingDown && !bottomDownWalled)
            {
                Vector2 provisionalPos = this.transform.position;

                provisionalPos.y -= speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingDown && bottomDownWalled)
            {
                snailingInRightWall = false;
                rb.gravityScale = 1;
            }

            if(inputScript.PressingSpace)
            {
                rb.gravityScale = 1;
                rb.AddForce(new Vector2(-1, 0), ForceMode2D.Impulse);
                snailingInRightWall = false;
            }
        }
    }

    void Spit()
    {
        pukeCharge += Time.deltaTime * 20;

        if(pukeCharge >= 20) pukeCharge = 20;

        if(Input.GetMouseButtonUp(0))
        {
            puke.GetComponent<Puke>().speed = pukeCharge;
            Instantiate(puke, new Vector3(this.transform.position.x + 0.5f, this.transform.position.y + 0.8f, 0), new Quaternion(0, 0, 0, 0));
            pukeCharge = 0;
            IdleState();
        }
    }

    void Drink()
    {

    }

    void Dead()
    {

    }

    void Freeze()
    {
        if(goingUpLeft) GoToUpLeft();

        if(goingUpRight) GoToUpRight();

        if(goingDownRight) GoToDownRight();
    }

    #endregion

    void GoToUpLeft()
    {
        if(this.transform.position.y <= goingToPos.y)
        {
            Vector2 provisionalPos = this.transform.position;

            provisionalPos.y += Time.deltaTime*5;

            this.gameObject.transform.position = provisionalPos;

            if(this.transform.position.y >= goingToPos.y && this.transform.position.x > goingToPos.x)
            {
                provisionalPos.y = goingToPos.y;

                provisionalPos.x -= Time.deltaTime * 5;

                this.gameObject.transform.position = provisionalPos;

                if(this.transform.position.x <= goingToPos.x)
                {
                    this.gameObject.transform.position = goingToPos;
                    goingUpLeft = false;
                    IdleState();
                }
            }
        }
    }

    void GoToUpRight()
    {
        if(this.transform.position.y <= goingToPos.y)
        {
            Vector2 provisionalPos = this.transform.position;

            provisionalPos.y += Time.deltaTime * 5;

            this.gameObject.transform.position = provisionalPos;

            if(this.transform.position.y >= goingToPos.y && this.transform.position.x < goingToPos.x)
            {
                provisionalPos.y = goingToPos.y;

                provisionalPos.x += Time.deltaTime * 5;

                this.gameObject.transform.position = provisionalPos;

                if(this.transform.position.x >= goingToPos.x)
                {
                    this.gameObject.transform.position = goingToPos;
                    goingUpRight = false;
                    IdleState();
                }
            }
        }
    }

    void GoToDownRight()
    {
        if(this.transform.position.x <= goingToPos.x)
        {
            Vector2 provisionalPos = this.transform.position;

            provisionalPos.x += Time.deltaTime * 5;

            this.gameObject.transform.position = provisionalPos;

            if(this.transform.position.x >= goingToPos.x && this.transform.position.y > goingToPos.y)
            {
                provisionalPos.x = goingToPos.x;

                provisionalPos.y -= Time.deltaTime * 5;

                this.gameObject.transform.position = provisionalPos;

                if(this.transform.position.y <= goingToPos.y)
                {
                    this.gameObject.transform.position = goingToPos;
                    goingDownRight = false;
                    IdleState();
                }
            }
        }
    }



    #region WALL DETECTION METHODS

    void TopWallDetection()
    {
        topWalled = false;

        numHits = Physics2D.Raycast(new Vector2(this.transform.position.x + topOrigin.x, this.transform.position.y + topOrigin.y), new Vector2(0, 1), contactfilter, rayHit, topDistance);

        if(numHits > 0)
        {
            topWalled = true;
        }        
    }

    void LeftWallDetection()
    {
        leftUpWalled = false;
        leftDownWalled = false;

        numHits = Physics2D.Raycast(new Vector2(this.transform.position.x + leftUpOrigin.x, this.transform.position.y + leftUpOrigin.y), new Vector2(-1, 0), contactfilter, rayHit, leftDistance);

        if(numHits > 0)
        {
            leftUpWalled = true;
        }

        numHits = Physics2D.Raycast(new Vector2(this.transform.position.x + leftDownOrigin.x, this.transform.position.y + leftDownOrigin.y), new Vector2(-1, 0), contactfilter, rayHit, leftDistance);

        if(numHits > 0)
        {
            leftDownWalled = true;
        }
    }

    void BottomWallDetection()
    {
        bottomUpWalled = false;
        bottomDownWalled = false;

        numHits = Physics2D.Raycast(new Vector2(this.transform.position.x + bottomUpOrigin.x, this.transform.position.y + bottomUpOrigin.y), new Vector2(0, -1), contactfilter, rayHit, bottomDistance);
        
        if(numHits > 0)
        {
            Debug.Log("bottomUpWalled");
            bottomUpWalled = true;
        }

        numHits = Physics2D.Raycast(new Vector2(this.transform.position.x + bottomDownOrigin.x, this.transform.position.y + bottomDownOrigin.y), new Vector2(0, -1), contactfilter, rayHit, bottomDistance);

        if(numHits > 0)
        {
            bottomDownWalled = true;
        }
    }

    void RightWallDetection()
    {
        rightUpWalled = false;
        rightDownWalled = false;

        numHits = Physics2D.Raycast(new Vector2(this.transform.position.x + rightUpOrigin.x, this.transform.position.y + rightUpOrigin.y), new Vector2(1, 0), contactfilter, rayHit, rightDistance);

        if(numHits > 0)
        {
            rightUpWalled = true;
        }

        numHits = Physics2D.Raycast(new Vector2(this.transform.position.x + rightDownOrigin.x, this.transform.position.y + rightDownOrigin.y), new Vector2(1, 0), contactfilter, rayHit, rightDistance);

        if(numHits > 0)
        {
            rightDownWalled = true;
        }
    }

    #endregion

    #region STATE METHODS

    void IdleState()
    {
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

    void DrinkState()
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

    private void OnDrawGizmos()
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
