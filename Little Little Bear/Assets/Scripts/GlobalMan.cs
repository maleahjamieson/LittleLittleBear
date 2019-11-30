using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMan : MonoBehaviour
{
    static public GlobalMan instance;
    public PlayerData data;

    //more stuff to be loaded into llb


    public void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
