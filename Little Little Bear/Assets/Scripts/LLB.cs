using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LLB : BasicEntity
{
    private int horizontal = 0; //store direction we are moving
    private int vertical = 0;

    // List of personal boolean
    // private bool turnEnd; // When true enemies can move
    private bool dig;
    private bool checkInput; // If true we can accept user input, avoids interrupting animation
    protected override void Start()
    {
        //turnEnd = false;
        dig = false;
        checkInput = true;
        health = 100;
        strength = 4;
        
        base.Start();
        //health = GameManager.instance.playerHealth; // grab loaded health

    }

    private void OnEnable() // When level starts up we enable the player entity
    {
        flipped = false;
    }

    private void OnDisable() // When object is disabled
    {
        //GameManager.instance.playerHealth = health; // applies when we change levels, do this for all stats
    }

    private bool Move(int xDir, int yDir) // out let us return multiple values
    {
        board = GameObject.Find("LevelTilesGenerator").GetComponent<gameManager>().board;
        selfEntity = board.map[currentX, currentY].entityType;
        Vector2 sPos = transform.position; //Start Position
        Vector2 ePos = sPos + new Vector2(xDir, yDir); // End Position

        if (board.map[xDir, yDir].entityType == EntitySet.NOTHING) // if nothing is there(for now)
        {
            switch (board.map[xDir, yDir].tileType)
            {
                //don't move there
                case TileSet.BOULDER:
                case TileSet.ROCK:
                case TileSet.WALL:
                    Debug.Log("CONTAINS " + board.map[xDir, yDir].tileType);
                    //do nothing
                    return false;

                default: // Currently default since moving is only here
                    Debug.Log("MOVING X: " + xDir + " AND Y: " + yDir);
                    Debug.Log("CONTAINS " + board.map[xDir, yDir].tileType);
                    board.map[currentX, currentY].entityType = EntitySet.NOTHING; // nothing where you where
                    board.map[xDir, yDir].entityType = selfEntity; // you are here now
                    currentX = xDir;
                    currentY = yDir;
                    return true;
            }
        }
        else //something is there
        {
            Debug.Log("CONTAINS " + board.map[xDir, yDir].entityType);
        }

        return true; // If nothing is hit then assume move
    }

    private void Update()
    {
        //if (!GameManager.instance.playersTurn) return; // means following code wont run unless its player turn

        if (checkInput) //No previous actions are being executed
        {
            if (Input.GetMouseButtonDown(0)) // left mouse click
            {
                attack = true; // player will attempt to attack
                checkInput = false; //input has been read
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                dig = true;
                checkInput = false;
            }

            //movement vector input
            horizontal = (int)Input.GetAxisRaw("Horizontal"); //using ints forces 1 unit movement
            vertical = (int)Input.GetAxisRaw("Vertical");

            if (horizontal != 0) //No diagonal
                vertical = 0;

            if (horizontal != 0 || vertical != 0) // There must be movement input
            {
                if (Move(currentX + horizontal, currentY + vertical)) //Current location + moveVector

                {
                    checkInput = false;

                    if (horizontal == -1)  // Identifies direction to move and then flips boolean to take action
                        moveLeft = true;
                    else if (horizontal == 1)
                        moveRight = true;
                    else if (vertical == -1)
                        moveDown = true;
                    else
                        moveUp = true;
                }

            }
        }
    }
    void FixedUpdate() // Where animation and actions take place
    {
        if (attack)
        {
            attack = false; // reset bool so input can be taken again
            animator.SetTrigger("Attack"); // Attack animation triggered
            StartCoroutine(wait2Move('o', 1.5f)); // Starts animation timer and should stop inputs
        }
        else if (dig)
        {
            dig = false;
            animator.SetTrigger("Dig");
            StartCoroutine(wait2Move('o', 2f));
        }
        else if (moveLeft)
        {
            moveLeft = false;
            if (!flipped) // LLB looking right about to go left, so we flip her sprite
            {
                Vector3 tempS = transform.localScale;
                tempS.x *= -1;  // Flips sprite
                transform.localScale = tempS;
                flipped = true;
            }
            animator.SetTrigger("WalkX");
            StartCoroutine(wait2Move('l', 0.3f)); //Left
        }
        else if (moveRight) // LLB looking left about to go right so we flip her sprite
        {
            moveRight = false;
            if (flipped)
            {
                Vector3 tempS = transform.localScale;
                tempS.x *= -1;  // Flips sprite
                transform.localScale = tempS;
                flipped = false;
            }
            animator.SetTrigger("WalkX");
            StartCoroutine(wait2Move('r', 0.3f));
        }
        else if (moveUp)
        {
            moveUp = false;
            StartCoroutine(wait2Move('u', 0.3f));
        }
        else if (moveDown)
        {
            moveDown = false;
            StartCoroutine(wait2Move('d', 0.3f));
        }

        horizontal = 0; // Resets input
        vertical = 0;
    }

    private IEnumerator wait2Move(char c, float t) // c is action, t (seconds) is time to wait for action
    {
        switch (c)
        {
            case 'l':  //Move left
                {
                    transform.Translate((Vector2.left) / 2); // Splits the difference so its a 2 step
                    yield return new WaitForSeconds(0.1f);
                    transform.Translate((Vector2.left) / 2);
                    yield return new WaitForSeconds(t - 0.1f); // Lagtime after movement is complete
                }

                break;
            case 'r': // Move right
                {
                    transform.Translate((Vector2.right) / 2);
                    yield return new WaitForSeconds(0.1f);
                    transform.Translate((Vector2.right) / 2);
                    yield return new WaitForSeconds(t - 0.1f);
                }
                break;
            case 'u': // Move up
                {
                    transform.Translate((Vector2.up) / 2);
                    yield return new WaitForSeconds(0.1f);
                    transform.Translate((Vector2.up) / 2);
                    yield return new WaitForSeconds(t - 0.1f);
                }
                break;
            case 'd': // Move Down
                {
                    transform.Translate((Vector2.down) / 2);
                    yield return new WaitForSeconds(0.1f);
                    transform.Translate((Vector2.down) / 2);
                    yield return new WaitForSeconds(t - 0.1f);
                }
                break;
            case 'o': //Other, just wait for animation to end
                {
                    yield return new WaitForSeconds(t); // Nothing special just wait t seconds
                }
                break;
        }
        //GameManager.instance.playersTurn = false; // Action complete, player loses turn enemies go
        checkInput = true; // Inputs are able to be taken again
    }
}



