using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

	//inventory variables
	private Inventory inventory;
	public GameObject itemButton;
    public GameObject LLB; // player
    public char itemType; // a ants, b berry, etc
    int playerHealth; // LLB health
    int maxHealth; // This will be updated by treats
	// Start is called before the first frame update
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        LLB = GameObject.Find("Player");
        playerHealth = LLB.GetComponent<LLB>().health;
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
	            
	            //if item is blueberry then this
	            Instantiate(itemButton, inventory.slots[i].transform, false);
	            Destroy(gameObject);
	            break;
        	}
    	}
    }
    public void use() 
    {
        switch (itemType)
        {
            case 'a': // ants
                break;
            case 'b': // berry
                if (playerHealth >= (maxHealth - 20))
                    playerHealth = maxHealth; // Only goes as high as max health
                else
                    playerHealth += 20; // increase by 20
                
                break;
            case 's': // skunk
                break;
            case 't': // treat
                break;
            default:
                Debug.Log("UH OH, item isnt set!");
                break;
        }
    }

    
}
