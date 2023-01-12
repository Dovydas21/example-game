using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menus : MonoBehaviour
{ 
    public KeyCode pauseKey; // The key that the player will need to press in order to open the menu.
    public MainMenu mainMenuScript;
    public OptionsMenu optionsMenuScript;

    private void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(pauseKey) && !mainMenuScript.menuOnScreen && !optionsMenuScript.menuOnScreen)
        {
            // If the player presses escape and the main menu and the options menu aren't already on screen.
            mainMenuScript.OpenMainMenu();
        }
        else if (Input.GetKeyDown(pauseKey) && mainMenuScript.menuOnScreen && !optionsMenuScript.menuOnScreen)
        {
            // If the player presses escape when on the main menu, close it.
            mainMenuScript.CloseMainMenu();
        }
        else if (Input.GetKeyDown(pauseKey) && optionsMenuScript.menuOnScreen)
        {
            // If the player presses escape when on the options menu, go back to the main menu.
            optionsMenuScript.BackButton();
        }
    }
}