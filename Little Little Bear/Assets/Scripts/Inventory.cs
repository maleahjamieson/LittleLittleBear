using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< HEAD
using UnityEngine.Sprites;
public class Inventory : MonoBehaviour
{
    public Sprite[] spritesForShit = new Sprite[10];
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = spritesForShit[Random.Range(0,10)];
    }


=======

public class Inventory : MonoBehaviour
{
    public bool[] isFull;
    public GameObject[] slots;
    public int selectedSlot = 0;

    void Start()
    {
    	
    }

    void Update()
    {

    }
>>>>>>> master
}
