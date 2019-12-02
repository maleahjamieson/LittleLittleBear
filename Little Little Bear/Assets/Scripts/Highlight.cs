using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
  
    public int range = 0;
    public char direction;
    public char attackType;
    public bool visible = false;
    public GameObject LLB;
    public GameObject[] tileList;

    // public enum TileType
    // {
    //     NOTHING = -1,
    //     FWall = "tile_Forest_Wall", FTun = "tile_Forest_Tunnel", 
    //     SWall = "tile_Swamp_Wall", STun = "tile_SwampTunnel", 
    //     CWall = "tile_Cave_Wall", CTun = "tile_Cave_Tunnel"
    // };

    public void Start()
    {
        for(int i = 0; i < tileList.Length; i++) // hide all the tiles
        {
            tileList[i].SetActive(false);
        }
    }
    public void Activate(int r, bool flipped, char t, char d) // Only activate the tiles needed in the proper formation
    {
        range = r; // how many tiles
        attackType = t; // which type of attack
        visible = true; // boolean for a check, but may not need later
        if (tileList != null) // make sure list has gameobjects
        {
            switch (attackType)
            {
                case 's': // slice, T shape
                    tileList[0].SetActive(true); // turn on tiles 0-4
                    tileList[1].SetActive(true);
                    tileList[2].SetActive(true);
                    tileList[3].SetActive(true); 
                    tileList[4].SetActive(true);

                    Aim(d);
                    
                    break;
                // case 'm': // Mouse-over highlight
                    
                //     break;
                default: // Range, thrust, blunt and basic
                for (int i = 0; i < r; i++) // runs through range and places in a line
                {
                    tileList[i].SetActive(true);
                }
                    Aim(d);
                break;
            }
        }

    }
    public void Deactivate()
    {
        visible = false;
        for (int i = 0; i < range; i++)
        {
            tileList[i].SetActive(false); // Makes the tiles invisible
        }
    }

    public void Aim(char d) // d direction, t type
    {
        switch (attackType) // Special moves aiming
        { // s = slice 
            case 's':  //Makes T shape on all sides
                    switch (d)
                    {
                        case 'l': // left
                            tileList[0].transform.position = new Vector2(LLB.transform.position.x - 1, LLB.transform.position.y);
                            tileList[1].transform.position = new Vector2(LLB.transform.position.x - 1, LLB.transform.position.y + 1);
                            tileList[2].transform.position = new Vector2(LLB.transform.position.x, LLB.transform.position.y + 1);
                            tileList[3].transform.position = new Vector2(LLB.transform.position.x - 1, LLB.transform.position.y - 1);
                            tileList[4].transform.position = new Vector2(LLB.transform.position.x, LLB.transform.position.y - 1);
                            break;
                        case 'r': // right
                            tileList[0].transform.position = new Vector2(LLB.transform.position.x + 1, LLB.transform.position.y);
                            tileList[1].transform.position = new Vector2(LLB.transform.position.x + 1, LLB.transform.position.y + 1);
                            tileList[2].transform.position = new Vector2(LLB.transform.position.x, LLB.transform.position.y + 1);
                            tileList[3].transform.position = new Vector2(LLB.transform.position.x + 1, LLB.transform.position.y - 1);
                            tileList[4].transform.position = new Vector2(LLB.transform.position.x, LLB.transform.position.y - 1);
                            break;
                        case 'u': // up
                            tileList[0].transform.position = new Vector2(LLB.transform.position.x, LLB.transform.position.y + 1);
                            tileList[1].transform.position = new Vector2(LLB.transform.position.x - 1, LLB.transform.position.y + 1);
                            tileList[2].transform.position = new Vector2(LLB.transform.position.x - 1, LLB.transform.position.y);
                            tileList[3].transform.position = new Vector2(LLB.transform.position.x + 1, LLB.transform.position.y + 1);
                            tileList[4].transform.position = new Vector2(LLB.transform.position.x + 1, LLB.transform.position.y);
                        break;
                        case 'd': // down
                            tileList[0].transform.position = new Vector2(LLB.transform.position.x, LLB.transform.position.y - 1);
                            tileList[1].transform.position = new Vector2(LLB.transform.position.x - 1, LLB.transform.position.y -+ 1);
                            tileList[2].transform.position = new Vector2(LLB.transform.position.x - 1, LLB.transform.position.y);
                            tileList[3].transform.position = new Vector2(LLB.transform.position.x + 1, LLB.transform.position.y - 1);
                            tileList[4].transform.position = new Vector2(LLB.transform.position.x + 1, LLB.transform.position.y);
                        break;
                    }

                break;
            default:
            for (int i = 0; i < range; i++)
            {
                switch (d)
                {
                    case 'l': // left
                        tileList[i].transform.position = new Vector2(LLB.transform.position.x - (i + 1), LLB.transform.position.y);
                        break;
                    case 'r': // right
                        tileList[i].transform.position = new Vector2(LLB.transform.position.x + (i + 1), LLB.transform.position.y);
                        break;
                    case 'u': // up
                        tileList[i].transform.position = new Vector2(LLB.transform.position.x, LLB.transform.position.y + (i + 1));
                        break;
                    case 'd': // down
                        tileList[i].transform.position = new Vector2(LLB.transform.position.x, LLB.transform.position.y - (i + 1));
                        break;
                }

            }
                break;
            
        }
    }
}
