using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//enum that contains all UI buttons
public enum ClickType {MainMenu, Pause, NewGame, LoadGame, Settings, Credits}
public class UIManager : MonoBehaviour {
	
	static public gameManager GManager;

	// Use this for initialization
	void Start () {
		
	}
	public ClickType clickType;
	public void OnClick(){
		//switch containing all actions for each button
		switch (clickType)
		{
			case ClickType.MainMenu:
			break;
			case ClickType.NewGame:
			//erase Save files
			EraseSaveFiles();
			//force load level 1
			gameManager.instance.LoadScene(1);
			break;
			case ClickType.LoadGame:
			//load save from txt probs would be best here
			break;
			case ClickType.Pause:
			//set instance of pauseMenu(needs to be created) to active
			break;
			//can use this to turn on and off credits in main menu
			case ClickType.Credits:
			if(!gameManager.instance.CreditsMenu.activeSelf)
			{
				gameManager.instance.CreditsMenu.SetActive(true);
			}
			else
			{
				gameManager.instance.CreditsMenu.SetActive(false);
			}
			break;
		}

	}
	// Update is called once per frame
	void Update () {
		
	}
	void EraseSaveFiles()
	{

	}

}
