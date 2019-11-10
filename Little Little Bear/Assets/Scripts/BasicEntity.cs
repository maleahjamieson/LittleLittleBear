using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicEntity : MonoBehaviour
{
    public int health;
    public int strength; // Generic damage for now

    public int CurrentX; // CurrentPosition
    public int CurrentY;

    public int range; // Maximum range of attack

    // List of generic action boolean values
    public bool moveUp;
    public bool moveDown;
    public bool moveLeft;
    public bool moveRight;
    public bool attack;
    public bool flipped;
    public EntitySet selfEntity;
    //
    public BoardGenerator board;




    protected virtual void Start()
    {

        moveUp = false;
        moveDown = false;
        moveLeft = false;
        moveRight = false;
        attack = false;
        flipped = false;

        board = GameObject.Find("LevelTilesGenerator").GetComponent<gameManager>().board;

    }

    protected bool Move(int xDir, int yDir) // out let us return multiple values
    {
        board = GameObject.Find("LevelTilesGenerator").GetComponent<gameManager>().board; //needed here due to dereferencing null
        selfEntity = board.map[CurrentX, CurrentY].entityType;
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
                case TileSet.NOTHING:
                    Debug.Log("CONTAINS " + board.map[xDir, yDir].tileType);
                    //do nothing
                    return true;

                default: // Currently default since moving is only here
                    Debug.Log("MOVING X: " + xDir + " AND Y: " + yDir);
                    Debug.Log("CONTAINS " + board.map[xDir, yDir].tileType);
                    board.map[CurrentX, CurrentY].entityType = EntitySet.NOTHING; // nothing where you where
                    board.map[xDir, yDir].entityType = selfEntity; // you are here now
                    CurrentX = xDir;
                    CurrentY = yDir;
                    return true;


            }
        }
        else //something is there
        {
            Debug.Log("CONTAINS " + board.map[xDir, yDir].entityType);
        }
        
 /*       if (true)
        {
            return true;
        }*/
        return false;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
