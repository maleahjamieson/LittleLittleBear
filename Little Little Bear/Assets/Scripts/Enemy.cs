using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BasicEntity
{
	//===== Parameters =====//

	//===== Stats =====//

	//===== Methods =====//
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
				else
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
				CurrentX += sign;
			else
				CurrentY += sign;
		}
	}
}