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

	//for pausing
	public bool Paused;
	public bool CreditsOn;
    //True if New game, false if Loading game within the SaveMenu Object
    public bool NewOrLoad;
    public bool SaveOn;
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
		Random.seed = System.DateTime.Now.Millisecond; // Seed generator
        board = new BoardGenerator(1000,1000);
        board.generate();
        //board.printRecords();
        board.Floor = GameObject.Find("TileForestGround");
        board.Wall = GameObject.Find("TileForestWall");
        board.Puzzle_Floor = GameObject.Find("TileForestPuzzleGround");
        board.Hallway = GameObject.Find("TileForestHallway");
        board.Puzzle_Hallway = GameObject.Find("TileForestPuzzleHallway");
        board.Spawner = GameObject.Find("TileForestSpawner");
        board.Secret_Floor = GameObject.Find("TileForestSecretGround");
        board.Trap = GameObject.Find("TileForestTrap");
        board.Dig_Tile = GameObject.Find("TileForestDig");
        board.Start_Tile = GameObject.Find("TileForestStartTile");
        board.End_Tile = GameObject.Find("TileForestEndTile");
        board.HamsterEntity = GameObject.Find("Player");
        board.GenMap(0);
        Debug.Log("GM SEES " + board.map[499, 500].tileType);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void LoadScene(int scene)
	{
		switch(scene)
		{
			case 1:
			SceneManager.LoadScene("Level1");
			break;
		}
	}
}
