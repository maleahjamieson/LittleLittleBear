using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    /*
     Ok so this is a bit rough right now but here is how we fix it,look up how to make
     an array of transforms, make 10 tiles, then during activate edit the first r 
     transforms to be on screen, deactivate changes those transforms back to (0, 0). 
     Super sketch but no more NULLS!
     */
    private int max = 10;
    public int range = 0;
    public char direction;
    public bool visible = false;
    public GameObject LLB;
    public GameObject[] tileList;

    public void Start()
    {
        for(int i = 0; i < tileList.Length; i++)
        {
            Debug.Log("Hide a temp!!!!" + i + "<---");
            tileList[i].SetActive(false);
        }
    }
    public void Activate(int r, bool flipped)
    {
        range = r;
        visible = true;
        if (tileList != null)
        { 
            for (int i = 0; i < r; i++)
            {
                tileList[i].SetActive(true);
                if (!flipped)
                {
                    tileList[i].transform.position = new Vector2(LLB.transform.position.x + (i + 1), LLB.transform.position.y);
                }
                else
                    tileList[i].transform.position = new Vector2(LLB.transform.position.x - (i + 1), LLB.transform.position.y);
            }
        }

    }
    public void Deactivate()
    {
        visible = false;
        for (int i = 0; i < range; i++)
        {
            Debug.Log("Hide a temp!!!!" + i + "<---");
            tileList[i].SetActive(false); // Makes the tiles invisible
        }
    }
    private void Update()
    {
       // visible = false;
    }

    public void Aim(char d)
    {
        for (int i = 0; i < range; i++)
        {
            switch (d)
            {
                case 'l':
                    tileList[i].transform.position = new Vector2(LLB.transform.position.x - (i+1), LLB.transform.position.y);
                    break;
                case 'r':
                    tileList[i].transform.position = new Vector2(LLB.transform.position.x + (i + 1), LLB.transform.position.y);
                    break;
                case 'u':
                    tileList[i].transform.position = new Vector2(LLB.transform.position.x, LLB.transform.position.y + (i + 1));
                    break;
                case 'd':
                    tileList[i].transform.position = new Vector2(LLB.transform.position.x, LLB.transform.position.y - (i + 1));
                    break;
            }
            
        }
    }
}
