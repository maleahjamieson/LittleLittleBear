using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
using UnityEngine.Sprites;
public class Inventory : MonoBehaviour
{
    public Sprite[] spritesForShit = new Sprite[10];
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = spritesForShit[Random.Range(0,10)];
    }



*/
public class Inventory : MonoBehaviour
{
    public bool[] isFull;
    public InventoryItem[] items = new InventoryItem[4];
    public GameObject[] slots;
    public int selectedSlot = 0;

    public bool[] getFullSlots()
    {
        return isFull;
    }

    public InventoryItem[] getItemStructs()
    {
        return items;
    }

    public GameObject[] getSlotButtons()
    {
        return slots;
    }

    void Start()
    {
    	
    }

    void Update()
    {

    }

}
