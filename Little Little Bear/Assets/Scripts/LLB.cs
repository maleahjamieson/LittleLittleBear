using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LLB : BasicEntity
{
    private int horizontal = 0; //store direction we are moving
    private int vertical = 0;
    private char damageType;
    public int DungeonDepth;
    private int aDirX = 1, aDirY = 0; // Attack direction x and y, how we aim
    private char attackDir; // char hold, for Combat();
    public int invEquipped; // 0 melee, 1 range, 2 first item, 3 second item
    public char weaponType; // b=blunt, t=thrust, s=slice, r=ranged
    private float seconds; // time of flight for projectile
    private float timer; // delta.time
    private float percent; // time / seconds
    private float scroll; // scroll input
    private Vector2 playerLocation;
    private Vector2 enemyLocation;
    private Vector2 difference; // between llb and enemy

    // List of personal boolean
    private bool specialA; // special attack
    private bool dig;
    private bool turnEnd;
    private bool attackWait;
    private bool rangeWait;
    private bool inCombat; // damage flash management 
    private bool checkInput; // If true we can accept user input, avoids interrupting animation
   	private bool sUp; // scroll up
   	private bool sDown; // scroll down

   // private bool seedXFlip;
   // private bool seedYFlip;

    /*//inventory variables
    private Inventory inventory;
    public GameObject itemButton;*/

    //public int stamina;  // Special move gague, when below a certain amount you cant use special

    public Highlight targetHighlight;  // Targeting script
    public GameObject projectile;
    public int stamina;  // Special move gague, when below a certain amount you cant use special
    public int ammo; // amount of seeds
    public bool staminaUsed;
    public GameObject seedButton;
    public Text ammoCounter;

    protected override void Start()
    {
        turnEnd = false;
        dig = false;
        attackWait = false;
        rangeWait = false;
        inCombat = false;
        checkInput = true;
        staminaUsed = false;
        invEquipped = 0; // Start on weapon slot
        range = 10; // base range on range weapon. I dont know if this will ever change
        attackDir = 'r';
        weaponType = 'b'; // Start with a carrot which is blunt
        maxHealth = 100;
        health = maxHealth;
        stamina = 100;
        strength = 4;
        ammo = 0;
        seconds = 1;  
        projectile = GameObject.Find("Projectile");
        projectile.SetActive(false);


        PlayerData playerData = GameObject.Find("GlobalManager").GetComponent<GlobalMan>().data;
        if (playerData.health > 0 )
        {
            health = playerData.health;
            stamina = playerData.stamina;
            strength = playerData.strength;
            maxHealth = playerData.maxHealth;

            Inventory inv = gameObject.GetComponent<Inventory>();
            //inv.isFull = playerData.isFull;
            inv.items = playerData.items;

	        //setting 2nd slot to sunflower seeds always
	        inv.isFull[1] = true;
	        seedButton = Instantiate(GameObject.Find("ButtonItem"), inv.slots[1].transform, false);
	        seedButton.GetComponent<Item>().itemType = ItemType.SUNFLOWER_SEED;
	        seedButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/Items/SunflowerSeed");
	        ammoCounter.text = "Ammo: " + ammo;
            
            foreach (InventoryItem ii in playerData.items)
            {
                if (ii.type != ItemType.NOTHING)
                {
                    for (int i = 0; i < inv.slots.Length; i++) //checking if inventory is full
                    {
                        // if (playerData.isFull[i])    //not full, pickup item
                        if (!inv.isFull[i])
                        {
                            //if item is blueberry then this
                            GameObject button = Instantiate(GameObject.Find("ButtonItem"), inv.slots[i].transform, false);
                            Debug.Log("Trying to put type "+ii.type+" in inventory");
                            switch (ii.type)
                            {
                                case ItemType.RED_ANTS_BOTTLE:
                                    button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/Items/RedAntsBottle");
                                    inv.isFull[i] = true;
                                    break;
                                default:
                                    inv.isFull[i] = false;
                                    Destroy(button);
                                    break;
                                case ItemType.BLUEBERRIES:
                                    button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/Items/Blueberries");
                                    inv.isFull[i] = true;
                                    break;
                                case ItemType.POCKETKNIFE:
                                    button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/Items/PocketKnife");
                                    inv.isFull[i] = true;
                                    break;
                                case ItemType.RAPIER:
                                    button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/Items/Rapier");
                                    inv.isFull[i] = true;
                                    break;
                                case ItemType.SKUNK_GAS:
                                    button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/Items/SkunkGas");
                                    inv.isFull[i] = true;
                                    break;
                                case ItemType.SNAPS:
                                    button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/Items/Snaps");
                                    inv.isFull[i] = true;
                                    break;
                                case ItemType.STICK_ROCK:
                                    button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/Items/StickRock");
                                    inv.isFull[i] = true;
                                    break;
                                case ItemType.SUNFLOWER_SEED:
                                    button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/Items/SunflowerSeed");
                                    inv.isFull[i] = true;
                                    break;
                                case ItemType.THORN_VINE:
                                    button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/Items/ThornVines");
                                    inv.isFull[i] = true;
                                    break;
                                case ItemType.CARROT:
                                    button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/Items/Carrot");
                                    inv.isFull[i] = true;
                                    break;
                                case ItemType.TREAT:
                                    button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/Items/Treat"); // board.spr_Treat;
                                    inv.isFull[i] = true;
                                    break;
                            }

                            break;
                        }
                        // else
                        // {
                        //     inv.isFull[i] = false;
                        // }
                    }
                }
            }
        }

        else
        {
            
            maxHealth = 100;
            health = maxHealth;
            stamina = 100;
            strength = 4;

            Inventory inv = gameObject.GetComponent<Inventory>();
	        //setting 2nd slot to sunflower seeds always
	        inv.isFull[1] = true;
	        seedButton = Instantiate(GameObject.Find("ButtonItem"), inv.slots[1].transform, false);
	        seedButton.GetComponent<Item>().itemType = ItemType.SUNFLOWER_SEED;
	        seedButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/Items/SunflowerSeed");
	        ammoCounter.text = "Ammo: " + ammo;
        }
        targetHighlight = GameObject.Find("Highlight").GetComponent<Highlight>();
        base.Start();
        //seedXFlip = false;
        //seedYFlip = false;
        //inventory = GetComponent<Inventory>();
        //health = GameManager.instance.playerHealth; // grab loaded health

    }
    
    private void OnEnable() // When level starts up we enable the player entity
    {
        flipped = false;
    }

    private void OnDisable() // When object is disabled
    {
        //GameManager.instance.playerHealth = health; // applies when we change levels, do this for all stats
    }

    private GameObject ReturnEnemy(int x, int y)
    {
        GameObject tempEnemy = board.map[x, y].entity;
        return tempEnemy;
    }
    
    private IEnumerator Combat(bool special) // basic : special = false, direction = d 
    {
        Debug.Log("Combat");
        bool flash = false; // currently, pretty sure we dont need this 
        GameObject[] enemyList;
        int r;// number of enemies to attack
	    if(invEquipped == 0) // melee
	    {
	        if (special)
	         {
	            if (stamina > 30)
	            {
	                stamina -= 30;
	                staminaUsed = true;
	                if (weaponType == 's')
	                {
	                    r = 5;
	                    enemyList = new GameObject[r];
	                    switch (attackDir)
	                    {
	                        case 'l': // left
	                            enemyList[0] = ReturnEnemy(currentX - 1, currentY);
	                            enemyList[1] = ReturnEnemy(currentX - 1, currentY + 1);
	                            enemyList[2] = ReturnEnemy(currentX, currentY + 1);
	                            enemyList[3] = ReturnEnemy(currentX - 1, currentY - 1);
	                            enemyList[4] = ReturnEnemy(currentX, currentY - 1);
	                            break;
	                        case 'r': // right
	                            enemyList[0] = ReturnEnemy(currentX + 1, currentY);
	                            enemyList[1] = ReturnEnemy(currentX + 1, currentY + 1);
	                            enemyList[2] = ReturnEnemy(currentX, currentY + 1);
	                            enemyList[3] = ReturnEnemy(currentX + 1, currentY - 1);
	                            enemyList[4] = ReturnEnemy(currentX, currentY - 1);
	                            break;
	                        case 'u': // up
	                            enemyList[0] = ReturnEnemy(currentX, currentY + 1);
	                            enemyList[1] = ReturnEnemy(currentX - 1, currentY + 1);
	                            enemyList[2] = ReturnEnemy(currentX - 1, currentY);
	                            enemyList[3] = ReturnEnemy(currentX + 1, currentY + 1);
	                            enemyList[4] = ReturnEnemy(currentX + 1, currentY);
	                            break;
	                        case 'd': // down
	                            enemyList[0] = ReturnEnemy(currentX, currentY - 1);
	                            enemyList[1] = ReturnEnemy(currentX - 1, currentY - 1);
	                            enemyList[2] = ReturnEnemy(currentX - 1, currentY);
	                            enemyList[3] = ReturnEnemy(currentX + 1, currentY - 1);
	                            enemyList[4] = ReturnEnemy(currentX + 1, currentY);
	                            break;
	                    }

	                }
	                else
	                {

	                    if (weaponType == 'b')
	                        r = 1;
	                    else
	                        r = 2;

	                    enemyList = new GameObject[r];
	                    switch (attackDir)
	                    {
	                        case 'l':
	                            for (int i = 0; i < r; i++)
	                            {
	                                enemyList[i] = ReturnEnemy(currentX - (i + 1), currentY);
	                            }
	                            break;
	                        case 'r':
	                            for (int i = 0; i < r; i++)
	                            {
	                                enemyList[i] = ReturnEnemy(currentX + (i + 1), currentY);
	                            }
	                            break;
	                        case 'u':
	                            for (int i = 0; i < r; i++)
	                            {
	                                enemyList[i] = ReturnEnemy(currentX, currentY + (i + 1));
	                            }
	                            break;
	                        case 'd':
	                            for (int i = 0; i < r; i++)
	                            {
	                                enemyList[i] = ReturnEnemy(currentX, currentY - (i + 1));
	                            }
	                            break;
	                    }

	                    
	                }
	                for (int i = 0; i < r; i++)
	                {
	                    if (enemyList[i] != null)
	                    {
	                        if (weaponType == 'b') // blunt
	                        {
	                            Debug.Log("Blunt");
	                            StartCoroutine(enemyList[i].GetComponent<EnemyBasic>().Hurt(this.strength * 2, 1)); // Inflict damage
	                            enemyList[i].GetComponent<EnemyBasic>().stunned = true;
	                            enemyList[i].GetComponent<EnemyBasic>().stunnedTurns = 1; // For now only 1
	                            yield return new WaitForSeconds(0.5f);
	                        }
	                        else if (weaponType == 't')
	                        {
	                            Debug.Log("Thrust");
	                            StartCoroutine(enemyList[i].GetComponent<EnemyBasic>().Hurt(this.strength, Random.Range(2, 5))); // Inflict damage 2-4 times
	                            while (enemyList[i].GetComponent<EnemyBasic>().flash)
	                                yield return new WaitForSeconds(0f);
	                        }
	                        else //thust
	                        {

	                            Debug.Log("Slice");
	                            StartCoroutine(enemyList[i].GetComponent<EnemyBasic>().Hurt(this.strength, 1)); // Inflict damage 2-4 times
	                            while (enemyList[i].GetComponent<EnemyBasic>().flash)
	                                yield return new WaitForSeconds(0f);
	                        }
	                    }
	                    else
	                    {
	                        Debug.Log("NULL");
	                    }
	                }
	            } 
	         }
	         else
	         {
	            GameObject enemy = null;
	            switch (attackDir)
	            {
	                case 'l':
	                    enemy = board.map[currentX - 1, currentY].entity;
	                    break;
	                case 'r':
	                    enemy = board.map[currentX + 1, currentY].entity;
	                    break;
	                case 'u':
	                    enemy = board.map[currentX, currentY + 1].entity;
	                    break;
	                case 'd':
	                    enemy = board.map[currentX, currentY - 1].entity;
	                    break;
	            }
	           
	            if (enemy != null)
	            {
	                StartCoroutine(enemy.GetComponent<EnemyBasic>().Hurt(this.strength, 1)); // Inflict damage
	            }
	            else
	            {
	                Debug.Log("NULL");
	            }
	         }
	    }
	    else // Range attack calc
	    {
	    	bool missed = true; // base case is missing
	    	Vector2 missPos = new Vector2(0,0); // in case the player misses
	    	ammo -= 1; // use one ammo
	    	ammoCounter.text = "Ammo: " + ammo;
	    	r = 10;
	    	enemyList = new GameObject[r];
            switch (attackDir)
            {
                case 'l':
                	/*if(!seedXFlip)
                	{
                		seedXFlip = true;
                		projectile.GetComponent<SpriteRenderer>().flipX = true;
                	}*/
                	missPos = new Vector2 (this.transform.position.x - 10, this.transform.position.y);
                    for (int i = 0; i < r; i++)
                    {
                        enemyList[i] = ReturnEnemy(currentX - (i + 1), currentY);
                    }
                    break;
                case 'r':
                	/*if(seedXFlip)
                	{
                		seedXFlip = false;
                		projectile.GetComponent<SpriteRenderer>().flipX = false;
                	}*/
                	missPos = new Vector2 (this.transform.position.x + 10, this.transform.position.y);
                    for (int i = 0; i < r; i++)
                    {
                        enemyList[i] = ReturnEnemy(currentX + (i + 1), currentY);
                    }
                    break;
                case 'u':
                    /*if(seedYFlip)
                	{
                		seedYFlip = false;
                		projectile.GetComponent<SpriteRenderer>().flipY = false;
                	}*/
                	missPos = new Vector2 (this.transform.position.x, this.transform.position.y + 10);
                    for (int i = 0; i < r; i++)
                    {
                        enemyList[i] = ReturnEnemy(currentX, currentY + (i + 1));
                    }
                    break;
                case 'd':
                	/*if(!seedYFlip)
                	{
                		seedYFlip = true;
                		projectile.GetComponent<SpriteRenderer>().flipY = true;
                	}*/
                	missPos = new Vector2 (this.transform.position.x, this.transform.position.y - 10);
                    for (int i = 0; i < r; i++)
                    {
                        enemyList[i] = ReturnEnemy(currentX, currentY - (i + 1));
                    }
                    break;
            }
            for (int i = 0; i < r; i++)
            {
                if (enemyList[i] != null)
                {
                	missed = false; // hit something
                	seconds = 0.2f * (i+1); // per tile .2 seconds
                    Debug.Log("Range from: " + this.transform.position + " to: " + enemyList[i].transform.position);
                    enemyLocation = enemyList[i].transform.position;
                    Debug.Log("Before " + projectile.transform.position);
                    //rangeWait = true;
                    projectile.SetActive(true);
                    while(rangeWait) // wait for projectile
						yield return null;
					projectile.SetActive(false);
					Debug.Log("After " + projectile.transform.position);
					enemyList[i].GetComponent<EnemyBasic>().makeAlert(); // piss the enemy off
                    
                    StartCoroutine(enemyList[i].GetComponent<EnemyBasic>().Hurt(2 + board.getDepth(), 1)); // Inflict damage
                    yield return new WaitForSeconds(0.5f);  
                    break;
                }

			}

			if(missed)
			{
				enemyLocation = missPos;
				projectile.SetActive(true);
				while(rangeWait) // wait for projectile
						yield return null;
				projectile.SetActive(false);
			}

        }
        inCombat = false;
    }

    private void Dig(int x, int y)
    {
        if (board.map[x, y].worldTile != null)
        {
            if (board.map[x, y].tileType == TileSet.DIG_TILE)
            {
                // Testing with 90% item spawn chance on dig -> TODO: set spawn chance back to <50%
                if (Random.value <= 0.35f) // 35% chance
                {
                    board.spawnItem(x, y);
                    // PickUp(board.map[x, y].item); // Why doesn't this work??
                }

                // Change the sprite of the worldTile that's there
                if (board.getBiome() == BiomeSet.FOREST)
                    board.map[x, y].worldTile.GetComponent<SpriteRenderer>().sprite = board.spr_ForestFloor;
                else if (board.getBiome() == BiomeSet.SWAMP)
                    board.map[x, y].worldTile.GetComponent<SpriteRenderer>().sprite = board.spr_SwampFloor;
                else if (board.getBiome() == BiomeSet.CAVE)
                    board.map[x, y].worldTile.GetComponent<SpriteRenderer>().sprite = board.spr_CaveFloor;

                board.map[x, y].tileType = TileSet.FLOOR;
            }
        }
    }

    public void AttachSpriteToPosition()
    {
        // Let's fix the misalligning manually
        if (transform.position.x != currentX)
            transform.position = new Vector2(currentX, transform.position.y);
        if (transform.position.y != currentY)
            transform.position = new Vector2(transform.position.x, currentY);
    }

    private bool Move(int xDir, int yDir) // out let us return multiple values
    {
        board = GameObject.Find("LevelTilesGenerator").GetComponent<gameManager>().board;

        if (board.map[xDir, yDir].entity == null) // if nothing is there(for now)
        {
            int xDiff = 0;
            int yDiff = 0;
            switch (board.map[xDir, yDir].tileType)
            {
                // Moveable boulders
                case TileSet.BOULDER:
                    xDiff = xDir - currentX;
                    yDiff = yDir - currentY;

                    if (xDiff != 0 || yDiff != 0)
                    {
                        if (board.map[xDir + xDiff, yDir + yDiff].tileType != TileSet.BOULDER &&
                            board.map[xDir + xDiff, yDir + yDiff].tileType != TileSet.WALL &&
                            board.map[xDir + xDiff, yDir + yDiff].tileType != TileSet.TUNNEL &&
                            board.map[xDir + xDiff, yDir + yDiff].tileType != TileSet.DIG_TILE)
                        {
                            board.map[xDir + xDiff, yDir + yDiff].tileType = TileSet.BOULDER;
                            board.map[xDir + xDiff, yDir + yDiff].worldTile.GetComponent<SpriteRenderer>().sprite = board.spr_CaveBoulder;
                            board.map[xDir, yDir].tileType = TileSet.FLOOR;
                            board.map[xDir, yDir].worldTile.GetComponent<SpriteRenderer>().sprite = board.spr_CaveFloor;
                            board.map[xDir, yDir].entity = board.map[currentX, currentY].entity;
                            board.map[currentX, currentY].entity = null;
                            currentX = xDir;
                            currentY = yDir;

                            return true;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                    // break;

                // Collision with solid, immobile walls
                case TileSet.ROCK:
                case TileSet.WALL:
                    // Do nothing
                    return false;

                // Falling into forest puzzle pit
                case TileSet.PIT:
                    board.respawnPuzzle();
                    StartCoroutine(Hurt(10, 1));
                    return false;
                    // break;

                // Moving on normal tiles -> default movement
                default:
                    Debug.Log("**** HEY I'M MOVING ****");
                    board.map[xDir, yDir].entity = board.map[currentX, currentY].entity;
                    board.map[currentX, currentY].entity = null;

                    xDiff = xDir - currentX;
                    yDiff = yDir - currentY;
                    Debug.Log("XDIFF: "+xDiff);
                    Debug.Log("YDIFF: "+yDiff);


                    // Before we change positions, see if we need to change a branch to a pit
                    if (board.map[currentX, currentY].tileType == TileSet.BRANCH)
                    {
                        board.map[currentX, currentY].tileType = TileSet.PIT;
                        board.map[currentX, currentY].worldTile.GetComponent<SpriteRenderer>().sprite = board.spr_ForestPit;
                    }

                    // Move to new position
                    currentX = xDir;
                    currentY = yDir;

                    if (board.map[currentX, currentY].tileType == TileSet.MUD)
                    {
                        // This should keep sliding us until we can't move anymore
                        // if (xDiff != 0 && yDiff != 0)
                        {
                            Debug.Log("SLIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIDING");

                            while (board.map[currentX + xDiff, currentY + yDiff].tileType == TileSet.MUD)
                            {
                                board.map[currentX + xDiff, currentY + yDiff].entity = board.map[currentX, currentY].entity;
                                board.map[currentX, currentY].entity = null;
                                currentX += xDiff;
                                currentY += yDiff;
                            }
                        }
                    }

                    // DO NOT MOVE THIS OUT OF HERE
                    if (board.map[currentX, currentY].tileType == TileSet.TUNNEL)
                    {
                        board.map[currentX, currentY].tileType = TileSet.FLOOR;

                        if (Random.value < 0.05f) // 5% chance to spawn
                        {
                            Debug.Log("Item would be spawned from tunnel");
                            board.spawnItem(currentX, currentY);
                        }

                        // Change the sprite of the worldTile that's there
                        if (board.getBiome() == BiomeSet.FOREST)
                            board.map[currentX, currentY].worldTile.GetComponent<SpriteRenderer>().sprite = board.spr_ForestFloor;
                        else if (board.getBiome() == BiomeSet.SWAMP)
                            board.map[currentX, currentY].worldTile.GetComponent<SpriteRenderer>().sprite = board.spr_SwampFloor;
                        else if (board.getBiome() == BiomeSet.CAVE)
                            board.map[currentX, currentY].worldTile.GetComponent<SpriteRenderer>().sprite = board.spr_CaveFloor;

                        return true;
                    }

                    //written by Maleah, pickup item for inventory
                    //check ground for item
                    if (board.map[xDir, yDir].item != null)
                    {
                        //try to pickup item
                        PickUp(board.map[xDir, yDir].item);
                    }
                    //written by Thomas, change the level
                    if (board.map[xDir, yDir].tileType == TileSet.END_TILE)
                    {
                        gameManager.instance.dungeonDepth++;
                        SaveData.SavePlayer(gameManager.instance.LLB.GetComponent<LLB>(), gameManager.instance.LLB.GetComponent<Inventory>());
                        GlobalMan.instance.data = SaveData.LoadPlayer();
                        gameManager.instance.LoadScene(1);
                    }
                    return true;
            }
        }
        else //something is there
        {            
            Debug.Log("CONTAINS " + board.map[xDir, yDir].entity);
            return false;
        }

        // return true; // If nothing is hit then assume move
    }

    private void PickUp(GameObject item)   // Picks up the item off the floor (In the future we can add UI)
    {
        item.GetComponent<Item>().pickup();
        ammoCounter.text = "Ammo: " + ammo;
        /*
        Debug.Log("Running pickup function");

        for(int i = 0; i < inventory.slots.Length; i++) //checking if inventory is full
        {
            if(inventory.isFull[i] == false)    //not full, pickup item
            {
                Debug.Log("Picked up item");
                //add item
                inventory.isFull[i] = true;
                
                //if item is blueberry then this
                Instantiate(itemButton, inventory.slots[i].transform, false);
                Destroy(item);
                break;
            }
        }
        */
    }

    private void Update()
    {
        if (health <= 0) {
            SceneManager.LoadScene("StartScene");
        }
    
        if (checkInput && active) //No previous actions are being executed
        {
        	if(timer != 0)
        		timer = 0;

        	scroll = Input.GetAxis("Mouse ScrollWheel"); 
        	if (scroll > 0f) // up
        	{	
        		sUp = true;
        		checkInput = false;
        		Debug.Log("UP");
        	}
        	else if (scroll < 0f) // down
        	{
        		sDown = true;
        		checkInput = false;
        		Debug.Log("DOWN");
        	}


            if (Input.GetMouseButtonDown(0)) // left mouse click
            {
            	if (invEquipped == 0)
            	{
            		attack = true; // player will attempt to attack
        			checkInput = false; //input has been read	 
            	}
            	else if (invEquipped == 1 && ammo > 0)
            	{
            		attack = true; // player will attempt to attack
        			checkInput = false; //input has been read	
            	}

            }
            if (Input.GetMouseButtonDown(1)) // right mouse click
            { 
            	if(invEquipped == 0){
	                if (stamina > 30)
	                {
	                    specialA = true; // player will attempt to special attack
	                    checkInput = false; //input has been read
	                }
	                else
	                    Debug.Log("Stamina is too low");
                }
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                dig = true;
                checkInput = false;
            }

            //movement vector input
            horizontal = (int)Input.GetAxisRaw("Horizontal"); //using ints forces 1 unit movement
            vertical = (int)Input.GetAxisRaw("Vertical");

            if (horizontal != 0) //No diagonal
                vertical = 0;

            if (horizontal != 0 || vertical != 0) // There must be movement input
            {
                Debug.Log("*******LLB*******");
                if (Move(currentX + horizontal, currentY + vertical)) //Current location + moveVector
                {
                    checkInput = false;

                    if (horizontal == -1)  // Identifies direction to move and then flips boolean to take action
                        moveLeft = true;
                    else if (horizontal == 1)
                        moveRight = true;
                    else if (vertical == -1)
                        moveDown = true;
                    else
                        moveUp = true;
                }

            }
        }
        else if (attackWait) // Waiting for decided direction
        {
            aDirX = (int)Input.GetAxisRaw("Horizontal"); //using ints forces 1 unit movement
            aDirY = (int)Input.GetAxisRaw("Vertical");
            if (aDirX != 0 || aDirY != 0) // There must be movement input
            {
                if (aDirX == -1)  // Identifies direction
                {
                    targetHighlight.Aim('l');
                    attackDir = 'l';
                    if (!flipped)
                    {
                        Vector2 tempS = transform.localScale;
                        tempS.x *= -1;  // Flips sprite
                        transform.localScale = tempS;
                        flipped = true;
                    }
                }
                else if (aDirX == 1)
                {
                    targetHighlight.Aim('r');
                    attackDir = 'r';
                    if (flipped)
                    {
                        Vector2 tempS = transform.localScale;
                        tempS.x *= -1;  // Flips sprite
                        transform.localScale = tempS;
                        flipped = false;
                    }
                }
                else if (aDirY == -1)
                {
                    targetHighlight.Aim('d');
                    attackDir = 'd';
                }
                else if (aDirY == 1)
                {
                    targetHighlight.Aim('u');
                    attackDir = 'u';
                }
            }
            
            if (Input.GetMouseButtonDown(0)) // Attack
            {
                attackWait = false;
                turnEnd = true;
            }
            if (Input.GetMouseButtonDown(1)) // Bail
            {
                attackWait = false;
            }
            if (!attackWait)
            {
                targetHighlight.Deactivate();
            }
        }
        else if (rangeWait) // Waiting for seed to travel
        {

        	if (timer <= seconds)
        	{
        		timer += Time.deltaTime;
        		percent = timer / seconds;
        		projectile.transform.position = Vector2.Lerp(playerLocation, enemyLocation, percent);
        	}
        	else
        	{
        		rangeWait = false;
        		projectile.SetActive(false);
        	}
        }
    }


    void FixedUpdate() // Where animation and actions take place
    {

        if (attack)
        {
            attack = false;
            attackWait = true;
            if(invEquipped == 0){
	            targetHighlight.Activate(1, flipped, 'o', attackDir); // o = other i.e basic attack
	            StartCoroutine(wait2Move('a', 1.5f)); // Starts animation timer and should stop inputs
        	}
        	else // invEquipped = 1
        	 {
        	 	rangeWait = true;
        		targetHighlight.Activate(10, flipped, 'o', attackDir); // o = other i.e basic attack
	            StartCoroutine(wait2Move('x', 1f)); // Using x since r for range is taken by right move 
        	 }
        }
        else if (specialA)
        {
            int r = 0;
            switch (weaponType)
            {
                case 'b': // bluntslide 180
                    r = 1;
                    break;
                case 's': // slice
                    r = 5;
                    break;
                case 't': // thrust
                    r = 2;
                    break;
            }
            specialA = false;
            attackWait = true;
            targetHighlight.Activate(r, flipped, weaponType, attackDir);
            StartCoroutine(wait2Move('s', 1.5f)); // Starts animation timer and should stop inputs
        }
        else if (dig)
        {
            dig = false;
            animator.SetTrigger("Dig");
            StartCoroutine(wait2Move('i', 2f)); // Start animation and call dig, d was taken, i = item
        }
        else if (sUp)
        {
        	sUp = false;
        	invEquipped += 1;
        	if (invEquipped > 3)
        		invEquipped = 0;
        	StartCoroutine(wait2Move('e', 0.1f));
        }
        else if (sDown)
        {
        	sDown = false;
        	invEquipped -= 1;
        	if (invEquipped < 0)
        		invEquipped = 3;
        	StartCoroutine(wait2Move('e', 0.1f));
        }
        else if (moveLeft)
        {
            moveLeft = false;
            if (!flipped) // LLB looking right about to go left, so we flip her sprite
            {
                Vector2 tempS = transform.localScale;
                tempS.x *= -1;  // Flips sprite
                transform.localScale = tempS;
                flipped = true;
            }
            animator.SetTrigger("WalkX");
            StartCoroutine(wait2Move('l', 0.3f)); //Left
        }
        else if (moveRight) // LLB looking left about to go right so we flip her sprite
        {
            moveRight = false;
            if (flipped)
            {
                Vector2 tempS = transform.localScale;
                tempS.x *= -1;  // Flips sprite
                transform.localScale = tempS;
                flipped = false;
            }
            animator.SetTrigger("WalkX");
            StartCoroutine(wait2Move('r', 0.3f));
        }
        else if (moveUp)
        {
            moveUp = false;
            animator.SetTrigger("WalkUp");
            StartCoroutine(wait2Move('u', 0.3f));
        }
        else if (moveDown)
        {
            moveDown = false;
            animator.SetTrigger("WalkDown");
            StartCoroutine(wait2Move('d', 0.3f));
        }

        horizontal = 0; // Resets input
        vertical = 0;
    }

    private IEnumerator wait2Move(char c, float t) // c is action, t (seconds) is time to wait for action
    {
        switch (c)
        {
            case 'a': //Attack
                {
                    while (attackWait)// Start combat decisions
                        yield return null;
                    if (turnEnd) // if attack actually happened
                    {
                    	switch(weaponType)
                    	{
                    		case 'b':
                    		animator.SetTrigger("Blunt");
                    		break;
                    		case 't':
                    		animator.SetTrigger("Thrust");
                    		break;
                    		case 's':
                    		animator.SetTrigger("Slice");
                    		break;
                    	}
                        yield return new WaitForSeconds(t);
                        inCombat = true;
                        StartCoroutine(Combat(false)); // Once LLB attack animation ends, do damage
                      
                        while (inCombat)
                            yield return new WaitForSeconds(0f); // waits till combat ends
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                break;
            case 's': //Special attack
                {
                    
                    while (attackWait)// Start combat decisions
                        yield return null;
                    if (turnEnd) // if attack actually happened
                    {
                        switch(weaponType)
                    	{
                    		case 'b':
                    		animator.SetTrigger("Blunt");
                    		break;
                    		case 't':
                    		animator.SetTrigger("Thrust");
                    		break;
                    		case 's':
                    		animator.SetTrigger("Slice");
                    		break;
                    	}
                        yield return new WaitForSeconds(t);
                        inCombat = true;
                        StartCoroutine(Combat(true));
                        
                        while (inCombat)
                            yield return new WaitForSeconds(0f); // waits till combat ends
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                break;
            case 'x': // range attack
	            { 
	            	projectile.transform.position = transform.position;
	            	while (attackWait)
	            		yield return null;
	            	if (turnEnd) // if attack actually happened
                    {

        				playerLocation = transform.position;

                        inCombat = true;
                        StartCoroutine(Combat(false)); // Once LLB attack animation ends, do damage
                      
                        while (inCombat)
                            yield return new WaitForSeconds(0f); // waits till combat ends
                        yield return new WaitForSeconds(0.5f);
                    }
	            }
	            break;
            case 'l':  //Move left
                {
                    attackDir = 'l'; // when we move attackDir = left
                    transform.Translate((Vector2.left) / 2); // Splits the difference so its a 2 step
                    yield return new WaitForSeconds(0.1f);
                    transform.Translate((Vector2.left) / 2);
                    AttachSpriteToPosition();
                    yield return new WaitForSeconds(t - 0.1f); // Lagtime after movement is complete
                    turnEnd = true;
                }

                break;
            case 'r': // Move right
                {
                    attackDir = 'r'; // when we move attackDir = right
                    transform.Translate((Vector2.right) / 2);
                    yield return new WaitForSeconds(0.1f);
                    transform.Translate((Vector2.right) / 2);
                    AttachSpriteToPosition();
                    yield return new WaitForSeconds(t - 0.1f);
                    turnEnd = true;
                }
                break;
            case 'u': // Move up
                {
                    attackDir = 'u'; // when we move attackDir = up
                    transform.Translate((Vector2.up) / 2);
                    yield return new WaitForSeconds(0.1f);
                    transform.Translate((Vector2.up) / 2);
                    AttachSpriteToPosition();
                    yield return new WaitForSeconds(t - 0.1f);
                    turnEnd = true;
                }
                break;
            case 'd': // Move Down
                {
                    attackDir = 'd'; // when we move attackDir = down
                    transform.Translate((Vector2.down) / 2);
                    yield return new WaitForSeconds(0.1f);
                    transform.Translate((Vector2.down) / 2);
                    AttachSpriteToPosition();
                    yield return new WaitForSeconds(t - 0.1f);
                    turnEnd = true;
                }
                break;
            case 'i': // find item/dig
                {
                    yield return new WaitForSeconds(t); // Nothing special just wait t seconds
                    Dig(currentX, currentY); // DIG!
                    turnEnd = true;
                }
                break;
            case 'o': //Other, just wait for animation to end
                {
                    yield return new WaitForSeconds(t); // Nothing special just wait t seconds
                    turnEnd = true;
                }
                break;
            case 'e': // empty
            {
            	yield return new WaitForSeconds(t);
            }
            break;
        }
        //GameManager.instance.playersTurn = false; // Action complete, player loses turn enemies go
        //board.moveEnemies(); // Move all of the bad bois
        if (turnEnd) // Ensures action has been made
        {
            yield return StartCoroutine(board.moveEnemies());
            if (!staminaUsed && stamina < 100) // regen stamina
                stamina += 5;
            else
                staminaUsed = false;
        }
        

        turnEnd = false; // Reset after enemies
        checkInput = true; // Inputs are able to be taken again
    }
}
