﻿using System.Collections;
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
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        LLB = GameObject.Find("Player");
        int depth = LLB.GetComponent<LLB>().DungeonDepth;
        int modifier;

        switch (this.itemType)
        {
            case ItemType.STICK_ROCK:
            case ItemType.CARROT:
                this.damageType = 'b';
                this.damage = 6;
                this.range = 1;
                modifier = (int)(depth + (Random.value * (depth / 2)));
                this.damage += modifier;
                break;
            case ItemType.RAPIER:
                this.damageType = 't';
                this.damage = 4;
                this.range = 1;
                modifier = (int)(depth + (Random.value * (depth / 2)));
                this.damage += modifier;
                break;
            case ItemType.POCKETKNIFE:
                this.damageType = 's';
                this.damage = 5;
                this.range = 1;
                modifier = (int)(depth + (Random.value * (depth / 2)));
                this.damage += modifier;
                break;
            default:
                break;
        }
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
                //LLB.GetComponent<LLB>().weaponType = 'b';
                break;
            case ItemType.RAPIER:
                this.damageType = 't';
                ///LLB.GetComponent<LLB>().weaponType = 't';
                break;
            case ItemType.POCKETKNIFE:
                this.damageType = 's';
                //LLB.GetComponent<LLB>().weaponType = 's';
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
        else if(this.itemType == ItemType.RAPIER || this.itemType == ItemType.STICK_ROCK || this.itemType == ItemType.CARROT || this.itemType == ItemType.POCKETKNIFE) //if weapon
        {
        	if(!inventory.isFull[0])
        	{
        		//put weapon in first slot and destroy on ground
	            inventory.isFull[0] = true;
	            inventory.items[0].damage = this.damage;
                LLB.GetComponent<LLB>().strength = this.damage;
                inventory.items[0].damageType = this.damageType;
                LLB.GetComponent<LLB>().weaponType = this.damageType;
                inventory.items[0].range = this.range;
	            inventory.items[0].type = this.itemType;
	            //adds item button to slot
	            GameObject button = Instantiate(GameObject.Find("ButtonItem"), inventory.slots[0].transform, false);
	            button.GetComponent<Image>().sprite = GetComponent<SpriteRenderer>().sprite;
	            button.GetComponent<Item>().itemType = this.itemType;
	            LLB.GetComponent<LLB>().weaponType = this.damageType;
	            Destroy(gameObject);
        	}
        }
        else
        {
    	    for(int i = 2; i < inventory.slots.Length; i++) //checking if inventory is full
    	    {
    	        if(inventory.isFull[i] == false && this.itemType != ItemType.SUNFLOWER_SEED)    //not full or seed, pickup item
    	        {
    	            Debug.Log("Picked up item");
    	            //add item
    	            inventory.isFull[i] = true;
                    inventory.items[i].type = this.itemType;

                    if (this.itemType == ItemType.POCKETKNIFE || this.itemType == ItemType.RAPIER || this.itemType == ItemType.STICK_ROCK || this.itemType == ItemType.CARROT)
                        break;
    	            
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
            case ItemType.THORN_VINE: // ants
                Debug.Log("thorns (no function rn)");
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
                if (LLB.GetComponent<LLB>().health >= (LLB.GetComponent<LLB>().maxHealth - 20))
                    LLB.GetComponent<LLB>().health = LLB.GetComponent<LLB>().maxHealth; // Only goes as high as max health
                else
                    LLB.GetComponent<LLB>().health += 20; // increase by 20
                Destroy(gameObject);
                break;
            case ItemType.STICK_ROCK:
                Debug.Log("used stick rock (no function rn)");
                LLB.GetComponent<LLB>().strength = 4;
                LLB.GetComponent<LLB>().weaponType = 'b';
                Destroy(gameObject);
                break;
            case ItemType.RAPIER:
                Debug.Log("used sword (no function rn)");
                LLB.GetComponent<LLB>().strength = 4;
                LLB.GetComponent<LLB>().weaponType = 'b';
                Destroy(gameObject);
                break;
            case ItemType.CARROT:
                Debug.Log("used carrot (no function rn)");
                LLB.GetComponent<LLB>().strength = 4;
                LLB.GetComponent<LLB>().weaponType = 'b';
                Destroy(gameObject);
                break;
            case ItemType.POCKETKNIFE:
                Debug.Log("used knife (no function rn)");
                LLB.GetComponent<LLB>().strength = 4;
                LLB.GetComponent<LLB>().weaponType = 'b';
                Destroy(gameObject);
                break;
            case ItemType.SNAPS:
                BoardGenerator board = GameObject.Find("LevelTilesGenerator").GetComponent<gameManager>().board;
                int xx = (int)(Random.value * board.getBoardWidth());
                int yy = (int)(Random.value * board.getBoardHeight());

                while (board.map[xx, yy].tileType != TileSet.FLOOR && board.map[xx, yy].entity == null)
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

    // Update is called once per frame
    // void Update () 
    // {

    // }

    
}
