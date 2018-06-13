using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Game elements")]
    InputManager inputScript;
    [SerializeField] GameObject puke;
    public PukePath pukePath;
    GameObject tile;
    public PukeBar pukeBar;
    Rigidbody2D rb;
    PlaySound playSound;
    public SceneMaster sceneM;

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

    public bool obstacleInLeft;
    public bool obstacleInRight;
    public bool obstacleInUp;
    public bool obstacleInDown;

    public bool snailingInLeftWall;
    public bool snailingInRightWall;

    RaycastHit2D[] rayHit = new RaycastHit2D[1];
    int numHits;
    [SerializeField] ContactFilter2D contactFilter;
    [SerializeField] ContactFilter2D obstacleFilter;
    [SerializeField] ContactFilter2D drinkFilter;

    [Header("Player properties")]
    float speed = 2;
    public bool falling;

    public Vector2 pukePointTopLeft;
    public Vector2 pukePointTopRight;
    public Vector2 pukePointLeftUp;
    public Vector2 pukePointLeftDown;
    public Vector2 pukePointRightUp;
    public Vector2 pukePointRightDown;

    public int numPukes = 5;
    [SerializeField]
    public bool powered;
    bool poweredColor;
    float colorTime;
    public float pukeCharge;
    float maxPukeChare = 10;
    bool canDrink;
    bool drinking;
    float drinkDistance = 0.6f;
    float idleTime;
    float snailWallTime = 0.4f;
    bool facingRight = true;
    Color playerColor;

    Vector2 goingToPos;
    bool goingUpLeft;
    bool goingDownLeft;
    bool goingUpRight;
    bool goingDownRight;
    bool arrived;
    public bool gameOver;

    Animator anim;

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
        anim = GetComponentInChildren<Animator>();
        pukePath = GetComponent<PukePath>();
        rb = GetComponent<Rigidbody2D>();
        tile = this.transform.GetChild(0).gameObject;
        IdleState(0f);
        pukeBar.ChangePukeBar(numPukes);
        playerColor = GetComponentInChildren<SpriteRenderer>().color;
        playSound = GameObject.Find("GameMaster").GetComponent<PlaySound>();

    }

    void FixedUpdate()
    {
        TopWallDetection();
        LeftWallDetection();
        BottomWallDetection();
        RightWallDetection();
        UpObstacleDetection();
        LeftObstacleDetection();
        RightObstacleDetection();
        DownObstacleDetection();
    }

    void Update ()
    {
        if(powered && !poweredColor)
        {
            colorTime += Time.deltaTime;
            playerColor = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 0.5f, 1, 1), colorTime);
            GetComponentInChildren<SpriteRenderer>().color = playerColor;
            if (colorTime >= 5)
            {
                poweredColor = true;
            }
        }

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
        anim.SetBool("Walk", false);

        if (!snailingInLeftWall && !snailingInRightWall && (bottomUpWalled || bottomDownWalled))
        {
            if(inputScript.PressingLeft && !leftUpWalled && bottomUpWalled && !obstacleInLeft) //MOVE LEFT
            {
                anim.SetBool("Walk", true);
                if (facingRight) Flip();
                Vector2 provisionalPos = this.transform.position;

                provisionalPos.x -= speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingLeft && leftUpWalled) //SNAIL LEFT WALL
            {
                if (facingRight) Flip();
                snailingInLeftWall = true;
                anim.SetTrigger("SnailWall");
                rb.gravityScale = 0;
                IdleState(0.5f);
            }

            if(inputScript.PressingLeft && !bottomUpWalled) //GOES DOWN TO RIGHT WALL
            {
                anim.SetBool("Walk", false);

                anim.SetBool("Falling", true);
                goingToPos = new Vector2(this.transform.position.x - 0.75f, this.transform.position.y - 1f);
                goingDownLeft = true;
                snailingInRightWall = true;
                rb.gravityScale = 0;
                FreezeState();
            }

            if(inputScript.PressingRight && !rightUpWalled && !obstacleInRight) //MOVE RIGHT
            {
                anim.SetBool("Walk", true);

                if (!facingRight) Flip();

                Vector2 provisionalPos = this.transform.position;

                provisionalPos.x += speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingRight && rightUpWalled) //SNAIL RIGHT WALL
            {
                if (!facingRight) Flip();
                snailingInRightWall = true;
                anim.SetTrigger("SnailWall");
                rb.gravityScale = 0;
                IdleState(0.5f);
            }

            if (inputScript.PressingRight && !bottomDownWalled) //GOES DOWN TO LEFT WALL
            {
                anim.SetBool("Walk", false);

                anim.SetBool("Falling", true);
                goingToPos = new Vector2(this.transform.position.x + 0.75f, this.transform.position.y - 1f);
                goingDownRight = true;
                snailingInLeftWall = true;
                rb.gravityScale = 0;
                FreezeState();
            }
        }

        if(snailingInLeftWall)
        {
            if(inputScript.PressingUp && leftUpWalled && !topWalled && !obstacleInUp)
            {
                anim.SetBool("Walk", true);

                if (facingRight) Flip();

                Vector2 provisionalPos = this.transform.position;

                provisionalPos.y += speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingUp && !leftUpWalled) //GOES UP LEFT WALL
            {
                goingToPos = new Vector2(this.transform.position.x - 1, this.transform.position.y + 1f);
                goingUpLeft = true;
                snailingInLeftWall = false;
                anim.SetBool("Falling", true);
                FreezeState();
            }

            if(inputScript.PressingDown && !bottomDownWalled && !obstacleInDown && leftDownWalled)
            {
                anim.SetBool("Walk", true);

                if (!facingRight) Flip();

                Vector2 provisionalPos = this.transform.position;

                provisionalPos.y -= speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingDown && bottomDownWalled)
            {
                if (!facingRight) Flip();
                snailingInLeftWall = false;
                anim.SetTrigger("SnailWall");
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
                anim.SetBool("Falling", true);
                FreezeState();
            }
        }

        if(snailingInRightWall)
        {
            if(inputScript.PressingUp && rightUpWalled && !topWalled && !obstacleInUp)
            {
                anim.SetBool("Walk", true);

                if (!facingRight) Flip();

                Vector2 provisionalPos = this.transform.position;

                provisionalPos.y += speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingUp && !rightUpWalled) //GOES UP RIGHT WALL
            {
                goingToPos = new Vector2(this.transform.position.x + 1, this.transform.position.y + 1f);
                goingUpRight = true;
                anim.SetBool("Falling", true);
                snailingInRightWall = false;
                FreezeState();
            }

            if(inputScript.PressingDown && !bottomDownWalled && !obstacleInDown && rightDownWalled)
            {
                anim.SetBool("Walk", true);

                if (facingRight) Flip();

                Vector2 provisionalPos = this.transform.position;

                provisionalPos.y -= speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputScript.PressingDown && bottomDownWalled)
            {
                if (facingRight) Flip();
                snailingInRightWall = false;
                anim.SetTrigger("SnailWall");
                rb.gravityScale = 1;
                IdleState(0.5f);
            }

            if (inputScript.PressingSpace)
            {
                rb.gravityScale = 1;
                rb.AddForce(new Vector2(-1, 0), ForceMode2D.Impulse);
                snailingInRightWall = false;
                tile.transform.localRotation = Quaternion.Euler(0, 0, 0);
                falling = true;
                anim.SetBool("Falling", true);
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
            anim.SetTrigger("Puke");
            playSound.Play(2, 1, 1);
            IdleState(0.5f);
        }
    }

    void Drink()
    {

        anim.SetTrigger("Drink");
        playSound.Play(1, 1, 1);

        if (powered)
        {
            RaycastHit2D[] drinkResults = new RaycastHit2D[1];

            int drinks = 0;

            drinks = rb.Cast(new Vector2(1f, 0f), drinkFilter, drinkResults, drinkDistance);

            if(drinks > 0)
            {
                if(drinkResults[0].collider.tag == "BigPuke")
                {
                    anim.SetTrigger("PermaDrink");
                    FreezeState();
                    gameOver = true;
                    sceneM.EndingScreen();
                }
            }
        }

        IdleState(1.1f);
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
                anim.SetBool("Falling", false);
                playSound.Play(7, 1, 1);
                falling = false;
                IdleState(0.5f);
           }
        }
    }

    #endregion

    #region WALL TRANSITION METHODS

    void GoToUpLeft()
    {
        if(this.transform.position.y >= goingToPos.y)
        {
            Vector2 provisionalPos = this.transform.position;
            provisionalPos.y = goingToPos.y;
            this.gameObject.transform.position = provisionalPos;
        }

        if (this.transform.position.y <= goingToPos.y)
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
                    tile.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    anim.SetBool("Falling", false);
                    rb.gravityScale = 1;
                    IdleState(0.5f);
                }
            }
        }
    }

    void GoToUpRight()
    {
        if(this.transform.position.y >= goingToPos.y)
        {
            Vector2 provisionalPos = this.transform.position;
            provisionalPos.y = goingToPos.y;
            this.gameObject.transform.position = provisionalPos;
        }

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
                    tile.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    goingUpRight = false;
                    anim.SetBool("Falling", false);
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
                    Rotate();
                    anim.SetBool("Falling", false);
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
            anim.SetBool("Falling", false);
            Rotate();
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
                    Rotate();
                    anim.SetBool("Falling", false);
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
            Rotate();
            anim.SetBool("Falling", false);
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

    #region OBSTACLE DETECTION METHODS

    void UpObstacleDetection()
    {
        numHits = Physics2D.Raycast(this.transform.position, new Vector2(0, 1), obstacleFilter, rayHit, 1.1f);

        if (numHits > 0)
        {
            obstacleInUp = true;
        }
        else obstacleInUp = false;
    }

    void LeftObstacleDetection()
    {
        numHits = Physics2D.Raycast(this.transform.position, new Vector2(-1, 0), obstacleFilter, rayHit, 0.55f);

        if (numHits > 0)
        {
            obstacleInLeft = true;
        }
        else obstacleInLeft = false;
    }

    void DownObstacleDetection()
    {
        numHits = Physics2D.Raycast(this.transform.position, new Vector2(0, -1), obstacleFilter, rayHit, 0.1f);

        if (numHits > 0)
        {
            obstacleInDown = true;
        }
        else obstacleInDown = false;
    }

    void RightObstacleDetection()
    {
        numHits = Physics2D.Raycast(this.transform.position, new Vector2(1, 0), obstacleFilter, rayHit, 0.55f);

        if (numHits > 0)
        {
            obstacleInRight = true;
        }
        else obstacleInRight = false;
    }

    #endregion

    #region MECHANICS METHODS

    public void TryDrink()
    {
        RaycastHit2D[] drinkResults = new RaycastHit2D[1];

        int drinks = 0;

        if(facingRight && !snailingInLeftWall && !snailingInRightWall)
        {
            drinks = rb.Cast(new Vector2(1f, 0f), drinkFilter, drinkResults, drinkDistance);
        }
        if (!facingRight && !snailingInLeftWall && !snailingInRightWall)
        {
            drinks = rb.Cast(new Vector2(-1f, 0f), drinkFilter, drinkResults, drinkDistance);
        }
        if (facingRight && snailingInLeftWall || !facingRight && snailingInRightWall)
        {
            drinks = rb.Cast(new Vector2(0f, -1f), drinkFilter, drinkResults, drinkDistance);
        }
        if (!facingRight && snailingInLeftWall || facingRight && snailingInRightWall)
        {
            drinks = rb.Cast(new Vector2(0f, 1f), drinkFilter, drinkResults, drinkDistance);
        }

        if (drinks > 0)
        {
            if(drinkResults[0].collider.gameObject.layer == 15)
            {
                if(!powered)
                {
                    if(drinkResults[0].collider.gameObject.GetComponentInChildren<Plutonium>().melted)
                    {
                        numPukes = 1;
                        drinkResults[0].collider.gameObject.GetComponentInChildren<Animator>().SetTrigger("Drink");
                        drinkResults[0].collider.gameObject.GetComponentInChildren<Plutonium>().drinked = true;
                        PowerUp();
                    }
                }
            }
            else
            {
                if(!powered)
                {
                    numPukes += drinkResults[0].collider.GetComponent<Drink>().Charge;
                    if (numPukes > 10) numPukes = 10;
                    pukeBar.ChangePukeBar(numPukes);
                    drinkResults[0].collider.gameObject.GetComponentInChildren<Animator>().SetTrigger("Drink");
                }
                else
                {
                    drinkResults[0].collider.gameObject.GetComponentInChildren<Animator>().SetTrigger("Drink");
                }
            }
        }
    }

    void PowerUp()
    {
        powered = true;
        pukeBar.powered = true;
        pukeBar.ChangePukeBar(10);
        pukeCharge = 1;
    }

    void Flip()
    {
        float scaleX = tile.transform.localScale.x;
        scaleX = scaleX * -1;
        tile.transform.localScale = new Vector3(scaleX, 1, 1);
        facingRight = !facingRight;
    }

    public void Puke()
    {
        pukePath.DesactivateTrajectoryPoints();

        puke.GetComponent<Puke>().pukeCharge = 1;
        puke.GetComponent<Puke>().Speed = pukePath.GetForceFrom(GetPukePoint(), Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if(!powered)
        {
            numPukes --;
            pukeBar.ChangePukeBar(numPukes);
        }

        Instantiate(puke, GetPukePoint(), new Quaternion(0, 0, 0, 0));

        IdleState(0.2f);
    }

    public void ShroomJump(Vector2 force)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
        playSound.Play(4, 1, 1);
    }

    public void Die()
    {
        DeadState();
        anim.SetTrigger("Die");
    }

    public void Rotate()
    {
        if(!snailingInLeftWall && !snailingInRightWall)
        {
            tile.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        if (snailingInLeftWall)
        {
            tile.transform.localRotation = Quaternion.Euler(0, 0, -90);
        }
        if (snailingInRightWall)
        {
            tile.transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
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

    #region ANIMATION METHODS

    public void Walking()
    {
        anim.SetBool("Walk", true);
    }

    public void NotWalking()
    {
        anim.SetBool("Walk", false);
    }

    #endregion

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "TriggerCamera")
        {
            GameObject.Find("Camera").GetComponent<CameraFollow>().BossPosition();
            rb.gravityScale = 1;
            rb.AddForce(new Vector2(1, 0), ForceMode2D.Impulse);
            tile.transform.localRotation = Quaternion.Euler(0, 0, 0);
            snailingInLeftWall = false;
            falling = true;
            anim.SetBool("Falling", true);
            GameObject.Find("Boss").GetComponent<Boss>().active = true;
            FreezeState();
        }
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
