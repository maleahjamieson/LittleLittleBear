using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasic : BasicEntity
{
    public int enemyTag; // int value for queue
    public enemyType type; // Name like mantis, bear, etc.
    private ArrayList visitedPoints;
    private bool alert;

    public enum enemyType : short
    {
        Empty = -1, Mantis = 0, Bear = 1,
        Bird
    };

    protected override void Start()
    {
        alert = false;
        visitedPoints = new ArrayList();
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
                    transform.position = new Vector3(currentX, currentY, 0);
                    return true;
            }
        }
        else //something is there
        {
            Debug.Log("CONTAINS " + board.map[xDir, yDir].entityType);
        }

        return true; // If nothing is hit then assume move
    }

    public bool isAlert()
    {
        return this.alert;
    }

    public void makeAlert()
    {
        this.alert = true;
    }

    public bool lineOfSight(BasicEntity entity, GridCell[,] map)
    {
        int sx, sy, gx, gy;
        sx = currentX; sy = currentY;
        gx = entity.currentX; gy = entity.currentY;

        int x_slope = (int) (gx - sx);
        int y_slope = (int) (gy - sy);
        bool flag = true;

        while (flag && (sx != gx && sy != gy))
        {
            if (map[sx, sy].tileType == TileSet.WALL)
                flag = false;

            if (x_slope > 0)
                sx++;
            else
                sx--;

            if (y_slope > 0)
                sy++;
            else
                sy--;
        }

        return flag;
    }

    public void pathfindTowardsPoint(int x, int y, GridCell[,] map)
    {
        // First, update visited points
        // Add current position to visited points
        visitedPoints.Add(new Vector2(this.currentX, this.currentY));

        // Don't store more than 20 visited points
        if (visitedPoints.Count > 20)
            visitedPoints.RemoveAt(0); // Remove oldest point

        // Now, check distance from each position adjacent to entity
        int[] dist = new int[4];
        dist[0] = (int)Mathf.Sqrt((x - this.currentX)*(x - this.currentX) + (y - this.currentY - 1)*(y - this.currentY - 1));
        dist[1] = (int)Mathf.Sqrt((x - this.currentX)*(x - this.currentX) + (y - this.currentY + 1)*(y - this.currentY + 1));
        dist[2] = (int)Mathf.Sqrt((x - this.currentX + 1)*(x - this.currentX + 1) + (y - this.currentY)*(y - this.currentY));
        dist[3] = (int)Mathf.Sqrt((x - this.currentX - 1)*(x - this.currentX - 1) + (y - this.currentY)*(y - this.currentY));

        // Check for walls
        if (map[this.currentX, this.currentY-1].tileType == TileSet.WALL)
            dist[0] = 9999;
        if (map[this.currentX, this.currentY+1].tileType == TileSet.WALL)
            dist[1] = 9999;
        if (map[this.currentX+1, this.currentY].tileType == TileSet.WALL)
            dist[2] = 9999;
        if (map[this.currentX-1, this.currentY].tileType == TileSet.WALL)
            dist[3] = 9999;

        // Check that we haven't visited these positions
        if (visitedPoints.Contains(new Vector2(this.currentX, this.currentY-1)))
            dist[0] = 9999;
        if (visitedPoints.Contains(new Vector2(this.currentX, this.currentY+1)))
            dist[1] = 9999;
        if (visitedPoints.Contains(new Vector2(this.currentX+1, this.currentY)))
            dist[2] = 9999;
        if (visitedPoints.Contains(new Vector2(this.currentX-1, this.currentY)))
            dist[3] = 9999;

        // Store variables to reference later
        int northDist, southDist, eastDist, westDist;
        northDist = dist[0];
        southDist = dist[1];
        eastDist = dist[2];
        westDist = dist[3];

        // Sort to be the lowest
        for (int i = 0; i < 4; i++)
        {
            for (int j = i; j < 4; j++)
            {
                if (dist[j] < dist[i])
                {
                    int temp = dist[j];
                    dist[j] = dist[i];
                    dist[i] = temp;
                }
            }
        }

        // Actually move based on our findings
        if (dist[0] == northDist)
        {
            Move(currentX, currentY - 1);
            // currentY -= 1;
        }
        else if (dist[0] == southDist)
        {
            Move(currentX, currentY + 1);
            // currentY += 1;
        }
        else if (dist[0] == eastDist)
        {
            Move(currentX + 1, currentY);
            // currentX += 1;
        }
        else if (dist[0] == westDist)
        {
            Move(currentX - 1, currentY);
            // currentX -= 1;
        }
        else
        {
            Debug.Log("Error: enemy attempted to pathfind with non-existant distance");
        }
    }

    public void moveTowardsEntity(BasicEntity entity)
    {
        // If the entity we're moving towards exists
        // if (GameObject.Find(entity) != null)
        {
            int hDist, vDist;
            float tX, tY;
            tX = Mathf.Abs(entity.currentX);
            tY = Mathf.Abs(entity.currentY);

            hDist = (int)Mathf.Abs(Mathf.Abs(currentX) - tX);
            vDist = (int)Mathf.Abs(Mathf.Abs(currentY) - tY);

            // Move towards the target x and y
            // and randomize whether we prioritize vertical or horizontal movement
            if (Random.value < 0.5f)
            {
                // Prioritize vertical movement
                if (vDist > hDist)
                {
                    // Try moving vertically while out of range
                    if (vDist > this.range)
                    {
                        if (this.currentY < entity.currentY)
                        {
                            Move(currentX, currentY + 1);
                            // currentY += 1; // Move down
                        }
                        else if (this.currentY > entity.currentY)
                        {
                            Move(currentX, currentY - 1);
                            // currentY -= 1; // Move up
                        }
                        else
                        {
                            // We're out of range but on the same Y, so try horizontal movement
                            if (this.currentX < entity.currentX)
                            {
                                Move(currentX + 1, currentY);
                                // currentX += 1; // Move east
                            }
                            else if (this.currentX > entity.currentX)
                            {
                                Move(currentX - 1, currentY);
                                // currentX -= 1; // Move west
                            }
                            else
                            {
                                // We're at the same position as the other entity but out of range, so its an error
                                Debug.Log("Error: matching position of target and out of range");
                            }
                        }
                    }
                    // Try moving vertically while in range
                    else
                    {
                        // Try to step closer
                        if (Random.value < 0.5f)
                        {
                            // We're going to step closer
                            if (this.currentY < entity.currentY)
                            {
                                Move(currentX, currentY + 1);
                                // currentY += 1; // Move down
                            }
                            else if (this.currentY > entity.currentY)
                            {
                                Move(currentX, currentY - 1);
                                // currentY -= 1; // Move up
                            }
                            else
                            {
                                // We're in range but on the same Y, so try horizontal movement
                                if (this.currentX < entity.currentX)
                                {
                                    Move(currentX + 1, currentY);
                                    // currentX += 1; // Move east
                                }
                                else if (this.currentX > entity.currentX)
                                {
                                    Move(currentX - 1, currentY);
                                    // currentX -= 1; // Move west
                                }
                                else
                                {
                                    // We're at the same position as the other entity and in range, so its an error
                                    Debug.Log("Error: matching position of target and in range");
                                }
                            }
                        }
                        else
                        {
                            // attack! We're in range
                            Debug.Log("TODO: Enemy Attack");
                        }
                    }
                }
                else
                {
                    // Try moving horizontally while out of range
                    if (hDist > this.range)
                    {
                        if (this.currentX < entity.currentX)
                        {
                            Move(currentX + 1, currentY);
                            // currentX += 1; // move east
                        }
                        else if (this.currentX > entity.currentX)
                        {
                            Move(currentX - 1, currentY);
                            // currentX -= 1; // move west
                        }
                        else
                        {
                            // We're out of range but on the same x, so try vertical movement
                            if (this.currentY < entity.currentY)
                            {
                                Move(currentX, currentY + 1);
                                // currentY += 1; // move down
                            }
                            else if (this.currentY > entity.currentY)
                            {
                                Move(currentX, currentY - 1);
                                // currentY -= 1; // move up
                            }
                            else
                            {
                                // We're at the same position as the target but out of range, so its an error
                                Debug.Log("Error: matching position of target and out of range");
                            }
                        }
                    }
                    // Try moving horizontally while in range
                    else
                    {
                        // Try to step closer
                        if (Random.value < 0.5f)
                        {
                            // We're going to step closer
                            if (this.currentX < entity.currentX)
                            {
                                Move(currentX + 1, currentY);
                                // currentX += 1; // move east
                            }
                            else if (this.currentX > entity.currentX)
                            {
                                Move(currentX - 1, currentY);
                                // currentX -= 1; // move west
                            }
                            else
                            {
                                // We're in range but on the same X, so try vertical movement
                                if (this.currentY < entity.currentY)
                                {
                                    Move(currentX, currentY + 1);
                                    // currentY += 1; // move down
                                }
                                else if (this.currentY > entity.currentY)
                                {
                                    Move(currentX, currentY - 1);
                                    // currentY -= 1; // move up
                                }
                                else
                                {
                                    // We're at the same position as the target and in range, so its an error
                                    Debug.Log("Error: matching position of target and in range");
                                }
                            }
                        }
                        else
                        {
                            // attack! We're in range
                            Debug.Log("TODO: Enemy Attack");
                        }
                    }
                }
            }
            else
            {
                // Prioritize horizontal movement
                if (hDist > vDist)
                {
                    // Try moving horizontally while out of range
                    if (hDist > this.range)
                    {
                        if (this.currentX < entity.currentX)
                            currentX += 1;
                        else if (this.currentX > entity.currentX)
                            currentX -= 1;
                        else
                        {
                            // We're out of range but on the same x, so try vertical movement
                            if (this.currentY < entity.currentY)
                                currentY += 1;
                            else if (this.currentY > entity.currentY)
                                currentY -= 1;
                            else
                            {
                                // We're at the same position as the target but out of range, so its an error
                                Debug.Log("Error: matching position of target and out of range");
                            }
                        }
                    }
                    // Try moving horizontally while in range
                    else
                    {
                        // Try to step closer
                        if (Random.value < 0.5f)
                        {
                            // We're going to step closer
                            if (this.currentX < entity.currentX)
                                currentX += 1;
                            else if (this.currentX > entity.currentX)
                                currentX -= 1;
                            else
                            {
                                // We're in range but on the same X, so try vertical movement
                                if (this.currentY < entity.currentY)
                                    currentY += 1;
                                else if (this.currentY > entity.currentY)
                                    currentY -= 1;
                                else
                                {
                                    // We're at the same position as the target and in range, so its an error
                                    Debug.Log("Error: matching position of target and in range");
                                }
                            }
                        }
                        else
                        {
                            // attack! We're in range
                            Debug.Log("TODO: Enemy Attack");
                        }
                    }
                }
                else
                {
                    // Try moving vertically while out of range
                    if (vDist > this.range)
                    {
                        if (this.currentY < entity.currentY)
                            currentY += 1; // Move down
                        else if (this.currentY > entity.currentY)
                            currentY -= 1; // Move up
                        else
                        {
                            // We're out of range but on the same Y, so try horizontal movement
                            if (this.currentX < entity.currentX)
                                currentX += 1;
                            else if (this.currentX > entity.currentX)
                                currentX -= 1;
                            else
                            {
                                // We're at the same position as the other entity but out of range, so its an error
                                Debug.Log("Error: matching position of target and out of range");
                            }
                        }
                    }
                    // Try moving vertically while in range
                    else
                    {
                        // Try to step closer
                        if (Random.value < 0.5f)
                        {
                            // We're going to step closer
                            if (this.currentY < entity.currentY)
                                currentY += 1;
                            else if (this.currentY > entity.currentY)
                                currentY -= 1;
                            else
                            {
                                // We're in range but on the same Y, so try horizontal movement
                                if (this.currentX < entity.currentX)
                                    currentX += 1;
                                else if (this.currentX > entity.currentX)
                                    currentX -= 1;
                                else
                                {
                                    // We're at the same position as the other entity and in range, so its an error
                                    Debug.Log("Error: matching position of target and in range");
                                }
                            }
                        }
                        else
                        {
                            // attack! We're in range
                            Debug.Log("TODO: Enemy Attack");
                        }
                    }
                }
            }
        }
    }

    public void wander()
    {
        int sign = 1;

        // First, see if we move at all (2/3 chance)
        if (Random.value < 0.66f)
        {
            // Now, see if we flip the sign to move up/left or down/right
            if (Random.value < 0.5f)
                sign *= -1;

            // Finally, see if we move vertically or horizontally
            if (Random.value < 0.5f)
            {
                Move(currentX + sign, currentY);
                // currentX += sign;
            }
            else
            {
                Move(currentX, currentY + sign);
                // currentY += sign;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
