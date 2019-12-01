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
    	Debug.Log("Running pickup function");
	    for(int i = 0; i < inventory.slots.Length; i++) //checking if inventory is full
	    {
	        if(inventory.isFull[i] == false)    //not full, pickup item
	        {
	            Debug.Log("Picked up item");
	            //add item
	            inventory.isFull[i] = true;

                inventory.items[i].damage = this.damage;
                inventory.items[i].damageType = this.damageType;
                inventory.items[i].range = this.range;
                inventory.items[i].type = this.itemType;
	            
	            //if item is blueberry then this
	            GameObject button = Instantiate(GameObject.Find("ButtonItem"), inventory.slots[i].transform, false);
                button.GetComponent<Image>().sprite = GetComponent<SpriteRenderer>().sprite;
	            Destroy(gameObject);
	            break;
        	}
    	}
    }
    public void use() 
    {
        switch (itemType)
        {
            case ItemType.RED_ANTS_BOTTLE: // ants
                break;
            case ItemType.BLUEBERRIES: // berry
                if (LLB.GetComponent<LLB>().health >= (LLB.GetComponent<LLB>().maxHealth - 20))
                    LLB.GetComponent<LLB>().health = LLB.GetComponent<LLB>().maxHealth; // Only goes as high as max health
                else
                    LLB.GetComponent<LLB>().health += 20; // increase by 20
                
                break;
            case ItemType.SKUNK_GAS: // skunk
                break;
            case ItemType.TREAT: // treat
                break;
            case ItemType.STICK_ROCK:
                break;
            case ItemType.RAPIER:
                break;
            case ItemType.CARROT:
                break;
            case ItemType.POCKETKNIFE:
                break;
            default:
                Debug.Log("UH OH, item isnt set!");
                break;
        }
    }

    
}
