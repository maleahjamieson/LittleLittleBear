using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    NOTHING = -1,
    RED_ANTS_BOTTLE, BLUEBERRIES, POCKETKNIFE, RAPIER, SKUNK_GAS,
    SNAPS, STICK_ROCK, SUNFLOWER_SEED, THORN_VINE, CARROT,
    TREAT, KEY
};
[System.Serializable]
public struct InventoryItem
{
    public int damage;
    public char damageType;
    public int range;
    public ItemType type;
}

public class Item : MonoBehaviour
{

	//inventory variables
	private Inventory inventory;
	public GameObject itemButton;
    public GameObject LLB; // player
    public ItemType itemType; // a ants, b berry, etc

    public int damage;
    public int ammoCount;
    public char damageType;
    public int range;

    int playerHealth; // LLB health
    int maxHealth; // This will be updated by treats
	// Start is called before the first frame update
    void Start()
    {
        this.damage = 4;
        this.damageType = 'b';
        this.range = 1;
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        LLB = GameObject.Find("Player");
        // playerHealth = LLB.GetComponent<LLB>().health;
    }

    public void generateWeaponStats(int depth)
    {
        // Define the damage type based on the weapon
        switch (this.itemType)
        {
            default:
            case ItemType.STICK_ROCK:
            case ItemType.CARROT:
                this.damageType = 'b';
                break;
            case ItemType.RAPIER:
                this.damageType = 't';
                break;
            case ItemType.POCKETKNIFE:
                this.damageType = 's';
                break;
        }

        // Randomize the damage and add on to the base damage
        int modifier = (int)(depth + (Random.value * (depth/2)));
        this.damage += modifier;

        // Flip
        // while (Random.value < 0.25f)
        //     this.range++;
    }

    public void pickup()
    {
        if(this.itemType == ItemType.SUNFLOWER_SEED) //if seed, add to ammo counter and dont add to inventory
        {
            //adds ammo to counter and destroys object on ground
            LLB.GetComponent<LLB>().ammo += 10;
            ammoCount = LLB.GetComponent<LLB>().ammo;

            Debug.Log("AMMO PLUS 10");
            Destroy(gameObject);
        }
        else
        {
    	    for(int i = 0; i < inventory.slots.Length; i++) //checking if inventory is full
    	    {
    	        if(inventory.isFull[i] == false && this.itemType != ItemType.SUNFLOWER_SEED)    //not full or seed, pickup item
    	        {
    	            Debug.Log("Picked up item");
    	            //add item
    	            inventory.isFull[i] = true;

                    inventory.items[i].damage = this.damage;
                    inventory.items[i].damageType = this.damageType;
                    inventory.items[i].range = this.range;
                    inventory.items[i].type = this.itemType;
    	            
    	            //adds item button to slot
    	            GameObject button = Instantiate(GameObject.Find("ButtonItem"), inventory.slots[i].transform, false);
                    button.GetComponent<Image>().sprite = GetComponent<SpriteRenderer>().sprite;
                    button.GetComponent<Item>().itemType = this.itemType;
    	            Destroy(gameObject);
    	            break;
            	}
        	}
        }
    }
    public void use() 
    {
        for(int i = 0; i < inventory.slots.Length; i++)
        {
            if (inventory.items[i].type == itemType)
            {
                inventory.isFull[i] = false;
                break;
            }
        }
        switch (itemType)
        {
            case ItemType.RED_ANTS_BOTTLE: // ants
                Debug.Log("ants everywhere (no function rn)");
                Destroy(gameObject);
                break;
            case ItemType.BLUEBERRIES: // berry
                Debug.Log("munchin a blueberry");
                if (LLB.GetComponent<LLB>().health >= (LLB.GetComponent<LLB>().maxHealth - 20))
                    LLB.GetComponent<LLB>().health = LLB.GetComponent<LLB>().maxHealth; // Only goes as high as max health
                else
                    LLB.GetComponent<LLB>().health += 20; // increase by 20
                Destroy(gameObject);
                break;
            case ItemType.SKUNK_GAS: // skunk
                Debug.Log("used smelly (no function rn)");
                Destroy(gameObject);
                break;
            case ItemType.TREAT:
                Debug.Log("what a treat");
                LLB.GetComponent<LLB>().maxHealth = LLB.GetComponent<LLB>().maxHealth + 20;
                Destroy(gameObject);
                break;
            case ItemType.STICK_ROCK:
                Debug.Log("used stick rock (no function rn)");
                Destroy(gameObject);
                break;
            case ItemType.RAPIER:
                Debug.Log("used sword (no function rn)");
                Destroy(gameObject);
                break;
            case ItemType.CARROT:
                Debug.Log("used carrot (no function rn)");
                //Destroy(gameObject); no destroy cause carrot is base item
                //in fact we should probably get rid of this
                break;
            case ItemType.POCKETKNIFE:
                Debug.Log("used knife (no function rn)");
                Destroy(gameObject);
                break;
            case ItemType.SNAPS:
                BoardGenerator board = GameObject.Find("LevelTilesGenerator").GetComponent<gameManager>().board;
                int xx = (int)(Random.value * board.getBoardWidth());
                int yy = (int)(Random.value * board.getBoardHeight());

                while (board.map[xx, yy].tileType != TileSet.FLOOR)
                {
                    xx = (int)(Random.value * board.getBoardWidth());
                    yy = (int)(Random.value * board.getBoardHeight());
                }
                int cx, cy;
                cx = LLB.GetComponent<LLB>().currentX;
                cy = LLB.GetComponent<LLB>().currentY;

                board.map[xx, yy].entity = board.map[cx, cy].entity;
                board.map[cx, cy].entity = null;
                LLB.GetComponent<LLB>().currentX = xx;
                LLB.GetComponent<LLB>().currentY = yy;
                LLB.GetComponent<LLB>().AttachSpriteToPosition();

                // Need to free the inventory positions
                Destroy(gameObject);
                break;
            default:
                Debug.Log("UH OH, item isnt set!");
                break;
        }
    }

    
}
