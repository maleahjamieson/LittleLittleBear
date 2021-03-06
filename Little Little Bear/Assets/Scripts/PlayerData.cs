﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerData 
{
    public int health;
    public int stamina;
    public int strength;
    public int maxHealth;
    public int depth;
    public int ammo;//ammo, weapontype
    public char weaponType;
    //inventory stuff
    public bool[] isFull;
    public InventoryItem[] items = new InventoryItem[4];
    //public string slotTag; // needs to be set

    public PlayerData(LLB llb, Inventory inv)
    {
        //place variables in here such as
        //level = player.level
        health = llb.health;
        stamina = llb.stamina;
        strength = llb.strength;
        maxHealth = llb.maxHealth;
        depth = llb.DungeonDepth;
        ammo = llb.ammo;
        weaponType = llb.weaponType;

        isFull = inv.getFullSlots();
        items = inv.getItemStructs();
        //slotTag = "InventoryTag";
        
    }
    
    
 
}
