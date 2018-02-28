using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Game elements")]
    Inputs inputs;

    [Header("Box properties")]
    public Vector2 topBoxPos;
    public Vector2 topBoxSize;

    public Vector2 leftBoxPos;
    public Vector2 leftBoxSize;

    public Vector2 bottomBoxPos;
    public Vector2 bottomBoxSize;

    public Vector2 rightBoxPos;
    public Vector2 rightBoxSize;

    [Header("Filter")]
    public ContactFilter2D filter;
    public int maxColliders = 1;

    [Header("Player properties")]
    float speed = 0.5f;

    public bool topWalled;
    public bool leftWalled;
    public bool rightWalled;
    public bool bottomWalled;
    public bool snailingInWall;

    PlayerState currentPlayerState;
    enum PlayerState
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
        inputs = GameObject.FindGameObjectWithTag("Input").GetComponent<Inputs>();
        IdleState();
	}

    void FixedUpdate()
    {
        ResetState();
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
        if(inputs.PressingUp || inputs.PressingLeft || inputs.PressingDown || inputs.PressingRight)
        {
            MoveState();
        }
    }

    void Move()
    {
        if(!snailingInWall)
        {
            if(inputs.PressingLeft && !leftWalled)
            {
                Vector3 provisionalPos = this.transform.position;

                provisionalPos.x -= speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputs.PressingLeft && leftWalled)
            {
                snailingInWall = true;
            }

            if(inputs.PressingRight && !rightWalled)
            {
                Vector3 provisionalPos = this.transform.position;

                provisionalPos.x += speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputs.PressingRight && rightWalled)
            {
                snailingInWall = true;
            }
        }

        if(snailingInWall)
        {
            if(inputs.PressingUp && !topWalled)
            {
                Vector3 provisionalPos = this.transform.position;

                provisionalPos.y += speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }

            if(inputs.PressingDown && !bottomWalled)
            {
                Vector3 provisionalPos = this.transform.position;

                provisionalPos.y -= speed * Time.deltaTime;

                this.gameObject.transform.position = provisionalPos;
            }
            else if(inputs.PressingDown && bottomWalled)
            {
                snailingInWall = false;
            }
        }
    }

    void Spit()
    {

    }

    void Drink()
    {

    }

    void Dead()
    {

    }

    void Freeze()
    {

    }

    #endregion

    #region DETECTION METHODS

    void ResetState()
    {
        topWalled = false;
        leftWalled = false;
        bottomWalled = false;
        rightWalled = false;
    }

    void TopWallDetection()
    {
        Vector3 pos = this.transform.position + (Vector3)topBoxPos;
        Collider2D[] results = new Collider2D[maxColliders];

        int numColliders = Physics2D.OverlapBox(pos, topBoxSize, 0, filter, results);

        if(numColliders > 0) topWalled = true;
    }

    void LeftWallDetection()
    {
        Vector3 pos = this.transform.position + (Vector3)leftBoxPos;
        Collider2D[] results = new Collider2D[maxColliders];

        int numColliders = Physics2D.OverlapBox(pos, leftBoxSize, 0, filter, results);

        if(numColliders > 0) leftWalled = true;
    }

    void BottomWallDetection()
    {
        Vector3 pos = this.transform.position + (Vector3)bottomBoxPos;
        Collider2D[] results = new Collider2D[maxColliders];

        int numColliders = Physics2D.OverlapBox(pos, bottomBoxSize, 0, filter, results);

        if(numColliders > 0) bottomWalled = true;
    }

    void RightWallDetection()
    {
        Vector3 pos = this.transform.position + (Vector3)rightBoxPos;
        Collider2D[] results = new Collider2D[maxColliders];

        int numColliders = Physics2D.OverlapBox(pos, rightBoxSize, 0, filter, results);

        if(numColliders > 0) rightWalled = true;
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

    void SpitState()
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 pos1 = this.transform.position + (Vector3)topBoxPos;
        Gizmos.DrawWireCube(pos1, topBoxSize);

        Gizmos.color = Color.red;
        Vector3 pos = this.transform.position + (Vector3)leftBoxPos;
        Gizmos.DrawWireCube(pos, leftBoxSize);

        Gizmos.color = Color.yellow;
        Vector3 pos4 = this.transform.position + (Vector3)bottomBoxPos;
        Gizmos.DrawWireCube(pos4, bottomBoxSize);

        Gizmos.color = Color.green;
        Vector3 pos2 = this.transform.position + (Vector3)rightBoxPos;
        Gizmos.DrawWireCube(pos2, rightBoxSize);
        
    }
}
