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

    // List of generic action boolean values
    public bool moveUp;
    public bool moveDown;
    public bool moveLeft;
    public bool moveRight;
    public bool attack;
    public bool flipped;
    // public EntitySet selfEntity;
    public BoardGenerator board;
    public Animator animator; //animation controller 

    protected virtual void Start()
    {
        moveUp = false;
        moveDown = false;
        moveLeft = false;
        moveRight = false;
        attack = false;
        flipped = false;
        board = GameObject.Find("LevelTilesGenerator").GetComponent<gameManager>().board;
        offset = board.offsetforTiles;
        animator = GetComponent<Animator>();
        GetComponent<SpriteRenderer>().sortingOrder = 2;
    }
    public void Hurt(int damage)
    {
        Debug.Log("-------------------------------------------------------------\nDEALT DAMAGE OF " + damage);
        this.health -= damage;
        
        StartCoroutine(damageFlash());

        // Kill enemy by destroying it from the board
        if (this.health <= 0)
            UnityEngine.Object.Destroy(board.map[this.currentX, this.currentY].entity);
    }
    IEnumerator damageFlash()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.5f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
