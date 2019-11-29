using System.Collections;
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
        BackgroundMenu.SetActive(false);
        InventoryMenu = GameObject.Find("InventoryBackground");
        BackgroundMenu.SetActive(false);
        TargetTile = GameObject.Find("Highlight");
        dungeonDepth = 4;


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
            board.HamsterEntity = GameObject.Find("Player");
            board.GenMap(0);
            Debug.Log("GM SEES " + board.map[499, 500].tileType);
            board.printRecords();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.P)) { // if keypress P pause menu is brought up
            BackgroundMenu.SetActive(true);
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
