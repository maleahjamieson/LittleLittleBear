using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BasicEntity
{
	//===== Parameters =====//
	ArrayList visitedPoints;
	//Vector2 visited;

	//===== Stats =====//

	//===== Methods =====//
	public Enemy()
	{
		visitedPoints = new ArrayList();
	}

	public bool lineOfSight(BasicEntity entity, GridCell[,] map)
	{
		int sx, sy, gx, gy;
		sx = CurrentX; sy = CurrentY;
		gx = entity.CurrentX; gy = entity.CurrentY;

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

	public void pathfindTowardsEntity(BasicEntity entity, GridCell[,] map)
	{
		// First, update visited points
		// Add current position to visited points
		visitedPoints.Add(Vector2(this.CurrentX, this.CurrentY));

		// Don't store more than 20 visited points
		if (visitedPoints.Count > 20)
			visitedPoints.RemoveAt(0); // Remove oldest point

		// Now, check distance from each position adjacent to entity
		int[4] dist;
		dist[0] = Mathf.sqrt((entity.CurrentX - this.CurrentX)*(entity.CurrentX - this.CurrentX) + (entity.CurrentY - this.CurrentY - 1)*(entity.CurrentY - this.CurrentY - 1));
		dist[1] = Mathf.sqrt((entity.CurrentX - this.CurrentX)*(entity.CurrentX - this.CurrentX) + (entity.CurrentY - this.CurrentY + 1)*(entity.CurrentY - this.CurrentY + 1));
		dist[2] = Mathf.sqrt((entity.CurrentX - this.CurrentX + 1)*(entity.CurrentX - this.CurrentX + 1) + (entity.CurrentY - this.CurrentY)*(entity.CurrentY - this.CurrentY));
		dist[3] = Mathf.sqrt((entity.CurrentX - this.CurrentX - 1)*(entity.CurrentX - this.CurrentX - 1) + (entity.CurrentY - this.CurrentY)*(entity.CurrentY - this.CurrentY));

		// Check for walls
		if (map[this.CurrentX, this.CurrentY-1].tileType == TileSet.WALL)
			dist[0] = 9999;
		if (map[this.CurrentX, this.CurrentY+1].tileType == TileSet.WALL)
			dist[1] = 9999;
		if (map[this.CurrentX+1, this.CurrentY].tileType == TileSet.WALL)
			dist[2] = 9999;
		if (map[this.CurrentX-1, this.CurrentY].tileType == TileSet.WALL)
			dist[3] = 9999;

		// Check that we haven't visited these positions
		if (visitedPoints.Contains(Vector2(this.CurrentX, this.CurrentY-1)))
			dist[0] = 9999;
		if (visitedPoints.Contains(Vector2(this.CurrentX, this.CurrentY+1)))
			dist[1] = 9999;
		if (visitedPoints.Contains(Vector2(this.CurrentX+1, this.CurrentY)))
			dist[2] = 9999;
		if (visitedPoints.Contains(Vector2(this.CurrentX-1, this.CurrentY)))
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
			Move(CurrentX, CurrentY - 1);
			// CurrentY -= 1;
		}
		else if (dist[0] == southDist)
		{
			Move(CurrentX, CurrentY + 1);
			// CurrentY += 1;
		}
		else if (dist[0] == eastDist)
		{
			Move(CurrentX + 1, CurrentY);
			// CurrentX += 1;
		}
		else if (dist[0] == westDist)
		{
			Move(CurrentX - 1, CurrentY);
			// CurrentX -= 1;
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
			tX = Mathf.Abs(entity.CurrentX);
			tY = Mathf.Abs(entity.CurrentY);

			hDist = (int)Mathf.Abs(Mathf.Abs(CurrentX) - tX);
			vDist = (int)Mathf.Abs(Mathf.Abs(CurrentY) - tY);

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
						if (this.CurrentY < entity.CurrentY)
						{
							Move(CurrentX, CurrentY + 1);
							// CurrentY += 1; // Move down
						}
						else if (this.CurrentY > entity.CurrentY)
						{
							Move(CurrentX, CurrentY - 1);
							// CurrentY -= 1; // Move up
						}
						else
						{
							// We're out of range but on the same Y, so try horizontal movement
							if (this.CurrentX < entity.CurrentX)
							{
								Move(CurrentX + 1, CurrentY);
								// CurrentX += 1; // Move east
							}
							else if (this.CurrentX > entity.CurrentX)
							{
								Move(CurrentX - 1, CurrentY);
								// CurrentX -= 1; // Move west
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
							if (this.CurrentY < entity.CurrentY)
							{
								Move(CurrentX, CurrentY + 1);
								// CurrentY += 1; // Move down
							}
							else if (this.CurrentY > entity.CurrentY)
							{
								Move(CurrentX, CurrentY - 1);
								// CurrentY -= 1; // Move up
							}
							else
							{
								// We're in range but on the same Y, so try horizontal movement
								if (this.CurrentX < entity.CurrentX)
								{
									Move(CurrentX + 1, CurrentY);
									// CurrentX += 1; // Move east
								}
								else if (this.CurrentX > entity.CurrentX)
								{
									Move(CurrentX - 1, CurrentY);
									// CurrentX -= 1; // Move west
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
						if (this.CurrentX < entity.CurrentX)
						{
							Move(CurrentX + 1, CurrentY);
							// CurrentX += 1; // move east
						}
						else if (this.CurrentX > entity.CurrentX)
						{
							Move(CurrentX - 1, CurrentY);
							// CurrentX -= 1; // move west
						}
						else
						{
							// We're out of range but on the same x, so try vertical movement
							if (this.CurrentY < entity.CurrentY)
							{
								Move(CurrentX, CurrentY + 1);
								// CurrentY += 1; // move down
							}
							else if (this.CurrentY > entity.CurrentY)
							{
								Move(CurrentX, CurrentY - 1);
								// CurrentY -= 1; // move up
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
							if (this.CurrentX < entity.CurrentX)
							{
								Move(CurrentX + 1, CurrentY);
								// CurrentX += 1; // move east
							}
							else if (this.CurrentX > entity.CurrentX)
							{
								Move(CurrentX - 1, CurrentY);
								// CurrentX -= 1; // move west
							}
							else
							{
								// We're in range but on the same X, so try vertical movement
								if (this.CurrentY < entity.CurrentY)
								{
									Move(CurrentX, CurrentY + 1);
									// CurrentY += 1; // move down
								}
								else if (this.CurrentY > entity.CurrentY)
								{
									Move(CurrentX, CurrentY - 1);
									// CurrentY -= 1; // move up
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
						if (this.CurrentX < entity.CurrentX)
							CurrentX += 1;
						else if (this.CurrentX > entity.CurrentX)
							CurrentX -= 1;
						else
						{
							// We're out of range but on the same x, so try vertical movement
							if (this.CurrentY < entity.CurrentY)
								CurrentY += 1;
							else if (this.CurrentY > entity.CurrentY)
								CurrentY -= 1;
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
							if (this.CurrentX < entity.CurrentX)
								CurrentX += 1;
							else if (this.CurrentX > entity.CurrentX)
								CurrentX -= 1;
							else
							{
								// We're in range but on the same X, so try vertical movement
								if (this.CurrentY < entity.CurrentY)
									CurrentY += 1;
								else if (this.CurrentY > entity.CurrentY)
									CurrentY -= 1;
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
						if (this.CurrentY < entity.CurrentY)
							CurrentY += 1; // Move down
						else if (this.CurrentY > entity.CurrentY)
							CurrentY -= 1; // Move up
						else
						{
							// We're out of range but on the same Y, so try horizontal movement
							if (this.CurrentX < entity.CurrentX)
								CurrentX += 1;
							else if (this.CurrentX > entity.CurrentX)
								CurrentX -= 1;
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
							if (this.CurrentY < entity.CurrentY)
								CurrentY += 1;
							else if (this.CurrentY > entity.CurrentY)
								CurrentY -= 1;
							else
							{
								// We're in range but on the same Y, so try horizontal movement
								if (this.CurrentX < entity.CurrentX)
									CurrentX += 1;
								else if (this.CurrentX > entity.CurrentX)
									CurrentX -= 1;
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

		// First, see if we move at all (1/3 chance)
		if (Random.value < 0.33f)
		{
			// Now, see if we flip the sign to move up/left or down/right
			if (Random.value < 0.5f)
				sign *= -1;

			// Finally, see if we move vertically or horizontally
			if (Random.value < 0.5f)
			{
				Move(CurrentX + sign, CurrentY);
				// CurrentX += sign;
			}
			else
			{
				Move(CurrentX, CurrentY + sign);
				// CurrentY += sign;
			}
		}
	}
}