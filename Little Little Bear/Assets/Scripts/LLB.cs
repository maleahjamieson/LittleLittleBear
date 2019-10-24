using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LLB : BasicEntity
{
    private Animator animator;
    private int horizontal = 0; //store direction we are moving
    private int vertical = 0;
    private float moveTime = 0.15f;

    // List of personal boolean
    private bool turnEnd;
    private bool dig;
    private bool checkInput;

    protected override void Start()
    {        
        turnEnd = false;
        dig = false;
        checkInput = true;
        health = 100;
        strength = 4;
        animator = GetComponent<Animator>();
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
    // Update is called once per frame

    private void Update()
    {
        //if (!GameManager.instance.playersTurn) return; // means following code wont run unless its a turn
        
        if (checkInput) //No previous actions are being executed
        {
            if (Input.GetMouseButtonDown(0))
            {
                attack = true;
                checkInput = false;
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                dig = true;
                checkInput = false; //input read
            }

            horizontal = (int)Input.GetAxisRaw("Horizontal"); //using ints forces 1 unit movement
            vertical = (int)Input.GetAxisRaw("Vertical");

            if (horizontal != 0) //No diagonal
                vertical = 0;

            if (horizontal != 0 || vertical != 0)
            {
                checkInput = false;
                if (Move(horizontal, vertical))
                {
                    if (horizontal == -1)
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
    void FixedUpdate()
    {
        //if (!GameManager.instance.playersTurn) return; // means following code wont run unless its a turn

        if (attack)
        {
            attack = false;
            animator.SetTrigger("Attack");
            StartCoroutine(wait2Move('o', 0.1f));
        }
        else if (dig)
        {
            dig = false;
            animator.SetTrigger("Dig");
            StartCoroutine(wait2Move('o', 1.5f));
        }
        else if (moveLeft)
        {
            moveLeft = false;
            if (!flipped)
            {
                Vector3 tempS = transform.localScale;
                tempS.x *= -1;  // Flips sprite
                transform.localScale = tempS;
                flipped = true;
            }
            animator.SetTrigger("WalkX");
            StartCoroutine(wait2Move('l', 0.33f)); //Left
        }
        else if (moveRight)
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
            StartCoroutine(wait2Move('r', 0.33f));
        }
        else if (moveUp)
        {
            moveUp = false;
            StartCoroutine(wait2Move('u', 0.33f));
        }
        else if (moveDown)
        {
            moveDown = false;
            StartCoroutine(wait2Move('d', 0.33f));
        }

        horizontal = 0;
        vertical = 0;
    }
    // Yes these IEnumerators are sloppy, this will be converged into a single IEnumerator soon
    private IEnumerator wait2Move(char c, float t)
    {
        
        switch (c)
        {
            case 'a': //attack
                {
                    if (!flipped)
                    {
                        transform.Translate((Vector2.left) / 2);
                        yield return new WaitForSeconds(t);
                        transform.Translate((Vector2.right) / 2);
                    }
                    else
                    {
                        transform.Translate((Vector2.right) / 2);
                        yield return new WaitForSeconds(t);
                        transform.Translate((Vector2.left) / 2);
                    }
                }
                break;
            case 'l':
                {
                    transform.Translate((Vector2.left)/2);
                    yield return new WaitForSeconds(t);
                    transform.Translate((Vector2.left) / 2);
                }
                
                break;
            case 'r':
                {
                    transform.Translate((Vector2.right) / 2);
                    yield return new WaitForSeconds(t);
                    transform.Translate((Vector2.right) / 2);
                }
                break;
            case 'u':
                {
                    transform.Translate((Vector2.up) / 2);
                    yield return new WaitForSeconds(t);
                    transform.Translate((Vector2.up) / 2);
                }
                break;
            case 'd':
                {
                    transform.Translate((Vector2.down) / 2);
                    yield return new WaitForSeconds(t);
                    transform.Translate((Vector2.down) / 2);
                }
                break;
            case 'o': //Other, just wait for animation to end
                {
                    yield return new WaitForSeconds(t);
                }
                break;
        }
        //GameManager.instance.playersTurn = false;
        checkInput = true;
    }
    private IEnumerator moveL(float t)
    {
        yield return new WaitForSeconds(t);
        transform.Translate(Vector2.left);
       // GameManager.instance.playersTurn = false;
        checkInput = true;
    }

}


