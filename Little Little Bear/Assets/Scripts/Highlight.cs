using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    public int range = 0;
    public char direction;
    public GameObject tile; // Tile we will be spawning 
    public GameObject[] tileList;

    public void initialize(int r)
    {
        if (range != r)
        {
            tileList = new GameObject[range];
            for(int i = 0; i < r; i++)
            {
                GameObject temp = Instantiate(tile);
            }
        }
        
         
    }

    public void Aim(char d)
    {
        direction = d;
    }
}
