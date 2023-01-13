using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMenu : MonoBehaviour
{

    public MainMenu mainMenuScript;
    public GameObject deathMenu;
    public GameObject gameUI;
    public bool menuOnScreen = false;
    GameObject panel;

    public void OpenDeathMenu()
    {
        print("Opening death menu");
        mainMenuScript.ViewPanel(true);
        ViewDeathMenu(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        gameUI.SetActive(false);
        mainMenuScript.ViewMainMenu(false);
        Cursor.visible = true;
        menuOnScreen = true;
    }

    public void ViewDeathMenu(bool visible)
    {
        deathMenu.SetActive(visible);
        menuOnScreen = visible;
    }

    public void RestartButton()
    {
        mainMenuScript.PlayButton();
    }

    public void QuitButton()
    {
        mainMenuScript.QuitButton();
    }
}
