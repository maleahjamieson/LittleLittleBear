using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//enum that contains all UI buttons
public enum ClickType {MainMenu, Pause, Save, NewGame, LoadGame, Settings, Credits, Music, ExitMenu, Inventory

}
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
                if (!gameManager.instance.SaveMenu.activeSelf)
                {
                    gameManager.instance.NewOrLoad = true;
                    gameManager.instance.SaveMenu.SetActive(true);
                }
                else
                {
                    gameManager.instance.SaveMenu.SetActive(false);
                }

                // Moved the break below EraseSaveFiles and the LoadScene function
                //erase Save files
                EraseSaveFiles();
                //load save from txt probs would be best here
                //force load level 1
				gameManager.instance.LoadScene(1);
				break;
            case ClickType.Save:
                SaveData.SavePlayer(gameManager.instance.LLB.GetComponent<LLB>(), null);
                break;
			case ClickType.LoadGame:

                GlobalMan.instance.data = SaveData.LoadPlayer();
                Debug.Log(GlobalMan.instance.data.health);

                
                /*
                if (!gameManager.instance.SaveMenu.activeSelf)
                {
                    gameManager.instance.NewOrLoad = false;
                    gameManager.instance.SaveMenu.SetActive(true);
                }
                else
                {
                    
                    gameManager.instance.SaveMenu.SetActive(false);
                }*/
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
            case ClickType.ExitMenu:
                if (!gameManager.instance.BackgroundMenu.activeSelf)
                {
                    gameManager.instance.BackgroundMenu.SetActive(true);
                }
                else
                {
                    gameManager.instance.BackgroundMenu.SetActive(false);
                }
                break;
            case ClickType.Music:
                break;
            case ClickType.Inventory:
                GetComponent<Item>().use();

                break;
		}

	}
	// Update is called once per frame
	void Update () {
		
	}
	int EraseSaveFiles()
	{
        return 0;
	}

}
