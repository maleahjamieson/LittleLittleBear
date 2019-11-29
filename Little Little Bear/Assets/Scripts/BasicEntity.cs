using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicEntity : MonoBehaviour
{
    public int health;
    public int strength; // Generic damage for now

    public float offset;  // Should match level's
    public int currentX; // CurrentPosition
    public int currentY;

    public int range; // Maximum range of attack
    public int stunnedTurns;

    // List of generic action boolean values
    public bool moveUp;
    public bool moveDown;
    public bool moveLeft;
    public bool moveRight;
    public bool active;
    public bool attack;
    public bool flipped;
    public bool stunned;
    public bool flash;
    // public EntitySet selfEntity;
    public BoardGenerator board;
    public Animator animator; //animation controller 

    protected virtual void Start()
    {
        active = true;
        moveUp = false;
        moveDown = false;
        moveLeft = false;
        moveRight = false;
        attack = false;
        flipped = false;
        stunned = false;
        stunnedTurns = 0;
        board = GameObject.Find("LevelTilesGenerator").GetComponent<gameManager>().board;
        offset = board.offsetforTiles;
        animator = GetComponent<Animator>();
        GetComponent<SpriteRenderer>().sortingOrder = 2;
    }
    public IEnumerator Hurt(int damage, int hits)
    {
        flash = true;
        Debug.Log("-------------------------------------------------------------\nDEALT DAMAGE OF " + damage);
        for (int i = 0; i < hits; i++)
        {
            this.health -= damage;
            GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.5f);
            if (this.health <= 0) // Kill enemy by destroying it from the board
            {
                Debug.Log("DEACTIVATED");
                this.active = false; // Keeps enemy from being active after death
                this.gameObject.SetActive(false);               
                // UnityEngine.Object.Destroy(board.map[this.currentX, this.currentY].entity);
                board.map[this.currentX, this.currentY].entity = null;
                break;
            }
            GetComponent<SpriteRenderer>().color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }
        
        flash = false;
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }
}
