using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasic : BasicEntity
{
    public int enemyTag; // int value for queue
    public enemyType type; // Name like mantis, bear, etc.


    public enum enemyType : short
    {
        Empty = -1, Mantis = 0, Bear = 1,
        Bird
    };

    protected override void Start()
    {
        // ---- will change when board calls Set() -----
        enemyTag = -1; // init number
        type = enemyType.Empty;
        // ---------------------------------------------
        base.Start(); // initialize values

        // ---------------REMOVE LATER -----------------
        Set(0, enemyType.Mantis); //Will be set inside gen, only here for testing
        // ---------------------------------------------
    }
    private void OnEnable() // When level starts up we enable the player entity
    {
        flipped = false;
    }
    protected void Set(int tag, enemyType t)
    {
        enemyTag = tag;
        type = t;
        switch (type)
        {
            case enemyType.Mantis:
                animator.SetTrigger("Mantis");
                break;
            case enemyType.Bear:
                animator.SetTrigger("Bear");
                break;

        }
    }
    protected bool Move(int xDir, int yDir) // out let us return multiple values
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
    // Update is called once per frame
    void Update()
    {

    }
}
