using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
public class Inventory : MonoBehaviour
{
    public Sprite[] spritesForShit = new Sprite[10];
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = spritesForShit[Random.Range(0,10)];
    }


}
