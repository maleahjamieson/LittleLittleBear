using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasic : BasicEntity
{
    public enemyType type; // Name like mantis, bear, etc.
    private ArrayList visitedPoints;
    private bool alert;
    private bool waitAttack;

    public enum enemyType : short
    {
        Empty = -1, Mantis = 0, Falcon = 1,
        Bear = 2
    };

    protected override void Start()
    {
        this.alert = false;
        this.waitAttack = false; // Ensures damage flash fully plays
        visitedPoints = new ArrayList();
        // ---- will change when board calls Set() -----
        // ---------------------------------------------
        base.Start(); // initialize values
    }

    private void OnEnable() // When level starts up we enable the player entity
    {
        flipped = false;
    }

    public void Set(enemyType t, int depth)
    {
        this.type = t;
        switch (type)
        {
            case enemyType.Mantis:
                this.animator.SetTrigger("Mantis");
                this.strength = 5 + ((int)(depth / 2));
                this.range = 1;
                this.health = 8 + depth;
                break;
            case enemyType.Falcon:
                this.animator.SetTrigger("Falcon");
                this.strength = 10 + depth;
                this.range = 1;
                this.health = 12 + (depth * 2);
                break;
            case enemyType.Bear:
                this.animator.SetTrigger("Bear");
                break;
            default:
                this.animator.SetTrigger("Mantis");
                this.strength = 1;
                this.range = 1;
                this.health = 1;
                break;
        }
    }
    private IEnumerator Attack()
    {
        BasicEntity LLB = gameManager.FindObjectOfType<LLB>();
        //Debug.Log(this.type + " at x: " + this.currentX + " y: " + this.currentY + " hit LLB who is at x: " + LLB.currentX + " y: " + LLB.currentY + " for: " + this.strength);
        StartCoroutine(LLB.GetComponent<LLB>().Hurt(this.strength, 1)); // Inflict damage
        Debug.Log("LLB HP is now: " + LLB.health);
        yield return new WaitForSeconds(0f);
    }
    /*public void Launched(int distance, char dir) // blunt special move
    {
        for(int i = 0; i < distance; i++)
        {
            switch (dir)
            {
                case 'r':
                    if(Move())
                    break;
                case 'r':
                    break;
                case 'u':
                    break;
                case 'd':
                    break;
            }
        }
    }*/
    protected bool Move(int xDir, int yDir) // out let us return multiple values
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
                case TileSet.TUNNEL:
                case TileSet.END_TILE:
                    Debug.Log("CONTAINS " + board.map[xDir, yDir].tileType);
                    //do nothing
                    return false;

                default: // Currently default since moving is only here
                    // Debug.Log("MOVING X: " + xDir + " AND Y: " + yDir);
                    // Debug.Log("CONTAINS " + board.map[xDir, yDir].tileType);
                
                    int facingVal = this.currentX - xDir; // Moving left or right?
                    board.map[xDir, yDir].entity = board.map[currentX, currentY].entity;
                    board.map[currentX, currentY].entity = null;

                    // board.map[currentX, currentY].entity = null; // nothing where you where
                    // board.map[xDir, yDir].entityType = selfEntity; // you are here now
                    currentX = xDir;
                    currentY = yDir;
                    // transform.position = new Vector3(currentX, currentY, 0);
                    transform.position = new Vector2(currentX, currentY);

                    // Checking if enemy sprite should flip left or right
                    SpriteRenderer mySpriteRenderer = GetComponent<SpriteRenderer>();
                    if (facingVal == -1) // Moving left, face left
                    {
                        mySpriteRenderer.flipX = true;
                    }
                    else if (facingVal == 1) // Moving right, face right
                    {
                        mySpriteRenderer.flipX = false;
                    }

                    if (board.map[currentX, currentY].tileType == TileSet.PIT)
                    {
                        StartCoroutine(Hurt(9999, 1));
                    }

                    return true;
            }
        }
        else //something is there, commented out debug to make it more readable
        {
            //Debug.Log("CONTAINS " + board.map[xDir, yDir].entity);
        }

        return true; // If nothing is hit then assume move
    }

    // Below is the core of the AI behaviors, written by Christopher Walen

    // It uses what I will call a 'one-step A* approach' where it will choose
    // the tile to move to based on the shortest path, and it determines
    // this path by measuring the x-distance and y-distance, and combining
    // this into a heuristic that we can sort by.

    // We do this for the positions above, below, and to the side of every enemy.
    // We then exclude positions depending on whats in them (i.e. is there a wall,
    // another entity, etc.) by pushing them to the 'back of the queue' by marking
    // them as very far away.

    // Then we do a very rudimentary search for the minimum value among those
    // distances, and compare it to the distance we stored for each position.

    // We also make sure that we have a distance more than 1 tile, and a distance
    // thats not our 'very far away' distance before we actually move.

    // When we're not pathing, we're randomly choosing to move north/south/east/west
    // based on a few Random.value comparisons to simulate a x out of 100 chance.

    // There is also a pretty basic Line-Of-Sight method that will actually run
    // LOS checks in the x and y lines, then we do a distance check to make up for
    // not checking perfectly on diagonals.

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

        // Check along the Y for LOS
        while (flag && sy != gy)
        {
            if (map[sx, sy].tileType == TileSet.WALL)
                flag = false;

            if (y_slope > 0)
                sy++;
            else
                sy--;
        }

        // Reset starting y
        sy = currentY;

        // Check along the X for LOS
        while (flag && sx != gx)
        {
            if (map[sx, sy].tileType == TileSet.WALL)
                flag = false;

            if (x_slope > 0)
                sx++;
            else
                sx--;
        }

        // If all else fails, fake close-up LOS with a distance check
        if (flag)
        {
            int sq1 = x_slope*x_slope;
            int sq2 = y_slope*y_slope;
            float dist = Mathf.Sqrt(sq1 + sq2);

            if (dist >= 5.0f)
                flag = false;
        }

        return flag;
    }

    public IEnumerator pathfindTowardsPoint(int x, int y, GridCell[,] map)
    {
        int euclid;

        // On the same x plane, so use the y difference for checking range
        if (this.currentX == x)
        {
            euclid = (int)Mathf.Abs(Mathf.Abs(y) - Mathf.Abs(this.currentY));
        }
        // On the same y plane, so use the x difference for checking range
        else if (this.currentY == y)
        {
            euclid = (int)Mathf.Abs(Mathf.Abs(x) - Mathf.Abs(this.currentX));
        }
        // We're not on the same plane either way, which means we're on the diagonal (so we can't attack anyways)
        else
        {
            euclid = 999;
        }
        // Debug.Log("Euclid: "+ euclid + " " + this.type + "'s range: " + this.range);

        if (euclid <= this.range) // && !diagTrue)
        {
            yield return StartCoroutine(Attack());
            // This return statement prevents the AI from running away
            // after hitting LLB. Please do not comment out this return

            // ...is what I would say if this had not been changed to a coroutine when it had no reason to.
            // Now the whole rest of the method must be changed.
            // return;
        }
        else
        {
            // Debug.Log("G: "+x+", "+y);
            // Debug.Log("Pos: "+this.currentX+", "+this.currentY);

            // First, update visited points
            // Add current position to visited points
            visitedPoints.Add(new Vector2(this.currentX, this.currentY));

            // Don't store more than 20 visited points
            if (visitedPoints.Count > 3) // Reduced memory to 3 positions
                visitedPoints.RemoveAt(0); // Remove oldest point

            // Now, check distance from each position adjacent to entity
            float[] dist = new float[4];
            dist[0] = (int)Mathf.Abs(Mathf.Abs(x) - Mathf.Abs(this.currentX)) + Mathf.Abs(Mathf.Abs(y) - Mathf.Abs(this.currentY - 1));
            dist[1] = (int)Mathf.Abs(Mathf.Abs(x) - Mathf.Abs(this.currentX)) + Mathf.Abs(Mathf.Abs(y) - Mathf.Abs(this.currentY + 1));
            dist[2] = (int)Mathf.Abs(Mathf.Abs(x) - Mathf.Abs(this.currentX + 1)) + Mathf.Abs(Mathf.Abs(y) - Mathf.Abs(this.currentY));
            dist[3] = (int)Mathf.Abs(Mathf.Abs(x) - Mathf.Abs(this.currentX - 1)) + Mathf.Abs(Mathf.Abs(y) - Mathf.Abs(this.currentY));

            // Check for walls
            if (map[this.currentX, this.currentY-1].tileType == TileSet.WALL
                || map[this.currentX, this.currentY-1].tileType == TileSet.TUNNEL
                || map[this.currentX, this.currentY-1].tileType == TileSet.END_TILE)
            {
                // Debug.Log("S-Wall");
                dist[0] = 9999.9f;
            }
            if (map[this.currentX, this.currentY+1].tileType == TileSet.WALL
                || map[this.currentX, this.currentY+1].tileType == TileSet.TUNNEL
                || map[this.currentX, this.currentY+1].tileType == TileSet.END_TILE)
            {
                // Debug.Log("N-Wall");
                dist[1] = 9999.9f;
            }
            if (map[this.currentX+1, this.currentY].tileType == TileSet.WALL
                || map[this.currentX+1, this.currentY].tileType == TileSet.TUNNEL
                || map[this.currentX+1, this.currentY].tileType == TileSet.END_TILE)
            {
                // Debug.Log("E-Wall");
                dist[2] = 9999.9f;
            }
            if (map[this.currentX-1, this.currentY].tileType == TileSet.WALL
                || map[this.currentX-1, this.currentY].tileType == TileSet.TUNNEL
                || map[this.currentX-1, this.currentY].tileType == TileSet.END_TILE)
            {
                // Debug.Log("W-Wall");
                dist[3] = 9999.9f;
            }

            // Check for entities
            if (map[this.currentX, this.currentY-1].entity != null)
            {
                dist[0] = 9999.9f;
            }
            if (map[this.currentX, this.currentY+1].entity != null)
            {
                dist[1] = 9999.9f;
            }
            if (map[this.currentX+1, this.currentY].entity != null)
            {
                dist[2] = 9999.9f;
            }
            if (map[this.currentX-1, this.currentY].entity != null)
            {
                dist[3] = 9999.9f;
            }

            // Check that we haven't visited these positions
            if (visitedPoints.Contains(new Vector2(this.currentX, this.currentY-1)))
                dist[0] = 9999.9f;
            if (visitedPoints.Contains(new Vector2(this.currentX, this.currentY+1)))
                dist[1] = 9999.9f;
            if (visitedPoints.Contains(new Vector2(this.currentX+1, this.currentY)))
                dist[2] = 9999.9f;
            if (visitedPoints.Contains(new Vector2(this.currentX-1, this.currentY)))
                dist[3] = 9999.9f;

            // Store variables to reference later
            float northDist, southDist, eastDist, westDist;
            southDist = dist[0];
            northDist = dist[1];
            eastDist = dist[2];
            westDist = dist[3];

            // Optimized to place smallest distance at dist[0]
            for (int i = 0; i < 4; i++)
            {
                if (dist[i] < dist[0])
                {
                    float temp = dist[i];
                    dist[i] = dist[0];
                    dist[0] = temp;
                }
            }

            // Debug.Log("N: "+northDist+", S: "+southDist+", E: "+eastDist+", W: "+westDist);

            // Actually move based on our findings
            if (dist[0] >= 1.0f && dist[0] < 9999.9f)
            {
                if (dist[0] == southDist)
                {
                    // Debug.Log("Moved south");
                    Move(currentX, currentY - 1);
                    // currentY -= 1;
                }
                else if (dist[0] == northDist)
                {
                    // Debug.Log("Moved north");
                    Move(currentX, currentY + 1);
                    // currentY += 1;
                }
                else if (dist[0] == eastDist)
                {
                    // Debug.Log("Moved east");
                    Move(currentX + 1, currentY);
                    // currentX += 1;
                }
                else if (dist[0] == westDist)
                {
                    // Debug.Log("Moved west");
                    Move(currentX - 1, currentY);
                    // currentX -= 1;
                }
                else
                {
                    Debug.Log("Error: enemy attempted to pathfind with non-existant distance");
                }
            }

            // This is only here because the method was changed to a coroutine.
            yield return new WaitForSeconds(0.00001f);
        }
    }

    // This method may be removed at a later date
    // lol no one can stop me from commenting out 300 lines of my code
    // public void moveTowardsEntity(BasicEntity entity)
    // {
    //     // If the entity we're moving towards exists
    //     if (entity != null)
    //     {
    //         int hDist, vDist;
    //         float tX, tY;
    //         tX = Mathf.Abs(entity.currentX);
    //         tY = Mathf.Abs(entity.currentY);

    //         hDist = (int)Mathf.Abs(Mathf.Abs(currentX) - tX);
    //         vDist = (int)Mathf.Abs(Mathf.Abs(currentY) - tY);

    //         // Move towards the target x and y
    //         // and randomize whether we prioritize vertical or horizontal movement
    //         if (Random.value < 0.5f)
    //         {
    //             Debug.Log("Pathfinding to LLB: Vertical");
    //             // Prioritize vertical movement
    //             if (vDist > hDist)
    //             {
    //                 // Try moving vertically while out of range
    //                 if (vDist > this.range)
    //                 {
    //                     if (this.currentY < entity.currentY)
    //                     {
    //                         Move(currentX, currentY + 1);
    //                         // currentY += 1; // Move down
    //                     }
    //                     else if (this.currentY > entity.currentY)
    //                     {
    //                         Move(currentX, currentY - 1);
    //                         // currentY -= 1; // Move up
    //                     }
    //                     else
    //                     {
    //                         // We're out of range but on the same Y, so try horizontal movement
    //                         if (this.currentX < entity.currentX)
    //                         {
    //                             Move(currentX + 1, currentY);
    //                             // currentX += 1; // Move east
    //                         }
    //                         else if (this.currentX > entity.currentX)
    //                         {
    //                             Move(currentX - 1, currentY);
    //                             // currentX -= 1; // Move west
    //                         }
    //                         else
    //                         {
    //                             // We're at the same position as the other entity but out of range, so its an error
    //                             Debug.Log("Error: matching position of target and out of range");
    //                         }
    //                     }
    //                 }
    //                 // Try moving vertically while in range
    //                 else
    //                 {
    //                     // Try to step closer
    //                     if (Random.value < 0.5f)
    //                     {
    //                         // We're going to step closer
    //                         if (this.currentY < entity.currentY)
    //                         {
    //                             Move(currentX, currentY + 1);
    //                             // currentY += 1; // Move down
    //                         }
    //                         else if (this.currentY > entity.currentY)
    //                         {
    //                             Move(currentX, currentY - 1);
    //                             // currentY -= 1; // Move up
    //                         }
    //                         else
    //                         {
    //                             // We're in range but on the same Y, so try horizontal movement
    //                             if (this.currentX < entity.currentX)
    //                             {
    //                                 Move(currentX + 1, currentY);
    //                                 // currentX += 1; // Move east
    //                             }
    //                             else if (this.currentX > entity.currentX)
    //                             {
    //                                 Move(currentX - 1, currentY);
    //                                 // currentX -= 1; // Move west
    //                             }
    //                             else
    //                             {
    //                                 // We're at the same position as the other entity and in range, so its an error
    //                                 Debug.Log("Error: matching position of target and in range");
    //                             }
    //                         }
    //                     }
    //                     else
    //                     {
    //                         // attack! We're in range
    //                         Attack();
    //                     }
    //                 }
    //             }
    //             else
    //             {
    //                 Debug.Log("Pathfinding to LLB: Horizontal");
    //                 // Try moving horizontally while out of range
    //                 if (hDist > this.range)
    //                 {
    //                     if (this.currentX < entity.currentX)
    //                     {
    //                         Move(currentX + 1, currentY);
    //                         // currentX += 1; // move east
    //                     }
    //                     else if (this.currentX > entity.currentX)
    //                     {
    //                         Move(currentX - 1, currentY);
    //                         // currentX -= 1; // move west
    //                     }
    //                     else
    //                     {
    //                         // We're out of range but on the same x, so try vertical movement
    //                         if (this.currentY < entity.currentY)
    //                         {
    //                             Move(currentX, currentY + 1);
    //                             // currentY += 1; // move down
    //                         }
    //                         else if (this.currentY > entity.currentY)
    //                         {
    //                             Move(currentX, currentY - 1);
    //                             // currentY -= 1; // move up
    //                         }
    //                         else
    //                         {
    //                             // We're at the same position as the target but out of range, so its an error
    //                             Debug.Log("Error: matching position of target and out of range");
    //                         }
    //                     }
    //                 }
    //                 // Try moving horizontally while in range
    //                 else
    //                 {
    //                     // Try to step closer
    //                     if (Random.value < 0.5f)
    //                     {
    //                         // We're going to step closer
    //                         if (this.currentX < entity.currentX)
    //                         {
    //                             Move(currentX + 1, currentY);
    //                             // currentX += 1; // move east
    //                         }
    //                         else if (this.currentX > entity.currentX)
    //                         {
    //                             Move(currentX - 1, currentY);
    //                             // currentX -= 1; // move west
    //                         }
    //                         else
    //                         {
    //                             // We're in range but on the same X, so try vertical movement
    //                             if (this.currentY < entity.currentY)
    //                             {
    //                                 Move(currentX, currentY + 1);
    //                                 // currentY += 1; // move down
    //                             }
    //                             else if (this.currentY > entity.currentY)
    //                             {
    //                                 Move(currentX, currentY - 1);
    //                                 // currentY -= 1; // move up
    //                             }
    //                             else
    //                             {
    //                                 // We're at the same position as the target and in range, so its an error
    //                                 Debug.Log("Error: matching position of target and in range");
    //                             }
    //                         }
    //                     }
    //                     else
    //                     {
    //                         // attack! We're in range
    //                         Attack();
    //                     }
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             // Prioritize horizontal movement
    //             if (hDist > vDist)
    //             {
    //                 // Try moving horizontally while out of range
    //                 if (hDist > this.range)
    //                 {
    //                     if (this.currentX < entity.currentX)
    //                         currentX += 1;
    //                     else if (this.currentX > entity.currentX)
    //                         currentX -= 1;
    //                     else
    //                     {
    //                         // We're out of range but on the same x, so try vertical movement
    //                         if (this.currentY < entity.currentY)
    //                             currentY += 1;
    //                         else if (this.currentY > entity.currentY)
    //                             currentY -= 1;
    //                         else
    //                         {
    //                             // We're at the same position as the target but out of range, so its an error
    //                             Debug.Log("Error: matching position of target and out of range");
    //                         }
    //                     }
    //                 }
    //                 // Try moving horizontally while in range
    //                 else
    //                 {
    //                     // Try to step closer
    //                     if (Random.value < 0.5f)
    //                     {
    //                         // We're going to step closer
    //                         if (this.currentX < entity.currentX)
    //                             currentX += 1;
    //                         else if (this.currentX > entity.currentX)
    //                             currentX -= 1;
    //                         else
    //                         {
    //                             // We're in range but on the same X, so try vertical movement
    //                             if (this.currentY < entity.currentY)
    //                                 currentY += 1;
    //                             else if (this.currentY > entity.currentY)
    //                                 currentY -= 1;
    //                             else
    //                             {
    //                                 // We're at the same position as the target and in range, so its an error
    //                                 Debug.Log("Error: matching position of target and in range");
    //                             }
    //                         }
    //                     }
    //                     else
    //                     {
    //                         // attack! We're in range
    //                         Attack();
    //                     }
    //                 }
    //             }
    //             else
    //             {
    //                 // Try moving vertically while out of range
    //                 if (vDist > this.range)
    //                 {
    //                     if (this.currentY < entity.currentY)
    //                         currentY += 1; // Move down
    //                     else if (this.currentY > entity.currentY)
    //                         currentY -= 1; // Move up
    //                     else
    //                     {
    //                         // We're out of range but on the same Y, so try horizontal movement
    //                         if (this.currentX < entity.currentX)
    //                             currentX += 1;
    //                         else if (this.currentX > entity.currentX)
    //                             currentX -= 1;
    //                         else
    //                         {
    //                             // We're at the same position as the other entity but out of range, so its an error
    //                             Debug.Log("Error: matching position of target and out of range");
    //                         }
    //                     }
    //                 }
    //                 // Try moving vertically while in range
    //                 else
    //                 {
    //                     // Try to step closer
    //                     if (Random.value < 0.5f)
    //                     {
    //                         // We're going to step closer
    //                         if (this.currentY < entity.currentY)
    //                             currentY += 1;
    //                         else if (this.currentY > entity.currentY)
    //                             currentY -= 1;
    //                         else
    //                         {
    //                             // We're in range but on the same Y, so try horizontal movement
    //                             if (this.currentX < entity.currentX)
    //                                 currentX += 1;
    //                             else if (this.currentX > entity.currentX)
    //                                 currentX -= 1;
    //                             else
    //                             {
    //                                 // We're at the same position as the other entity and in range, so its an error
    //                                 Debug.Log("Error: matching position of target and in range");
    //                             }
    //                         }
    //                     }
    //                     else
    //                     {
    //                         // attack! We're in range
    //                         Attack();
    //                     }
    //                 }
    //             }
    //         }
    //     }
    // }

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

        //Debug.Log("Wandering!"); commented out debug to make it more readable
    }
    // End core AI methods

    // Update is called once per frame
    void Update()
    {

    }
}
