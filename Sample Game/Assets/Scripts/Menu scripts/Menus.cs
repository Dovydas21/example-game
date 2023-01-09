using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menus : MonoBehaviour
{ 
    public KeyCode pauseKey; // The key that the player will need to press in order to open the menu.
    public MainMenu mainMenuScript;

    private void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pauseKey) && !mainMenuScript.menuOnScreen)
        {
            mainMenuScript.OpenMainMenu();
        }
        else if (Input.GetKeyDown(pauseKey) && mainMenuScript.menuOnScreen)
        {
            mainMenuScript.CloseMainMenu();
        }
    }
}