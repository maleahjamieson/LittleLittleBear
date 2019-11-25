using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

	//inventory variables
	private Inventory inventory;
	public GameObject itemButton;

	// Start is called before the first frame update
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
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

    
}
