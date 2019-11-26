using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerData : MonoBehaviour
{
    public int level;
    public int health;
    public float[] position;

    public PlayerData(LLB llb, Inventory inv)
    {
        //place variables in here such as
        //level = player.level
        //health = player.health
    }
    
    
    
    // Start is called before the first frame update



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
