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

using System;

//**********************************//
//     Specific Data Structures     //
//**********************************//
public enum TileSet : short
{
	NOTHING = -1, FLOOR = 0,
	WALL, HALLWAY, TRAP, SPAWNER, SECRET_FLOOR,
	PUZZLE_FLOOR, ROCK, MUD, BOULDER, DIG_TILE,
	PUZZLE_HALLWAY, START_TILE, END_TILE
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
}

public struct GridCell
{
	public TileSet tileType;
	public Entity entity;
	public Item item;
}

public class Entity
{
	// Placeholder
}

public class Item
{
	// Placeholder
}

//**************************************************//
// The map array shall be accessed in this fashion: //
//        map[x, y]                                 //
//**************************************************//
public class BoardGenerator
{	
	//**********************************//
	//      Parameters & Variables      //
	//**********************************//
	private int board_width, board_height;
	private int min_hallway_length, max_hallway_length;
	private bool hallwayTurns;
	private bool centeredRooms;
	private int min_tunnel_length, max_tunnel_length;
	private int min_tunnel_width, max_tunnel_width;
	private int min_room_width, max_room_width;
	private int min_room_height, max_room_height;
	private int min_room_count, max_room_count;
	private int min_secret_room_count, max_secret_room_count;
	private bool createdPuzzleRoom;
	private int offshoot_chance, secret_chance, tunnel_chance, puzzle_chance;
	private int direction_chance, trap_chance, spawn_chance;
	private int general_item_chance, rare_item_chance;
	private int dungeonDepth;
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
		setDungeonDepth(1);

		// Default parameters here
		setDefaultParameters();
	}

	BoardGenerator(int width, int height)
	{
		setBoardSize(width, height);
		setDungeonDepth(1);

		// Default parameters here
		setDefaultParameters();
	}

	BoardGenerator(int width, int height, int depth)
	{
		setBoardSize(width, height);
		setDefaultParameters();

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

	public void setRoomParams(int minWidth, int minHeight, int maxWidth, int maxHeight, bool centered)
	{
		this.min_room_width = minWidth;
		this.min_room_height = minHeight;
		this.max_room_width = maxWidth;
		this.max_room_height = maxHeight;
		this.centeredRooms = centered;
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

	public void setTileParams(int trapChance, int spawnChance)
	{
		this.trap_chance = trapChance;
		this.spawn_chance = spawnChance;
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
	}

	public void setDefaultParameters()
	{
		setDungeonDepth(1);
		setBoardParams(7, 12, 1, 999);
		setRoomParams(5, 10, 5, 10, true);
		setHallwayParams(6, 12, false);
		setTunnelParams(6, 12, 2, 5);
		setOffshootParams(5, 15, 5, 30);
		setTileParams(50, 50);
		setMiscParams(40, 70, 100);
	}

	//**********************************//
	//          Helper Methods          //
	//**********************************//
	public static int random(int x)
	{
		Random rand = new Random();
		
		if (x < 0)
			x *= -1;

		return rand.Next(0, x);
	}

	public static int random_range(int a, int b)
	{
		Random rand = new Random();

		// We've gotta be safe
		if (a < b)
			return rand.Next(a, b);
		else
			return rand.Next(b, a);
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
			lBound = -(int)Math.Floor((double)width/2);
			rBound = (int)Math.Ceiling((double)width/2);
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
						this.map[m.x + j, m.y + k].tileType = TileSet.DIG_TILE;
						this.tileCounter++;
						this.digCounter++;
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
						this.map[m.x + j, m.y + k].tileType = TileSet.DIG_TILE;
						this.tileCounter++;
						this.digCounter++;
					}
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

	// Done [ ]
	private void room(Position p)
	{
		Position m = p;
		int _width = random_range(this.min_room_width, this.max_room_width);
		int _height = random_range(this.min_room_height, this.max_room_height);

		int lBound, rBound, uBound, dBound;

		if (this.centeredRooms)
		{
			// Create rooms around x and y
			lBound = -(int) Math.Floor((double) _width/2);
			rBound = (int) Math.Ceiling((double) _width/2);
			uBound = -(int) Math.Floor((double) _height/2);
			dBound = (int) Math.Ceiling((double) _height/2);
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

				// Set down the tile we chose if nothing is there
				if (this.map[m.x + xx, m.y + yy].tileType == TileSet.NOTHING || this.map[m.x + xx, m.y + yy].tileType == TileSet.HALLWAY)
				{
					this.map[m.x + xx, m.y + yy].tileType = type;
					this.tileCounter++;

					if (type == TileSet.TRAP)
						this.trapCounter++;
					else if (type == TileSet.SPAWNER)
						this.spawnCounter++;
				}

				// Place item in the room
				if (chance(this.general_item_chance))
				{
					this.map[m.x + xx, m.y + yy].item = new Item();
					this.itemCounter++;
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
		int _width = random_range(this.min_room_width, this.max_room_width);
		int _height = random_range(this.min_room_height, this.max_room_height);

		int lBound, rBound, uBound, dBound;

		if (this.centeredRooms)
		{
			// Create rooms around x and y
			lBound = -(int) Math.Floor((double) _width/2);
			rBound = (int) Math.Ceiling((double) _width/2);
			uBound = -(int) Math.Floor((double) _height/2);
			dBound = (int) Math.Ceiling((double) _height/2);
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

				// Set down the tile we chose if nothing is there
				if (this.map[m.x + xx, m.y + yy].tileType == TileSet.NOTHING || this.map[m.x + xx, m.y + yy].tileType == TileSet.HALLWAY)
				{
					this.map[m.x + xx, m.y + yy].tileType = type;
					this.tileCounter++;

					if (type == TileSet.TRAP)
						this.trapCounter++;
					else if (type == TileSet.SPAWNER)
						this.spawnCounter++;
				}

				// Place item in the room
				if (chance(this.rare_item_chance))
				{
					this.map[m.x + xx, m.y + yy].item = new Item();
					this.itemCounter++;
				}
			}
		}

		this.roomCounter++;
		this.secretCounter++;
	}

	private void puzzle_room(Position p)
	{
		// TODO: The FUCKING MESS that will be the puzzle room method
		this.puzzleCounter++;
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

					// First, make sure that we're placing walls around a tile
					if (this.map[x, y].tileType != TileSet.NOTHING && this.map[x, y].tileType != TileSet.WALL)
					{
						// Then, check the 8 positions around it (technically including itself)
						for (int xx = -1; xx <= 1; xx++)
							for (int yy = -1; yy <= 1; yy++)
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
						//Console.Write("["+(short)this.map[x,y].tileType+"]");
						if (this.map[x,y].tileType == TileSet.NOTHING)
							Console.Write(" ");
						else
							Console.Write((short)this.map[x,y].tileType);
						break;
					case 1: // Prints enum names
						Console.Write(this.map[x,y].tileType+" ");
						break;
					case 2: // Prints short values without empty spaces
						if (this.map[x, y].tileType != TileSet.NOTHING)
							Console.Write("["+(short)this.map[x,y].tileType+"]");
						break;
				}
			}

			Console.WriteLine();
		}
	}

	public void printRecords()
	{
		Console.WriteLine("======= Records =======");

		// Grid-Centric
		Console.WriteLine("Total Tiles: "+this.tileCounter);
		Console.WriteLine("Total Walls: "+this.wallCounter);
		Console.WriteLine("Total Spawns: "+this.spawnCounter);
		Console.WriteLine("Total Traps: "+this.trapCounter);
		Console.WriteLine("Total Items: "+this.itemCounter);
		Console.WriteLine("Total Dig Tiles: "+this.digCounter);

		Console.WriteLine();

		// Concept-centric
		Console.WriteLine("Total Offshoots: "+this.offshootCounter);
		Console.WriteLine("Total Rooms: "+this.roomCounter);
		Console.WriteLine("Total Hallways: "+this.hallwayCounter);
		Console.WriteLine("Total Tunnels: "+this.tunnelCounter);
		Console.WriteLine("Total Secrets: "+this.secretCounter);
		Console.WriteLine("Total Puzzles: "+this.puzzleCounter);
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
		Console.WriteLine();
		board_gen.printMap(2);
	}
	*/
}