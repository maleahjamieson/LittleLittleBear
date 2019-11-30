﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class gameManager : MonoBehaviour {
	static public gameManager instance;
	private UIManager uiManager;
    public BoardGenerator board;
	//for turning credits on or off
	public GameObject CreditsMenu;
    public GameObject SaveMenu;
    public GameObject LLB; //LLB entity
    public GameObject BackgroundMenu; //background menu
    public GameObject InventoryMenu;
    public GameObject TargetTile;
    public GameObject GlobalMan;
    public int depth;
	//for pausing
	public bool Paused;
	public bool CreditsOn;
    //True if New game, false if Loading game within the SaveMenu Object
    public bool NewOrLoad;
    public bool SaveOn;

    public int dungeonDepth; // For levels
	// Use this for initialization

	//sets up global instance of gameManager
	public void Awake()
    {
        instance = this;
    }
	void Start () {
		//if credit button found, populate CreditsMenu object handle 
		
		if(GameObject.Find("CreditsMenu") != null)
		{
			//disables CreditsMenu on startup
			CreditsMenu = GameObject.Find("CreditsMenu");
			CreditsMenu.SetActive(false);
		}
        BackgroundMenu = GameObject.Find("MenuBackground");
      
        InventoryMenu = GameObject.Find("InventoryBackground");
        BackgroundMenu.SetActive(false);
        TargetTile = GameObject.Find("Highlight");

        if (GameObject.Find("GlobalManager").GetComponent<GlobalMan>())
        {
            if (GameObject.Find("GlobalManager").GetComponent<GlobalMan>().data.depth != 0)
            {
                dungeonDepth = GameObject.Find("GlobalManager").GetComponent<GlobalMan>().data.depth;
                Debug.Log("1DEPTH REEEEEEE " + dungeonDepth);
                GameObject.Find("GlobalManager").GetComponent<GlobalMan>().data.depth = 0;
            }
            else
            {
                dungeonDepth = 1;
            }
        }
        else {
            dungeonDepth = 1;
        }
        LLB.GetComponent<LLB>().DungeonDepth = dungeonDepth;
        Debug.Log("2DEPTH REEEEEEE " + dungeonDepth);

        // Random.seed = System.DateTime.Now.Millisecond; // Seed generator
        Random.InitState(System.DateTime.Now.Millisecond); // Unity's preferred way to seed the RNG
        if (board != null)                                  //if board is loaded
        {
            // board = new BoardGenerator(1000, 1000); // Removed this line to comply with Unity better
            board = gameObject.AddComponent(typeof(BoardGenerator)) as BoardGenerator;
            board.setBoardSize(2500, 2500);
            board.setDungeonDepth(dungeonDepth);
            board.generate();
            Debug.Log("Should be generated at depth: "+dungeonDepth);
            //board.printRecords();
            // board.Floor = GameObject.Find("TileForestGround");
            // board.Wall = GameObject.Find("TileForestWall");
            // board.Puzzle_Floor = GameObject.Find("TileForestPuzzleGround");
            // board.Hallway = GameObject.Find("TileForestHallway");
            // board.Puzzle_Hallway = GameObject.Find("TileForestPuzzleHallway");
            // board.Spawner = GameObject.Find("TileForestSpawner");
            // board.Secret_Floor = GameObject.Find("TileForestSecretGround");
            // board.Trap = GameObject.Find("TileForestTrap");
            // board.Dig_Tile = GameObject.Find("TileForestDig");
            // board.Start_Tile = GameObject.Find("TileForestStartTile");
            // board.End_Tile = GameObject.Find("TileForestEndTile");
            board.HamsterEntity = GameObject.Find("Player");
            board.GenMap(0);
            Debug.Log("GM SEES " + board.map[499, 500].tileType);
            board.printRecords();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.P)) { // if keypress P pause menu is brought up

            if (!BackgroundMenu.activeSelf)
            {
                BackgroundMenu.SetActive(true);
            }
            else
            {
                BackgroundMenu.SetActive(false);
            }
        }
        if (Input.GetKey(KeyCode.I)) // if keypress I then inventory is brought up
        {
            InventoryMenu.SetActive(true);
        }
    }
	public void LoadScene(int scene)
	{
		switch(scene)
		{
			case 1:
			SceneManager.LoadScene("Level1");
			break;
            case 2:
            SceneManager.LoadScene("Level2");
            break;
        }
	}
}
