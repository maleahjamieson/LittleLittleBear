using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicEntity : MonoBehaviour
{
    public int health;
    public int strength; // Generic damage for now
<<<<<<< Updated upstream
=======
    public float offset;  // Should match level's
    public int currentX; // CurrentPosition
    public int currentY;
>>>>>>> Stashed changes

    // List of generic action boolean values
    public bool moveUp;
    public bool moveDown;
    public bool moveLeft;
    public bool moveRight;
    public bool attack;
    public bool flipped;
<<<<<<< Updated upstream
    //
=======
    public EntitySet selfEntity;
    public BoardGenerator board;



>>>>>>> Stashed changes

    protected virtual void Start()
    {
        moveUp = false;
        moveDown = false;
        moveLeft = false;
        moveRight = false;
        attack = false;
        flipped = false;
<<<<<<< Updated upstream
=======
        board = GameObject.Find("LevelTilesGenerator").GetComponent<gameManager>().board;
        offset = board.offsetforTiles;
>>>>>>> Stashed changes
    }

    protected bool Move(int xDir, int yDir) // out let us return multiple values
    {
<<<<<<< Updated upstream
        Vector2 sPos = transform.position; //Start Position
        Vector2 ePos = sPos + new Vector2(xDir, yDir); // End Position
        // if (checkBoardMove(ePos)){}
        
        if (true)
        {
            return true;
        }
        return false;
    }
    
    // Update is called once per frame
    void Update()
    {
        
=======
        board = GameObject.Find("LevelTilesGenerator").GetComponent<gameManager>().board; //needed here due to dereferencing null
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

        
            return true;
>>>>>>> Stashed changes
    }
}