using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LLB : BasicEntity
{
    private int horizontal = 0; //store direction we are moving
    private int vertical = 0;
    private char damageType;
    private int stamina;  // Special move gague, when below a certain amount you cant use special
    private int aDirX = 1, aDirY = 0; // Attack direction x and y, how we aim
    private char attackDir; // char hold, for Combat();
    private int invEquipped; // 0 melee, 1 range, 2 first item, 3 second item
    private char weaponType; // b=blunt, t=thrust, s=slice, r=ranged

    // List of personal boolean
    private bool specialA; // special attack
    private bool dig;
    private bool turnEnd;
    private bool attackWait;
    private bool checkInput; // If true we can accept user input, avoids interrupting animation
    
    /*//inventory variables
    private Inventory inventory;
    public GameObject itemButton;*/

    public Highlight targetHighlight;  // Targeting script

    protected override void Start()
    {
        turnEnd = false;
        dig = false;
        attackWait = false;
        checkInput = true;
        invEquipped = 0; // Start on weapon slot
        range = 10; // base range on range weapon. I dont know if this will ever change
        weaponType = 's'; // Start with a carrot which is blunt
        health = 100;
        stamina = 100;
        strength = 4;
        targetHighlight = GameObject.Find("Highlight").GetComponent<Highlight>();
        base.Start();
        //inventory = GetComponent<Inventory>();
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

    private void Combat(bool special) // basic : special = false, direction = d 
    {
        GameObject enemy = null;
        Debug.Log("-------------------------------------------------------------\nATTACK STARTED");
        if (special)
         {
            switch (damageType)
            {
                case 'b': // blunt
                        //Knockback + stun
                break;
                case 's': // slash
                              //Wide slash (3 tiles in a perpindicular line)
                break;
                case 't': // thrust
                              //Multi-Hit
                break;
            }
         }
         else
         {
            switch (attackDir)
            {
                case 'l':
                    enemy = board.map[currentX - 1, currentY].entity;
                    break;
                case 'r':
                    enemy = board.map[currentX + 1, currentY].entity;
                    break;
                case 'u':
                    enemy = board.map[currentX, currentY + 1].entity;
                    break;
                case 'd':
                    enemy = board.map[currentX, currentY - 1].entity;
                    break;
            }
           
            if (enemy != null)
            {
                Debug.Log("-------------------------------------------------------------\nATTEMPT");
                enemy.GetComponent<EnemyBasic>().Hurt(strength);

            }
            else
            {
                Debug.Log("NULL");
            }
         }
   
    }

    private bool Move(int xDir, int yDir) // out let us return multiple values
    {
        board = GameObject.Find("LevelTilesGenerator").GetComponent<gameManager>().board;
        // selfEntity = board.map[currentX, currentY].entityType;
        Vector2 sPos = transform.position; //Start Position
        Vector2 ePos = sPos + new Vector2(xDir, yDir); // End Position

        if (board.map[xDir, yDir].entity == null) // if nothing is there(for now)
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

                    board.map[xDir, yDir].entity = board.map[currentX, currentY].entity;
                    board.map[currentX, currentY].entity = null;

                    // board.map[currentX, currentY].entityType = EntitySet.NOTHING; // nothing where you where
                    // board.map[xDir, yDir].entityType = selfEntity; // you are here now
                    currentX = xDir;    // OverwritePosition
                    currentY = yDir;

                    //written by Maleah, pickup item for inventory
                    //check ground for item
                    if (board.map[xDir, yDir].item != null)
                    {
                        //try to pickup item
                        PickUp(board.map[xDir, yDir].item);
                    }
                    //written by Thomas, change the level
                    if (board.map[xDir, yDir].tileType == TileSet.END_TILE)
                    {
                        gameManager.instance.LoadScene(1);
                    }
                    return true;
            }
        }
        else //something is there
        {            
            Debug.Log("CONTAINS " + board.map[xDir, yDir].entity);
            return false;
        }

        // return true; // If nothing is hit then assume move
    }

    private void PickUp(GameObject item)   // Picks up the item off the floor (In the future we can add UI)
    {
        item.GetComponent<Item>().pickup();
        /*
        Debug.Log("Running pickup function");
        for(int i = 0; i < inventory.slots.Length; i++) //checking if inventory is full
        {
            if(inventory.isFull[i] == false)    //not full, pickup item
            {
                Debug.Log("Picked up item");
                //add item
                inventory.isFull[i] = true;
                
                //if item is blueberry then this
                Instantiate(itemButton, inventory.slots[i].transform, false);
                Destroy(item);
                break;
            }
        }
        */
    }
    
    private void Update()
    {
        if (checkInput) //No previous actions are being executed
        {
            if (Input.GetMouseButtonDown(0)) // left mouse click
            {
                attack = true; // player will attempt to attack
                checkInput = false; //input has been read
            }
            if (Input.GetMouseButtonDown(1)) // right mouse click
            {
                specialA = true; // player will attempt to special attack
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
                // Debug.Log("*******LLB*******");
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
        else if (attackWait) // Waiting for decided direction
        {
            aDirX = (int)Input.GetAxisRaw("Horizontal"); //using ints forces 1 unit movement
            aDirY = (int)Input.GetAxisRaw("Vertical");
            if (aDirX != 0 || aDirY != 0) // There must be movement input
            {
                if (aDirX == -1)  // Identifies direction
                {
                    targetHighlight.Aim('l');
                    attackDir = 'l';
                    if (!flipped)
                    {
                        Vector2 tempS = transform.localScale;
                        tempS.x *= -1;  // Flips sprite
                        transform.localScale = tempS;
                        flipped = true;
                    }
                }
                else if (aDirX == 1)
                {
                    targetHighlight.Aim('r');
                    attackDir = 'r';
                    if (flipped)
                    {
                        Vector2 tempS = transform.localScale;
                        tempS.x *= -1;  // Flips sprite
                        transform.localScale = tempS;
                        flipped = false;
                    }
                }
                else if (aDirY == -1)
                {
                    targetHighlight.Aim('d');
                    attackDir = 'd';
                }
                else if (aDirY == 1)
                {
                    targetHighlight.Aim('u');
                    attackDir = 'u';
                }
            }
            
            if (Input.GetMouseButtonDown(0)) // Attack
            {
                attackWait = false;
                turnEnd = true;
            }
            if (Input.GetMouseButtonDown(1)) // Bail
            {
                attackWait = false;
            }
            if (!attackWait)
            {
                targetHighlight.Deactivate();
            }
        }
    }


    void FixedUpdate() // Where animation and actions take place
    {
        if (attack)
        {
            attack = false;
            attackWait = true;
            targetHighlight.Activate(1, flipped, 'o'); // o = other i.e basic attack
            StartCoroutine(wait2Move('a', 1.5f)); // Starts animation timer and should stop inputs
        }
        else if (specialA)
        {
            int r = 0;
            switch (weaponType)
            {
                case 'b': // bluntslide 180
                    r = 1;
                    break;
                case 's': // slice
                    r = 5;
                    break;
                case 't': // thrust
                    r = 2;
                    break;
            }
            specialA = false;
            attackWait = true;
            targetHighlight.Activate(r, flipped, weaponType);
            StartCoroutine(wait2Move('s', 1.5f)); // Starts animation timer and should stop inputs
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
                Vector2 tempS = transform.localScale;
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
                Vector2 tempS = transform.localScale;
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
            case 'a': //Attack
                {
                    while (attackWait)// Start combat decisions
                        yield return null;
                    if (turnEnd) // if attack actually happened
                    {
                        animator.SetTrigger("Attack");
                        yield return new WaitForSeconds(t);
                        Combat(false); // Once LLB attack animation ends, do damage
                    }
                }
                break;
            case 's': //Special attack
                {
                    
                    while (attackWait)// Start combat decisions
                        yield return null;
                    if (turnEnd) // if attack actually happened
                    {
                        animator.SetTrigger("Attack");
                        yield return new WaitForSeconds(t);
                    }
                }
                break;
            case 'l':  //Move left
                {
                    transform.Translate((Vector2.left) / 2); // Splits the difference so its a 2 step
                    yield return new WaitForSeconds(0.1f);
                    transform.Translate((Vector2.left) / 2);
                    yield return new WaitForSeconds(t - 0.1f); // Lagtime after movement is complete
                    turnEnd = true;
                }

                break;
            case 'r': // Move right
                {
                    transform.Translate((Vector2.right) / 2);
                    yield return new WaitForSeconds(0.1f);
                    transform.Translate((Vector2.right) / 2);
                    yield return new WaitForSeconds(t - 0.1f);
                    turnEnd = true;
                }
                break;
            case 'u': // Move up
                {
                    transform.Translate((Vector2.up) / 2);
                    yield return new WaitForSeconds(0.1f);
                    transform.Translate((Vector2.up) / 2);
                    yield return new WaitForSeconds(t - 0.1f);
                    turnEnd = true;
                }
                break;
            case 'd': // Move Down
                {
                    transform.Translate((Vector2.down) / 2);
                    yield return new WaitForSeconds(0.1f);
                    transform.Translate((Vector2.down) / 2);
                    yield return new WaitForSeconds(t - 0.1f);
                    turnEnd = true;
                }
                break;
            case 'o': //Other, just wait for animation to end
                {
                    yield return new WaitForSeconds(t); // Nothing special just wait t seconds
                    turnEnd = true;
                }
                break;
        }
        //GameManager.instance.playersTurn = false; // Action complete, player loses turn enemies go
        //board.moveEnemies(); // Move all of the bad bois
        if (turnEnd) // Ensures action has been made
        {
            yield return StartCoroutine(board.moveEnemies());
        }
        turnEnd = false; // Reset after enemies
        checkInput = true; // Inputs are able to be taken again
    }
}
