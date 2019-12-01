// This Generator was written by: Christopher Walen
//
// This generator uses a 'drunken walker' style to place hallways and rooms starting
// from the center of the playable area. It is all stored into a 2D array of GridCells,
// which are defined below.
//
// The GridCells have three layers: a tile layer, an item layer, and an entity layer.
// This allows for the player/enemies/etc. to move along the entity layer, making sure
// that the tile layer doesn't have something blocking them, and it allows for entities
// to walk over items so that they don't get erased in the process.
//
// This generator was originally made for the class COP 4331, where Project Group 11
// chose the Roguelike/Roguelite project and decided on the Little-Little-Bear theme.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//**********************************//
//     Specific Data Structures     //
//**********************************//
public enum TileSet : short
{
	NOTHING = -1, FLOOR = 0,
	WALL, HALLWAY, TRAP, SPAWNER, SECRET_FLOOR,
	PUZZLE_FLOOR, ROCK, MUD, BOULDER, DIG_TILE,
	PUZZLE_HALLWAY, START_TILE, END_TILE, TUNNEL, BRANCH,
	PIT, KEY_TILE
};

public enum BiomeSet : short
{
	NOTHING = -1, FOREST, SWAMP, CAVE
};

public enum ItemSet : short
{
    NOTHING = 0, ANT = 1,      // 0:Empty  1:Ants in a bottle
    SKUNK = 2, SNAP = 3,        // 2:SkunkBottle    3:Snaps
    PSNAP = 4, THORN = 5,        // 4:Puzzle Snap    5:Thorn Vines
    BERRY = 6, SEED = 7         // 6:Blue Berries   7:Sunflower Seeds
};

public enum Direction
{
	ERROR = -1, NORTH = 0, SOUTH, EAST, WEST
};

public struct Position
{
	public int x;
	public int y;
	public Direction dir;

	public Position(int xx, int yy)
	{
		x = xx;
		y = yy;
		dir = Direction.ERROR;
	}

	public Position(int xx, int yy, Direction d)
	{
		x = xx;
		y = yy;
		dir = d;
	}
}

public struct GridCell
{
	public GameObject worldTile;
	public TileSet tileType;
    public GameObject entity;
    public GameObject item;

    public GridCell(TileSet t)
    {
    	tileType = t;
    	worldTile = null;
    	entity = null;
    	item = null;
    }

    public GridCell(TileSet t, GameObject e)
    {
    	tileType = t;
    	entity = e;
    	worldTile = null;
    	item = null;
    }

    public GridCell(TileSet t, GameObject e, GameObject i)
    {
    	tileType = t;
    	entity = e;
    	item = i;
    	worldTile = null;
    }
}

//**************************************************//
// The map array shall be accessed in this fashion: //
//        map[x, y]                                 //
//**************************************************//
public class BoardGenerator : MonoBehaviour
{
    //**********************************//
    //      Parameters & Variables      //	
    //**********************************//
	public GameObject[] spawnableItems = new GameObject[9];

    //=============//
    //   Sprites   //
    //=============//
    private BiomeSet biome;

    // Forest Sprites
    public Sprite spr_ForestFloor;
    public Sprite spr_ForestWall;
    public Sprite spr_ForestBranch;
    public Sprite spr_ForestPit;
    public Sprite spr_ForestPuzzleFloor;
    public Sprite spr_ForestTunnel;
    public Sprite spr_ForestDig;
    public Sprite spr_ForestHallway;
    public Sprite spr_ForestStart;
    public Sprite spr_ForestEnd;
    public Sprite spr_ForestSpawner;
    public Sprite spr_ForestSecretFloor;
    public Sprite spr_ForestTrap;
    public Sprite spr_ForestPuzzleHallway;
    public Sprite spr_ForestKeyTile;

    // Swamp Sprites
    public Sprite spr_SwampFloor;
    public Sprite spr_SwampWall;
    public Sprite spr_SwampMud;
    public Sprite spr_SwampPuzzleFloor;
    public Sprite spr_SwampTunnel;
    public Sprite spr_SwampDig;
    public Sprite spr_SwampHallway;
    public Sprite spr_SwampStart;
    public Sprite spr_SwampEnd;
    public Sprite spr_SwampSpawner;
    public Sprite spr_SwampSecretFloor;
    public Sprite spr_SwampTrap;
    public Sprite spr_SwampPuzzleHallway;
    public Sprite spr_SwampKeyTile;
    public Sprite spr_SwampRock;

    // Cave Sprites
    public Sprite spr_CaveFloor;
    public Sprite spr_CaveWall;
    public Sprite spr_CavePuzzleFloor;
    public Sprite spr_CaveTunnel;
    public Sprite spr_CaveDig;
    public Sprite spr_CaveHallway;
    public Sprite spr_CaveStart;
    public Sprite spr_CaveEnd;
    public Sprite spr_CaveSpawner;
    public Sprite spr_CaveSecretFloor;
    public Sprite spr_CaveTrap;
    public Sprite spr_CavePuzzleHallway;
    public Sprite spr_CaveKeyTile;
    public Sprite spr_CaveBoulder;

    // Item Sprites
    public Sprite spr_RedAntsBottle;
    public Sprite spr_Blueberries;
    public Sprite spr_Pocketknife;
    public Sprite spr_Rapier;
    public Sprite spr_SkunkGas;
    public Sprite spr_Snaps;
    public Sprite spr_StickRock;
    public Sprite spr_SunflowerSeed;
    public Sprite spr_ThornVine;
    public Sprite spr_Carrot;
    public Sprite spr_Treat;
    public Sprite spr_Key;

    public GameObject HamsterEntity; // LLB basically
    public ArrayList EnemyList;

    public float offsetforTiles = 1f;
    public bool enemyActive = false; // to keep coroutine organized
    // Generation Parameters
    private int board_width, board_height;
	private int min_hallway_length, max_hallway_length;
	private bool hallwayTurns;
	private bool centeredRooms;
	private int min_tunnel_length, max_tunnel_length;
	private int min_tunnel_width, max_tunnel_width;
	private int min_room_width, max_room_width;
	private int min_room_height, max_room_height;
	private int min_secret_room_width, max_secret_room_width;
	private int min_secret_room_height, max_secret_room_height;
	private int min_room_count, max_room_count;
	private int min_secret_room_count, max_secret_room_count;
	private bool createdPuzzleRoom;
	private int offshoot_chance, secret_chance, tunnel_chance, puzzle_chance;
	private int direction_chance, trap_chance, spawn_chance, dig_chance;
	private int general_item_chance, rare_item_chance;
	private int dungeonDepth;
	private Position puzzlePos;
	private GridCell[,] puzzle;
	public GridCell[,] map;

	// Some miscellaneous stats for record-keeping and num-checks
	private int tileCounter;
	private int wallCounter;
	private int trapCounter;
	private int spawnCounter;
	private int itemCounter;
	private int digCounter;

	private int offshootCounter;
	private int puzzleCounter;
	private int secretCounter;
	private int tunnelCounter;
	private int roomCounter;
	private int hallwayCounter;

	//**********************************//
	//           Constructors           //
	//**********************************//
	BoardGenerator()
	{
		setBoardSize(2500, 2500);
		// Random.seed = System.DateTime.Now.Millisecond;
		this.EnemyList = new ArrayList();
		this.biome = BiomeSet.FOREST;

		// Default parameters here
		setDefaultParameters();
	}

	public BoardGenerator(int width, int height)
	{
		setBoardSize(width, height);
		// Random.seed = System.DateTime.Now.Millisecond;
		this.EnemyList = new ArrayList();
		this.biome = BiomeSet.FOREST;

		// Default parameters here
		setDefaultParameters();
	}

	BoardGenerator(int width, int height, int depth)
	{
		setBoardSize(width, height);
		setDefaultParameters();
		// Random.seed = System.DateTime.Now.Millisecond;
		this.EnemyList = new ArrayList();
		this.biome = BiomeSet.FOREST;

		setDungeonDepth(depth); // Overwrite default depth
	}

	//*********************************//
	//        Parameter Setters        //
	//*********************************//
	public void setBoardSize(int width, int height)
	{
		this.board_width = width;
		this.board_height = height;
	}

	public void setBoardParams(int minRooms, int maxRooms, int minSecretRooms, int maxSecretRooms)
	{
		this.min_room_count = minRooms;
		this.max_room_count = maxRooms;
		this.min_secret_room_count = minSecretRooms;
		this.max_secret_room_count = maxSecretRooms;
	}

	public void setRoomParams(int minWidth, int maxWidth, int minHeight, int maxHeight, bool centered)
	{
		this.min_room_width = minWidth;
		this.min_room_height = minHeight;
		this.max_room_width = maxWidth;
		this.max_room_height = maxHeight;
		this.centeredRooms = centered;
	}

	public void setSecretRoomParams(int minWidth, int maxWidth, int minHeight, int maxHeight)
	{
		this.min_secret_room_width = minWidth;
		this.max_secret_room_width = maxWidth;
		this.min_secret_room_height = minHeight;
		this.max_secret_room_height = maxHeight;
	}	

	public void setHallwayParams(int minLength, int maxLength, bool noTurns)
	{
		this.min_hallway_length = minLength;
		this.max_hallway_length = maxLength;
		this.hallwayTurns = !noTurns;
	}

	public void setTunnelParams(int minLength, int maxLength, int minWidth, int maxWidth)
	{
		this.min_tunnel_length = minLength;
		this.max_tunnel_length = maxLength;
		this.min_tunnel_width = minWidth;
		this.max_tunnel_width = maxWidth;
	}

	public void setOffshootParams(int offshoot, int secret, int tunnel, int puzzle)
	{
		this.offshoot_chance = offshoot;
		this.secret_chance = secret;
		this.tunnel_chance = tunnel;
		this.puzzle_chance = puzzle;
	}

	public void setTileParams(int trapChance, int spawnChance, int digChance)
	{
		this.trap_chance = trapChance;
		this.spawn_chance = spawnChance;
		this.dig_chance = digChance;
	}

	public void setMiscParams(int dirChance, int itemChance, int rareItemChance)
	{
		this.direction_chance = dirChance;
		this.general_item_chance = itemChance;
		this.rare_item_chance = rareItemChance;
	}

	public void setDungeonDepth(int depth)
	{
		this.dungeonDepth = depth;

		updateDungeonDepth();
	}

	public void setDefaultParameters()
	{
		setBoardParams(7, 12, 1, 999);
		setRoomParams(5, 12, 5, 12, true); // Previously, (5, 10, 5, 10, true);
		setSecretRoomParams(4, 7, 4, 7);
		setHallwayParams(6, 12, false);
		setTunnelParams(6, 12, 1, 5);
		setOffshootParams(5, 15, 5, 30);
		setTileParams(50, 50, 50);
		setMiscParams(40, 70, 100);
		setDungeonDepth(1); // This needs to go last!
	}

	public void updateDefaultParameters()
	{
		setBoardParams(7 + this.dungeonDepth, 12 + this.dungeonDepth, 1 + this.dungeonDepth, 999);
		setRoomParams(5 + this.dungeonDepth, 12 + this.dungeonDepth, 5 + this.dungeonDepth, 12 + this.dungeonDepth, true); // Previously, (5, 10, 5, 10, true);
		setSecretRoomParams(4 + this.dungeonDepth, 7 + this.dungeonDepth, 4 + this.dungeonDepth, 7 + this.dungeonDepth);
		setHallwayParams(6 + this.dungeonDepth, 12 + this.dungeonDepth, false);
		setTunnelParams(6 + this.dungeonDepth, 12 + this.dungeonDepth, 1 + this.dungeonDepth, 5 + this.dungeonDepth);
		setOffshootParams(5 + this.dungeonDepth, 15 + this.dungeonDepth, 5 + this.dungeonDepth, 30);
		setTileParams(50, 50, 50);
		setMiscParams(40, 70, 100);
	}

	public void updateDungeonDepth()
	{
		// First, change the biome if we need to
		if (this.dungeonDepth <= 3)
			this.biome = BiomeSet.FOREST;
		else if (this.dungeonDepth <= 6)
			this.biome = BiomeSet.SWAMP;
		else
			this.biome = BiomeSet.CAVE;

		// Next, update the parameters
		updateDefaultParameters();
	}

	public BiomeSet getBiome()
	{
		return this.biome;
	}

	public int getDepth()
	{
		return this.dungeonDepth;
	}

	public int getBoardWidth()
	{
		return this.board_width;
	}

	public int getBoardHeight()
	{
		return this.board_height;
	}

	//**********************************//
	//          Helper Methods          //
	//**********************************//
	public static int random(int x)
	{
		//Random rand = new Random();
		int result = Random.Range(0, x+1);

		//Debug.Log("random() -> "+result);

		return result;
	}

	public static int random_range(int a, int b)
	{
		//Random rand = new Random();
		int result;
		// We've gotta be safe
		if (a < b)
			result = Random.Range(a, b+1);
		else
			result = Random.Range(b, a+1);

		//Debug.Log("random_range() -> "+result);

		return result;
	}

	public static bool chance(int outOf100)
	{
		int result = random_range(1, outOf100);

		if (result <= 1)
			return true;
		else
			return false;
	}

	private Direction pickDir()
	{
		return (Direction)random_range(0, 3);
	}

	private Direction tryToTurn(Direction d)
	{
		int option = random(1);

		switch (d)
		{
			case Direction.NORTH:
				if (option == 0)
					return Direction.EAST;
				else
					return Direction.WEST;
			case Direction.SOUTH:
				if (option == 0)
					return Direction.WEST;
				else
					return Direction.EAST;
			case Direction.EAST:
				if (option == 0)
					return Direction.SOUTH;
				else
					return Direction.NORTH;
			case Direction.WEST:
				if (option == 0)
					return Direction.NORTH;
				else
					return Direction.SOUTH;
			default:
				return Direction.ERROR;
		}
	}

	private int pickRoomWidth()
	{
		return random_range(this.min_room_width, this.min_room_height);
	}

	private int pickRoomHeight()
	{
		return random_range(this.min_room_height, this.max_room_height);
	}

	public static int move_x(Direction d)
	{
		if (d == Direction.EAST)
			return 1;
		else if (d == Direction.WEST)
			return -1;
		else
			return 0;
	}

	public static int move_y(Direction d)
	{
		if (d == Direction.NORTH)
			return -1;
		else if (d == Direction.SOUTH)
			return 1;
		else
			return 0;
	}

	//**********************************//
	//           Initializers           //
	//**********************************//
	private void initializeCounters()
	{
		this.tileCounter = 0;
		this.wallCounter = 0;
		this.trapCounter = 0;
		this.spawnCounter = 0;
		this.itemCounter = 0;
		this.digCounter = 0;

		this.offshootCounter = 0;
		this.puzzleCounter = 0;
		this.secretCounter = 0;
		this.tunnelCounter = 0;
		this.roomCounter = 0;
		this.hallwayCounter = 0;
	}

	private void initializeMap()
	{
		for (int x = 0; x < this.board_width; x++)
		{
			for (int y = 0; y < this.board_height; y++)
			{
				this.map[x, y].tileType = TileSet.NOTHING;
				this.map[x, y].entity = null;
				this.map[x, y].item = null;
				this.map[x, y].worldTile = null;
			}
        }

		this.createdPuzzleRoom = false;
	}

	//**********************************//
	//   Actual generation functions    //
	//**********************************//
	// Done [ ]
	private Position hallway(Position p)
	{
		Position m = p;
		int length = random_range(this.min_hallway_length, this.max_hallway_length);

		for (int i = 0; i < length; i++)
		{
			if (this.hallwayTurns)
				if (chance(this.direction_chance))
				{
					m.dir = tryToTurn(m.dir);
				}

			if (this.map[m.x, m.y].tileType == TileSet.NOTHING)
			{
				// Nothing is in the way, so just place the hallway piece
				this.map[m.x, m.y].tileType = TileSet.HALLWAY;
				this.tileCounter++;
			}
			else
			{
				// Move past the used tiles
				while (this.map[m.x, m.y].tileType != TileSet.NOTHING)
				{
					m.x += move_x(m.dir);
					m.y += move_y(m.dir);
				}

				// Place our hallway piece
				this.map[m.x, m.y].tileType = TileSet.HALLWAY;
				this.tileCounter++;
			}

			m.x += move_x(m.dir);
			m.y += move_y(m.dir);
		}

		this.hallwayCounter++;

		return m;
	}

	// Done [ ]
	private void puzzleHallway(Position p)
	{
		Position m = p;
		int length = random_range(this.min_hallway_length, this.max_hallway_length);

		for (int i = 0; i < length; i++)
		{
			// Puzzle hallways DO NOT TURN

			if (this.map[m.x, m.y].tileType == TileSet.NOTHING)
			{
				// Nothing is in the way, so just place the hallway piece
				this.map[m.x, m.y].tileType = TileSet.PUZZLE_HALLWAY;
				this.tileCounter++;
			}
			else
			{
				// Move past the used tiles
				while (this.map[m.x, m.y].tileType != TileSet.NOTHING)
				{
					m.x += move_x(m.dir);
					m.y += move_y(m.dir);
				}

				// Place our hallway piece
				this.map[m.x, m.y].tileType = TileSet.PUZZLE_HALLWAY;
				this.tileCounter++;
			}

			m.x += move_x(m.dir);
			m.y += move_y(m.dir);
		}

		this.hallwayCounter++;

		puzzle_room(m);
	}

	// Done [ ]
	private void tunnel(Position p)
	{
		Position m = p;
		int length = random_range(this.min_tunnel_length, this.max_tunnel_length);

		// For the length of the hallway...
		for (int i = 0; i < length; i++)
		{
			if (this.hallwayTurns)
				if (chance(this.direction_chance))
				{
					m.dir = tryToTurn(m.dir);
				}

			// Change the width every step for varied tunnels
			int width = random_range(this.min_tunnel_width, this.max_tunnel_width);
			int lBound, rBound, uBound, dBound;
			lBound = -(int)Mathf.Floor((int)width/2)-1;
			rBound = (int)Mathf.Ceil((int)width/2)+1;
			uBound = lBound;
			dBound = rBound;

			// ...try to place dig tiles in a square shape based on the current width
			for (int j = lBound; j < rBound; j++)
			{
				for (int k = uBound; k < dBound; k++)
				{
					if (this.map[m.x + j, m.y + k].tileType == TileSet.NOTHING)
					{
						// Nothing is in the way, so just place the hallway piece
						this.map[m.x + j, m.y + k].tileType = TileSet.TUNNEL;

						// Remove any entities we've found
						if (this.map[m.x + j, m.y + k].entity != null)
						{
							if (this.EnemyList.Contains(this.map[m.x + j, m.y + k].entity))
								this.EnemyList.Remove(this.map[m.x + j, m.y + k].entity);

							Destroy(this.map[m.x + j, m.y + k].entity);
							this.map[m.x + j, m.y + k].entity = null;
						}

						if (this.map[m.x + j, m.y + k].item != null)
						{
							Destroy(this.map[m.x + j, m.y + k].item);
							this.map[m.x + j, m.y + k].item = null;
						}

						this.tileCounter++;
						this.digCounter++;
					}
					///*
					else
					{
						// Move past the used tiles
						if (j == 0 && k == 0)
						{
							while (this.map[m.x, m.y].tileType != TileSet.NOTHING && this.map[m.x, m.y].tileType != TileSet.TUNNEL)
							{
								m.x += move_x(m.dir);
								m.y += move_y(m.dir);
							}

							// Place our hallway piece
							this.map[m.x + j, m.y + k].tileType = TileSet.TUNNEL;

							// Remove any entities we've found
							if (this.map[m.x + j, m.y + k].entity != null)
							{
								if (this.EnemyList.Contains(this.map[m.x + j, m.y + k].entity))
									this.EnemyList.Remove(this.map[m.x + j, m.y + k].entity);

								Destroy(this.map[m.x + j, m.y + k].entity);
								this.map[m.x + j, m.y + k].entity = null;
							}

							if (this.map[m.x + j, m.y + k].item != null)
							{
								Destroy(this.map[m.x + j, m.y + k].item);
								this.map[m.x + j, m.y + k].item = null;
							}

							this.tileCounter++;
							this.digCounter++;
						}
					}
					//*/
				}
			}

			// Move to our next tunnel position
			m.x += move_x(m.dir);
			m.y += move_y(m.dir);
		}

		this.tunnelCounter++;

		// Create a secret if we're halfway through generation and we don't have one yet
		if (this.secretCounter <= this.min_secret_room_count && this.roomCounter >= (int)this.max_room_count/2)
			secret_room(m);
		else
		// Otherwise, do the normal thing
		{
			if (this.secretCounter < this.max_secret_room_count)
				if (chance(this.secret_chance))
					secret_room(m);
		}
	}

	public void LoadItemSprites()
	{
		spr_RedAntsBottle = Resources.Load<Sprite>("Art/Items/RedAntsBottle");
	    spr_Blueberries = Resources.Load<Sprite>("Art/Items/Blueberries");
	    spr_Pocketknife = Resources.Load<Sprite>("Art/Items/PocketKnife");
	    spr_Rapier = Resources.Load<Sprite>("Art/Items/Rapier");
	    spr_SkunkGas = Resources.Load<Sprite>("Art/Items/SkunkGas");
	    spr_Snaps = Resources.Load<Sprite>("Art/Items/Snaps");
	    spr_StickRock = Resources.Load<Sprite>("Art/Items/StickRock");
	    spr_SunflowerSeed = Resources.Load<Sprite>("Art/Items/SunflowerSeed");
	    spr_ThornVine = Resources.Load<Sprite>("Art/Items/ThornVines");
	    spr_Carrot = Resources.Load<Sprite>("Art/Items/Carrot");
	    spr_Treat = Resources.Load<Sprite>("Art/Items/Treat");
	    spr_Key = Resources.Load<Sprite>("Art/Items/Key");
	}

	public void spawnItem(int xx, int yy)
	{
		int whichItem = Random.Range(0, 10);
		GameObject tempItem = Instantiate(GameObject.Find("WorldItem"), new Vector2(xx * offsetforTiles, yy * offsetforTiles), Quaternion.identity);

		switch(whichItem)
		{
			// Red Ants Bottle
			case 0:
				tempItem.GetComponent<SpriteRenderer>().sprite = spr_RedAntsBottle;
				tempItem.GetComponent<Item>().itemType = ItemType.RED_ANTS_BOTTLE;
				break;
			// Blueberries
			default:
			case 1:
				tempItem.GetComponent<SpriteRenderer>().sprite = spr_Blueberries;
				tempItem.GetComponent<Item>().itemType = ItemType.BLUEBERRIES;
				break;
			// PocketKnife
			case 2:
				tempItem.GetComponent<SpriteRenderer>().sprite = spr_Pocketknife;
				tempItem.GetComponent<Item>().itemType = ItemType.POCKETKNIFE;
				//tempItem.GetComponent<Item>().damageType = 's';
				break;
			// Rapier
			case 3:
				tempItem.GetComponent<SpriteRenderer>().sprite = spr_Rapier;
				tempItem.GetComponent<Item>().itemType = ItemType.RAPIER;
				//tempItem.GetComponent<Item>().damageType = 't';
				break;
			// Skunk Gas
			case 4:
				tempItem.GetComponent<SpriteRenderer>().sprite = spr_SkunkGas;
				tempItem.GetComponent<Item>().itemType = ItemType.SKUNK_GAS;
				break;
			// Snaps
			case 5:
				tempItem.GetComponent<SpriteRenderer>().sprite = spr_Snaps;
				tempItem.GetComponent<Item>().itemType = ItemType.SNAPS;
				break;
			// Carrot
			case 6:
				tempItem.GetComponent<SpriteRenderer>().sprite = spr_Carrot;
				tempItem.GetComponent<Item>().itemType = ItemType.CARROT;
				//tempItem.GetComponent<Item>().damageType = 'b';
				break;
			// Sunflower Seed
			case 7:
				tempItem.GetComponent<SpriteRenderer>().sprite = spr_SunflowerSeed;
				tempItem.GetComponent<Item>().itemType = ItemType.SUNFLOWER_SEED;
				break;
			// Thorny Vines
			case 8:
				tempItem.GetComponent<SpriteRenderer>().sprite = spr_ThornVine;
				tempItem.GetComponent<Item>().itemType = ItemType.THORN_VINE;
				break;
			// Treat
			case 9:
				tempItem.GetComponent<SpriteRenderer>().sprite = spr_Treat;
				tempItem.GetComponent<Item>().itemType = ItemType.TREAT;
				break;

		}

		tempItem.GetComponent<SpriteRenderer>().sortingOrder = 1;
		tempItem.GetComponent<Item>().generateWeaponStats(dungeonDepth);
		this.map[xx, yy].item = tempItem;
	}

	// Done [ ]
	private void room(Position p)
	{
		Position m = p;
		// Debug.Log("Room is picking width ("+this.min_room_width+","+this.max_room_width+") and height ("+this.min_room_height+","+this.max_room_height+")");
		int _width = random_range(this.min_room_width, this.max_room_width);
		int _height = random_range(this.min_room_height, this.max_room_height);
		// Debug.Log("Picked width and height: "+_width+", "+_height);

		// Strings that point to Enemy Prefabs
		string[] enemyArray = new string[2];
		enemyArray[0] = "MantisEnemy";
		enemyArray[1] = "FalconEnemy";

		int lBound, rBound, uBound, dBound;

		if (this.centeredRooms)
		{
			// Create rooms around x and y
			lBound = -(int) Mathf.Floor((int) _width/2);
			rBound = (int) Mathf.Ceil((int) _width/2);
			uBound = -(int) Mathf.Floor((int) _height/2);
			dBound = (int) Mathf.Ceil((int) _height/2);
		}
		else
		{
			lBound = 0;
			rBound = _width;
			uBound = 0;
			dBound = _height;
		}

		// Actually start placing tiles
		for (int xx = lBound; xx < rBound; xx++)
		{
			for (int yy = uBound; yy < dBound; yy++)
			{
				// Default: we set down room floor tiles
				TileSet type = TileSet.FLOOR;

				// Try to change the tile
				if (chance(this.trap_chance))
					type = TileSet.TRAP;
				else if (chance(this.spawn_chance))
					type = TileSet.SPAWNER;
				else if (chance(this.dig_chance))
					type = TileSet.DIG_TILE;

				// Set down the tile we chose if nothing is there
				if (this.map[m.x + xx, m.y + yy].tileType == TileSet.NOTHING || this.map[m.x + xx, m.y + yy].tileType == TileSet.HALLWAY)
				{
					this.map[m.x + xx, m.y + yy].tileType = type;
					this.tileCounter++;

					if (type == TileSet.TRAP)
						this.trapCounter++;
					else if (type == TileSet.SPAWNER)
					{
						this.spawnCounter++;

						// Create an enemy
						// Picking index from enemyArray
						int enemyTypeRand = Random.Range(0,2);
						EnemyBasic.enemyType enemyChosenType = (EnemyBasic.enemyType) enemyTypeRand;

						// Making enemy object for the board, then setting it's stats based on enemyType
						GameObject enemy = (GameObject)Instantiate(GameObject.Find(enemyArray[enemyTypeRand]), new Vector2((m.x + xx) * offsetforTiles, (m.y + yy) * offsetforTiles), Quaternion.identity);
						enemy.GetComponent<EnemyBasic>().Set(enemyChosenType, this.dungeonDepth);
						// Debug.Log("Placing: " + enemy.name + " at x: " + m.x + " y: " + m.y);

						this.map[m.x + xx, m.y + yy].entity = enemy;
						enemy.GetComponent<EnemyBasic>().currentX = m.x+xx;
						enemy.GetComponent<EnemyBasic>().currentY = m.y+yy;
						this.EnemyList.Add(enemy);
					}
				}

				// Place item in the room
				if (type != TileSet.DIG_TILE)
				{
					if (chance(this.general_item_chance))
					{
						spawnItem(m.x + xx, m.y + yy);
						this.itemCounter++;
					}
				}
			}
		}

		this.roomCounter++;

		if (chance(this.tunnel_chance))
		{
			Position t = m;
			t.dir = tryToTurn(m.dir);
			tunnel(t);
		}
	}

	// Done [ ]
	private void secret_room(Position p)
	{
		// For now, just use the normal room script
		Position m = p;
		int _width = random_range(this.min_secret_room_width, this.max_secret_room_width);
		int _height = random_range(this.min_secret_room_height, this.max_secret_room_height);

		int lBound, rBound, uBound, dBound;

		if (this.centeredRooms)
		{
			// Create rooms around x and y
			lBound = -(int) Mathf.Floor((int) _width/2);
			rBound = (int) Mathf.Ceil((int) _width/2);
			uBound = -(int) Mathf.Floor((int) _height/2);
			dBound = (int) Mathf.Ceil((int) _height/2);
		}
		else
		{
			lBound = 0;
			rBound = _width;
			uBound = 0;
			dBound = _height;
		}

		// Actually start placing tiles
		for (int xx = lBound; xx < rBound; xx++)
		{
			for (int yy = uBound; yy < dBound; yy++)
			{
				// Default: we set down room floor tiles
				TileSet type = TileSet.SECRET_FLOOR;

				// Try to change the tile
				if (chance(this.trap_chance))
					type = TileSet.TRAP;
				else if (chance(this.spawn_chance))
					type = TileSet.SPAWNER;
				else if (chance(this.dig_chance))
					type = TileSet.DIG_TILE;

				// Set down the tile we chose if nothing is there
				if (this.map[m.x + xx, m.y + yy].tileType == TileSet.NOTHING || this.map[m.x + xx, m.y + yy].tileType == TileSet.HALLWAY || this.map[m.x + xx, m.y + yy].tileType == TileSet.DIG_TILE)
				{
					this.map[m.x + xx, m.y + yy].tileType = type;
					this.tileCounter++;

					if (type == TileSet.TRAP)
						this.trapCounter++;
					else if (type == TileSet.SPAWNER)
						this.spawnCounter++;
				}

				// Place item in the room
				if (type != TileSet.DIG_TILE)
				{
					if (chance(this.rare_item_chance))
					{
						spawnItem(m.x + xx, m.y + yy);
						this.itemCounter++;
					}
				}
			}
		}

		this.roomCounter++;
		this.secretCounter++;
	}

	// Written By: Christopher Walen
	// ASSUMES AN NXN MATRIX
	public static GridCell [,] rotate_right_90(int n, GridCell [,] matrix)
	{
		GridCell[,] temp = new GridCell[n, n];

		for (int row = 0; row < n; row++)
		{
			for (int x = 0; x < n; x++)
			{
				temp[x, row] = matrix[row, n - 1 - x];
			}
		}

		return temp;
	}

	// Written By: Christopher Walen
	// ASSUMES AN NXN MATRIX
	public static GridCell [,] rotate_left_90(int n, GridCell [,] matrix)
	{
		GridCell[,] temp = new GridCell[n, n];

		for (int row = 0; row < n; row++)
		{
			for (int x = 0; x < n; x++)
			{
				temp[x, row] = matrix[n - 1 - row, x];
			}
		}

		return temp;
	}

	private void puzzle_room(Position p)
	{
		// Grab the puzzle from grabPuzzle();
		this.puzzle = grabPuzzle();
		int xOffset = 0;
		int yOffset = 0;

		this.puzzlePos = p;

		// Rotate the array based on the direction
		// and set the offset accordingly
		if (p.dir == Direction.NORTH)
		{
			Debug.Log("CREATING PUZZLE IN DIRECTION NORTH");
			this.puzzle = rotate_right_90(11, this.puzzle);
			xOffset = -5;
			yOffset = -10;
		}
		else if (p.dir == Direction.SOUTH)
		{
			Debug.Log("CREATING PUZZLE IN DIRECTION SOUTH");
			this.puzzle = rotate_left_90(11, this.puzzle);
			xOffset = -5;
			yOffset = 0;
		}
		else if (p.dir == Direction.EAST)
		{
			Debug.Log("CREATING PUZZLE IN DIRECTION EAST");
			this.puzzle = rotate_right_90(11, this.puzzle);
			this.puzzle = rotate_right_90(11, this.puzzle);
			xOffset = 0;
			yOffset = -5;
		}
		else if (p.dir == Direction.WEST)
		{
			Debug.Log("CREATING PUZZLE IN DIRECTION WEST");
			// Don't need to rotate puzzle here
			xOffset = -10;
			yOffset = -5;
		}
		else
		{
			// We've encountered an error, so jump out
			Debug.Log("ERROR: Attempted to create puzzle in non-existant direction");
			return;
		}

		// Place the tiles and overwrite them if need be
		for (int xx = 0; xx < 11; xx++)
		{
			for (int yy = 0; yy < 11; yy++)
			{
				if (this.map[p.x + xx + xOffset, p.y + yy + yOffset].item != null)
				{
					Destroy(this.map[p.x + xx + xOffset, p.y + yy + yOffset].item);
					this.map[p.x + xx + xOffset, p.y + yy + yOffset].item = null;
				}
				if (this.map[p.x + xx + xOffset, p.y + yy + yOffset].entity != null)
				{
					if (this.EnemyList.Contains(this.map[p.x + xx + xOffset, p.y + yy + yOffset].entity))
						this.EnemyList.Remove(this.map[p.x + xx + xOffset, p.y + yy + yOffset].entity);

					Destroy(this.map[p.x + xx + xOffset, p.y + yy + yOffset].entity);
					this.map[p.x + xx + xOffset, p.y + yy + yOffset].entity = null;
				}

				this.map[p.x + xx + xOffset, p.y + yy + yOffset].tileType = this.puzzle[xx, yy].tileType;
			}
		}

		this.puzzleCounter++;
	}

	public void respawnPuzzle()
	{
		Position p = this.puzzlePos;
		int xOffset = 0;
		int yOffset = 0;

		if (p.dir == Direction.NORTH)
		{
			xOffset = -5;
			yOffset = -10;
		}
		else if (p.dir == Direction.SOUTH)
		{
			xOffset = -5;
			yOffset = 0;
		}
		else if (p.dir == Direction.EAST)
		{
			xOffset = 0;
			yOffset = -5;
		}
		else if (p.dir == Direction.WEST)
		{
			xOffset = -10;
			yOffset = -5;
		}

		for (int xx = 0; xx < 11; xx++)
		{
			for (int yy = 0; yy < 11; yy++)
			{
				if (this.map[p.x + xx + xOffset, p.y + yy + yOffset].item != null)
				{
					Destroy(this.map[p.x + xx + xOffset, p.y + yy + yOffset].item);
					this.map[p.x + xx + xOffset, p.y + yy + yOffset].item = null;
				}
				if (this.map[p.x + xx + xOffset, p.y + yy + yOffset].entity != null)
				{
					if (this.map[p.x + xx + xOffset, p.y + yy + yOffset].entity != HamsterEntity)
					{
						Destroy(this.map[p.x + xx + xOffset, p.y + yy + yOffset].entity);
						this.map[p.x + xx + xOffset, p.y + yy + yOffset].entity = null;
					}
				}

				this.map[p.x + xx + xOffset, p.y + yy + yOffset].tileType = this.puzzle[xx, yy].tileType;

				if (this.map[p.x + xx + xOffset, p.y + yy + yOffset].tileType == TileSet.PIT)
					this.map[p.x + xx + xOffset, p.y + yy + yOffset].worldTile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestPit;
				else if (this.map[p.x + xx + xOffset, p.y + yy + yOffset].tileType == TileSet.BRANCH)
					this.map[p.x + xx + xOffset, p.y + yy + yOffset].worldTile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestBranch;
				else if (this.map[p.x + xx + xOffset, p.y + yy + yOffset].tileType == TileSet.KEY_TILE)
				{
					this.map[p.x + xx + xOffset, p.y + yy + yOffset].worldTile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestKeyTile;

					// Respawn the key
					GameObject tempItem = Instantiate(GameObject.Find("WorldItem"), new Vector2((p.x + xx + xOffset) * offsetforTiles, (p.y + yy + yOffset) * offsetforTiles), Quaternion.identity);
					tempItem.GetComponent<SpriteRenderer>().sprite = this.spr_Key;
					tempItem.GetComponent<SpriteRenderer>().sortingOrder = 1;
					tempItem.GetComponent<Item>().itemType = ItemType.KEY;
					this.map[p.x + xx + xOffset, p.y + yy + yOffset].item = tempItem;
				}
			}
		}

		HamsterEntity.GetComponent<LLB>().currentX = p.x;
		HamsterEntity.GetComponent<LLB>().currentY = p.y;
		HamsterEntity.GetComponent<LLB>().AttachSpriteToPosition();
	}

	// Done [ ]
	private void start_room(Position p)
	{
		// For now, just use the normal room script
		// TODO: specific changes for starting room
		room(p);
		this.map[p.x, p.y].tileType = TileSet.START_TILE;

		this.roomCounter++;
	}

	// Done [ ]
	private void end_room(Position p)
	{
		// For now, just use the normal room script
		// TODO: specific changes for ending room
		room(p);
		this.map[p.x, p.y].tileType = TileSet.END_TILE;

		this.roomCounter++;
	}

	private void boss_arena(Position p)
	{
		for (int yy = -15; yy < 15; yy++)
		{
			for (int xx = -15; xx < 15; xx++)
			{
				TileSet type = TileSet.FLOOR;

				if ((xx < -12 || xx > 12) && (yy < -12 || yy > 12))
				{
					if (Random.value < 0.3)
					{
						type = TileSet.SPAWNER;
					}
				}

				this.map[p.x + xx, p.y + yy].tileType = type;
			}
		}

		this.map[p.x, p.y + 10].tileType = TileSet.START_TILE;
	}

	// Done [ ]
	private void make_walls()
	{
		for (int x = 1; x < this.board_width-1; x++)
		{
			for (int y = 1; y < this.board_height-1; y++)
			{
				// Perimiter of the map must be clear
				if (x == 0 || y == 0 || x == this.board_width-1 || y == this.board_height-1)
				{
					this.map[x, y].tileType = TileSet.NOTHING;
					this.wallCounter++;
				}
				else
				{
					// Get ready for the rumble
					int numWalls = 3 + (int)(Mathf.Ceil(Random.value * 6));

					// First, make sure that we're placing walls around a tile
					if (this.map[x, y].tileType != TileSet.NOTHING && this.map[x, y].tileType != TileSet.WALL)
					{
						// Then, check the 8 positions around it (technically including itself)
						for (int xx = -numWalls; xx <= numWalls; xx++)
							for (int yy = -numWalls; yy <= numWalls; yy++)
							{
								if (this.map[x+xx, y+yy].tileType == TileSet.NOTHING)
								{
									this.map[x+xx, y+yy].tileType = TileSet.WALL;
									this.wallCounter++;
								}
							}
					}
				}
			}
		}
	}

	// Done [ ]
	private void offshoot(Position p)
	{
		Position m = p;
		int numRooms = random_range(2, 4);

		for (int i = 0; i < numRooms; i++)
		{
			m = hallway(m);
			room(m);

			if (chance(direction_chance))
				m.dir = tryToTurn(m.dir);
		}

		this.offshootCounter++;
	}

	// I hate that I even had to write this
	GridCell[,] grabPuzzle()
	{
		GridCell[,] puzzle;
		int choice = (int)(Random.Range(1, 5));

		// If we're in FOREST biome, don't add anything
		if (this.biome == BiomeSet.SWAMP)
			choice += 4;
		else if (this.biome == BiomeSet.CAVE)
			choice += 8;

		Debug.Log("Picked: " + choice);

		switch (choice)
		{
			default:
				puzzle = new GridCell[,]
				{
					{new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR)},
					{new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR)},
					{new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR)},
					{new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR)},
					{new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.KEY_TILE), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR)},
					{new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR)},
					{new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR)},
					{new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR)},
					{new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR)},
					{new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR), new GridCell(TileSet.FLOOR)}
				};
				break;
			// Falling Puzzles
			// Done [x]
			case 1:
				puzzle = new GridCell[,]
				{
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.KEY_TILE), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), 	new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.FLOOR), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)}
				};
				break;
			// Done [x]
			case 2:
				puzzle = new GridCell[,]
				{
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.KEY_TILE), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.FLOOR), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)}
				};
				break;
			// Done [x]
			case 3:
				puzzle = new GridCell[,]
				{
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.KEY_TILE), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.FLOOR), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)}
				};
				break;
			// Done [x]
			case 4:
				puzzle = new GridCell[,]
				{
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.KEY_TILE), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.BRANCH), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.PIT), new GridCell(TileSet.BRANCH), new GridCell(TileSet.PIT), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.FLOOR), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)}
				};
				break;

			// Sliding Puzzles
			// Done [x]
			case 5:
				puzzle = new GridCell[,]
				{
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.ROCK), new GridCell(TileSet.KEY_TILE), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.FLOOR), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)}
				};
				break;
			// Done [x]
			case 6:
				puzzle = new GridCell[,]
				{
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.KEY_TILE), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.FLOOR), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)}
				};
				break;
			// Done [x]
			case 7:
				puzzle = new GridCell[,]
				{
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.KEY_TILE), new GridCell(TileSet.ROCK), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.FLOOR), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)}
				};
				break;
			// Done [x]
			case 8:
				puzzle = new GridCell[,]
				{
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.ROCK), new GridCell(TileSet.KEY_TILE), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.MUD), new GridCell(TileSet.ROCK), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.FLOOR), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)}
				};
				break;
			// TODO: MAKE BOULDER ARRAYS 11x11 AND FILL THEM ACCORDING TO PLANS
			// Boulder Puzzles
			// Done [x]
			case 9:
				puzzle = new GridCell[,]
				{
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.KEY_TILE), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)}
				};
				break;
			// Done [x]
			case 10:
				puzzle = new GridCell[,]
				{
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.KEY_TILE), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)}
				};
				break;
			// Done [x]
			case 11:
				puzzle = new GridCell[,]
				{
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.KEY_TILE), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)}
				};
				break;
			// Done [x]
			case 12:
				puzzle = new GridCell[,]
				{
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.KEY_TILE), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.BOULDER), new GridCell(TileSet.WALL)},
					{new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.PUZZLE_FLOOR), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL), new GridCell(TileSet.WALL)}
				};
				break;
		}

		return puzzle;
	}

	public void generate()
	{
		map = new GridCell[this.board_width, this.board_height];
		initializeMap();
		initializeCounters();

		// Starting x and y for the generator
		Position g;
		g.x = this.board_width / 2;
		g.y = this.board_width / 2;
		g.dir = pickDir();

		// If we've hit the final level, make the boss level instead and jump out
		if (this.dungeonDepth > 9)
		{
			boss_arena(g);
			make_walls();
			return;
		}

		int numRooms = random_range(this.min_room_count, this.max_room_count);

		// Starting room

		start_room(g);

		for (int i = 0; i < numRooms; i++)
		{
			// Standard loop - hallway then room
			g = hallway(g);
			room(g);

			// Offshoots
			if (chance(this.offshoot_chance))
			{
				Position t = g;
				t.dir = tryToTurn(g.dir);
				offshoot(t);
			}
			// Secrets
			else if (chance(this.tunnel_chance))
			{
				Position t = g;
				t.dir = tryToTurn(g.dir);
				tunnel(t);
			}
			// Puzzle - for now only supports one
			else if (chance(this.puzzle_chance) && !this.createdPuzzleRoom)
			{
				Position t = g;
				t.dir = tryToTurn(g.dir);
				puzzleHallway(t);
				this.createdPuzzleRoom = true;
			}

			// Time for the generator to try to turn
			if (chance(direction_chance))
				g.dir = tryToTurn(g.dir);
		}

		// Ending room
		end_room(g);

		// Create the puzzle if we haven't already
		if (!this.createdPuzzleRoom)
		{
			puzzleHallway(g);
			this.createdPuzzleRoom = true;
		}

		// Create the walls
		make_walls();
	}

	public IEnumerator moveEnemies() // IEnumerator so the game can space out their actions
	{
		Debug.Log("Moving "+this.EnemyList.Count+" enemies..."); // Edit so I can make a pull request :P

		for (int i = 0; i < this.EnemyList.Count; i++)
		{
			GameObject temp = (GameObject)this.EnemyList[i];
            if (!temp.GetComponent<BasicEntity>().stunned)
            {
                if (temp != null && temp.GetComponent<BasicEntity>().active)
                {
                    if (temp.GetComponent<EnemyBasic>().isAlert())
                    {

                        yield return new WaitForSeconds(0.00005f); // IEnumerators must yield at some point
                        int goalX = HamsterEntity.GetComponent<BasicEntity>().currentX;
                        int goalY = HamsterEntity.GetComponent<BasicEntity>().currentY;
                        // Debug.Log("Pathfinding to: "+goalX+", "+goalY);
                        yield return StartCoroutine(temp.GetComponent<EnemyBasic>().pathfindTowardsPoint(goalX, goalY, this.map));
                    }
                    else
                    {
                        if (temp.GetComponent<EnemyBasic>().lineOfSight(HamsterEntity.GetComponent<BasicEntity>(), this.map))
                        {
                            yield return new WaitForSeconds(0.00005f); // IEnumerators must yield at some point
                            Debug.Log("Found you!");
                            temp.GetComponent<EnemyBasic>().makeAlert();
                        }

                        temp.GetComponent<EnemyBasic>().wander();
                    }

                }
            }
            else // if stunned, take a turn off
            {
                temp.GetComponent<BasicEntity>().stunnedTurns -= 1;
                if (temp.GetComponent<BasicEntity>().stunnedTurns < 0)
                    temp.GetComponent<BasicEntity>().stunned = false;
            }
		}
	}

	//*********************************//
	//      Miscellaneous Methods      //
	//*********************************//
	// For debugging purposes, prints the map with selected mode
	public void printMap(int mode)
	{
		for (int y = 0; y < this.board_height; y++)
		{
			for (int x = 0; x < this.board_width; x++)
			{
				switch (mode)
				{
					case 0: // Prints short values and empty spaces
						//Debug.Log("["+(short)this.map[x,y].tileType+"]");
						if (this.map[x,y].tileType == TileSet.NOTHING)
							Debug.Log(" ");
                            
						else
							Debug.Log((short)this.map[x,y].tileType);
                            
                        break;
					case 1: // Prints enum names
						Debug.Log(this.map[x,y].tileType+" ");
						break;
					case 2: // Prints short values without empty spaces
						if (this.map[x, y].tileType != TileSet.NOTHING)
							Debug.Log("["+(short)this.map[x,y].tileType+"]");
                            // /Instantiate(Floor, new Vector2(x,y), Quaternion.identity);
						break;
				}
			}
            
			Debug.Log("\n");
		}
	}

	public void LoadTileSprites()
	{
		// Set Forest Sprites
    	this.spr_ForestFloor = Resources.Load<Sprite>("Art/ForestTiles/tile_Forest_Ground");
    	this.spr_ForestWall = Resources.Load<Sprite>("Art/ForestTiles/tile_Forest_Wall");
    	this.spr_ForestBranch = Resources.Load<Sprite>("Art/ForestTiles/tile_Forest_Branch");
    	this.spr_ForestPit = Resources.Load<Sprite>("Art/ForestTiles/tile_Forest_Pit");
    	this.spr_ForestTunnel = Resources.Load<Sprite>("Art/ForestTiles/tile_Forest_Tunnel");
    	this.spr_ForestDig = Resources.Load<Sprite>("Art/ForestTiles/tile_Forest_DigTile");
    	this.spr_ForestHallway = Resources.Load<Sprite>("Art/ForestTiles/tile_Forest_Hallway");
    	this.spr_ForestEnd = Resources.Load<Sprite>("Art/ForestTiles/tile_Forest_EndTile");
    	this.spr_ForestSpawner = Resources.Load<Sprite>("Art/ForestTiles/tile_Forest_Spawner");
    	this.spr_ForestTrap = Resources.Load<Sprite>("Art/ForestTiles/tile_Forest_Trap");
    	this.spr_ForestStart = this.spr_ForestFloor;
    	this.spr_ForestSecretFloor = this.spr_ForestFloor;
    	this.spr_ForestPuzzleHallway = this.spr_ForestFloor;
    	this.spr_ForestPuzzleFloor = this.spr_ForestFloor;
    	this.spr_ForestKeyTile = this.spr_ForestFloor;

    	// Set Swamp Sprites
	    this.spr_SwampFloor = Resources.Load<Sprite>("Art/SwampTiles/tile_Swamp_Ground");
	    this.spr_SwampWall = Resources.Load<Sprite>("Art/SwampTiles/tile_Swamp_Wall");
	    this.spr_SwampMud = Resources.Load<Sprite>("Art/SwampTiles/tile_Swamp_Mud");
	    this.spr_SwampPuzzleFloor = this.spr_SwampFloor;
	    this.spr_SwampTunnel = Resources.Load<Sprite>("Art/SwampTiles/tile_Swamp_PuzzleGround");
	    this.spr_SwampDig = Resources.Load<Sprite>("Art/SwampTiles/tile_Swamp_DigTile");
	    this.spr_SwampHallway = Resources.Load<Sprite>("Art/SwampTiles/tile_Swamp_Hallway");
	    this.spr_SwampEnd = Resources.Load<Sprite>("Art/SwampTiles/tile_Swamp_EndTile");
	    this.spr_SwampSpawner = Resources.Load<Sprite>("Art/SwampTiles/tile_Swamp_Spawner");
	    this.spr_SwampTrap = Resources.Load<Sprite>("Art/SwampTiles/tile_Swamp_Trap");
	    this.spr_SwampSecretFloor = this.spr_SwampFloor;
	    this.spr_SwampStart = this.spr_SwampFloor;
	    this.spr_SwampPuzzleHallway = this.spr_SwampFloor;
	    this.spr_SwampKeyTile = this.spr_SwampFloor;
	    this.spr_SwampRock = Resources.Load<Sprite>("Art/SwampTiles/tile_Swamp_Rock");

	    // Set Cave Sprites
	    this.spr_CaveFloor = Resources.Load<Sprite>("Art/CaveTiles/tile_Cave_Ground");
	    this.spr_CaveWall = Resources.Load<Sprite>("Art/CaveTiles/tile_Cave_Wall");
	    this.spr_CaveBoulder = Resources.Load<Sprite>("Art/CaveTiles/tile_Cave_Boulder");
	    this.spr_CaveTunnel = Resources.Load<Sprite>("Art/CaveTiles/tile_Cave_Tunnel");
	    this.spr_CaveDig = Resources.Load<Sprite>("Art/CaveTiles/tile_Cave_DigTile");
	    this.spr_CaveHallway = Resources.Load<Sprite>("Art/CaveTiles/tile_Cave_Hallway");
	    this.spr_CaveStart = this.spr_CaveFloor;
	    this.spr_CavePuzzleFloor = this.spr_CaveFloor;
	    this.spr_CaveEnd = Resources.Load<Sprite>("Art/CaveTiles/tile_Cave_EndTile");
	    this.spr_CaveSpawner = Resources.Load<Sprite>("Art/CaveTiles/tile_Cave_Spawner");
	    this.spr_CaveSecretFloor = this.spr_CaveFloor;
	    this.spr_CaveTrap = Resources.Load<Sprite>("Art/CaveTiles/tile_Cave_Trap");
	    this.spr_CavePuzzleHallway = this.spr_CaveFloor;
	    this.spr_CaveKeyTile = this.spr_CaveFloor;
	}

    public void GenMap(int mode)
    {
    	// Load in the sprites when we generate the world tiles
    	// LoadTileSprites();

        for (int y = 0; y < this.board_height; y++)
        {
            for (int x = 0; x < this.board_width; x++)
            {
                switch (mode)
                {
                    case 0: // Prints short values and empty spaces
                            //Debug.Log("["+(short)this.map[x,y].tileType+"]");
                        if (this.map[x, y].tileType == TileSet.NOTHING) {
                            //Debug.Log(" ");

                        } else
                        {
                        	// For reference
       						// public enum TileSet : short
							// {
							// 	NOTHING = -1, FLOOR = 0,
							// 	WALL, HALLWAY, TRAP, SPAWNER, SECRET_FLOOR,
							// 	PUZZLE_FLOOR, ROCK, MUD, BOULDER, DIG_TILE,
							// 	PUZZLE_HALLWAY, START_TILE, END_TILE
							// };
                            
                        	GameObject tile = Instantiate(GameObject.Find("WorldTile"), new Vector2(x * offsetforTiles, y * offsetforTiles), Quaternion.identity);
                        	// Debug.Log(tile);
                        	tile.GetComponent<SpriteRenderer>().sortingOrder = 0;

							switch (this.map[x,y].tileType)
							{
								default:
								case TileSet.FLOOR:
									// Debug.Log("Found: tiletype FLOOR");

									if (this.biome == BiomeSet.FOREST)
									{
										// Debug.Log("Biome: FOREST");
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestFloor;
									}
									else if (this.biome == BiomeSet.SWAMP)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_SwampFloor;
									else if (this.biome == BiomeSet.CAVE)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_CaveFloor;
									break;
								case TileSet.WALL:
									if (this.biome == BiomeSet.FOREST)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestWall;
									else if (this.biome == BiomeSet.SWAMP)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_SwampWall;
									else if (this.biome == BiomeSet.CAVE)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_CaveWall;
									break;
								case TileSet.HALLWAY:
									if (this.biome == BiomeSet.FOREST)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestHallway;
									else if (this.biome == BiomeSet.SWAMP)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_SwampHallway;
									else if (this.biome == BiomeSet.CAVE)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_CaveHallway;
									break;
								case TileSet.TRAP:
									if (this.biome == BiomeSet.FOREST)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestTrap;
									else if (this.biome == BiomeSet.SWAMP)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_SwampTrap;
									else if (this.biome == BiomeSet.CAVE)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_CaveTrap;
									break;
								case TileSet.SPAWNER:
									if (this.biome == BiomeSet.FOREST)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestSpawner;
									else if (this.biome == BiomeSet.SWAMP)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_SwampSpawner;
									else if (this.biome == BiomeSet.CAVE)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_CaveSpawner;
									break;
								case TileSet.SECRET_FLOOR:
									if (this.biome == BiomeSet.FOREST)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestSecretFloor;
									else if (this.biome == BiomeSet.SWAMP)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_SwampSecretFloor;
									else if (this.biome == BiomeSet.CAVE)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_CaveSecretFloor;
									break;
								case TileSet.PUZZLE_FLOOR:
									if (this.biome == BiomeSet.FOREST)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestPuzzleFloor;
									else if (this.biome == BiomeSet.SWAMP)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_SwampPuzzleFloor;
									else if (this.biome == BiomeSet.CAVE)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_CavePuzzleFloor;
									break;
								case TileSet.PUZZLE_HALLWAY:
									if (this.biome == BiomeSet.FOREST)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestPuzzleHallway;
									else if (this.biome == BiomeSet.SWAMP)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_SwampPuzzleHallway;
									else if (this.biome == BiomeSet.CAVE)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_CavePuzzleHallway;
									break;
								case TileSet.TUNNEL:
									if (this.biome == BiomeSet.FOREST)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestTunnel;
									else if (this.biome == BiomeSet.SWAMP)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_SwampTunnel;
									else if (this.biome == BiomeSet.CAVE)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_CaveTunnel;
									break;
								case TileSet.BOULDER:
									tile.GetComponent<SpriteRenderer>().sprite = this.spr_CaveBoulder;
									break;
								case TileSet.ROCK:
									tile.GetComponent<SpriteRenderer>().sprite = this.spr_SwampRock;
									break;
								case TileSet.MUD:
									tile.GetComponent<SpriteRenderer>().sprite = this.spr_SwampMud;
									break;
								case TileSet.PIT:
									tile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestPit;
									break;
								case TileSet.BRANCH:
									tile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestBranch;
									break;
								case TileSet.DIG_TILE:
									if (this.biome == BiomeSet.FOREST)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestDig;
									else if (this.biome == BiomeSet.SWAMP)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_SwampDig;
									else if (this.biome == BiomeSet.CAVE)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_CaveDig;
									break;
								case TileSet.START_TILE:
									// Move LLB to the start tile
									HamsterEntity.GetComponent<BasicEntity>().currentX = x;
                                    HamsterEntity.GetComponent<BasicEntity>().currentY = y;
                                    HamsterEntity.transform.position = new Vector2(x * offsetforTiles, y * offsetforTiles);
                                    this.map[x, y].entity = HamsterEntity;

									if (this.biome == BiomeSet.FOREST)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestStart;
									else if (this.biome == BiomeSet.SWAMP)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_SwampStart;
									else if (this.biome == BiomeSet.CAVE)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_CaveStart;
									break;
								case TileSet.END_TILE:
									if (this.biome == BiomeSet.FOREST)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestEnd;
									else if (this.biome == BiomeSet.SWAMP)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_SwampEnd;
									else if (this.biome == BiomeSet.CAVE)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_CaveEnd;
									break;
								case TileSet.KEY_TILE:
									// Create the key item here!
									GameObject tempItem = Instantiate(GameObject.Find("WorldItem"), new Vector2(x * offsetforTiles, y * offsetforTiles), Quaternion.identity);
									tempItem.GetComponent<SpriteRenderer>().sprite = this.spr_Key;
									tempItem.GetComponent<SpriteRenderer>().sortingOrder = 1;
									tempItem.GetComponent<Item>().itemType = ItemType.KEY;
									this.map[x, y].item = tempItem;

									// Now set the type!
									if (this.biome == BiomeSet.FOREST)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_ForestKeyTile;
									else if (this.biome == BiomeSet.SWAMP)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_SwampKeyTile;
									else if (this.biome == BiomeSet.CAVE)
										tile.GetComponent<SpriteRenderer>().sprite = this.spr_CaveKeyTile;
									break;
							}

							this.map[x, y].worldTile = tile;
                        }
                        break;
                    case 1: // Prints enum names
                        Debug.Log(this.map[x, y].tileType + " ");
                        break;
                    case 2: // Prints short values without empty spaces
                        if (this.map[x, y].tileType != TileSet.NOTHING)
                            Debug.Log("[" + (short)this.map[x, y].tileType + "]");
                        // Instantiate(Floor, new Vector2(x * 10, y * 10), Quaternion.identity);
                        break;
                }
            }
            
        }
    }

    public void printRecords()
	{
		Debug.Log("======= Records =======");
       
		// Grid-Centric
		Debug.Log("Total Tiles: "+this.tileCounter);
		Debug.Log("Total Walls: "+this.wallCounter);
		Debug.Log("Total Spawns: "+this.spawnCounter);
		Debug.Log("Total Traps: "+this.trapCounter);
		Debug.Log("Total Items: "+this.itemCounter);
		Debug.Log("Total Dig Tiles: "+this.digCounter);

        Debug.Log("\n");

		// Concept-centric
		Debug.Log("Total Offshoots: "+this.offshootCounter);
		Debug.Log("Total Rooms: "+this.roomCounter);
		Debug.Log("Total Hallways: "+this.hallwayCounter);
		Debug.Log("Total Tunnels: "+this.tunnelCounter);
		Debug.Log("Total Secrets: "+this.secretCounter);
		Debug.Log("Total Puzzles: "+this.puzzleCounter);
	}

	//!&!&!&!&!&!&!&!&!&!&!&!&!&!&!&!&!&!&!&!&!//
	//        For testing purposes only        //
	//!&!&!&!&!&!&!&!&!&!&!&!&!&!&!&!&!&!&!&!&!//
	/*
	public static void Main()
	{
		// Create the generator with board size
		BoardGenerator board_gen = new BoardGenerator(1000, 1000);
		
		// Finally, generate
		board_gen.generate();

		// Testing
		board_gen.printRecords();
		Debug.Log();
		board_gen.printMap(2);
	}
	*/
}