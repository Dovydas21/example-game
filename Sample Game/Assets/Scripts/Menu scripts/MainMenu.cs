using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string GameSceneName;
    public GameObject mainMenu;
    public GameObject panel;
    public GameObject gameUI;
    public OptionsMenu optionsMenu;
    public bool menuOnScreen = false;
    Scene scene;

    private void Start()
    {
        scene = SceneManager.GetActiveScene();
    }

    public void PlayButton()
    {

        if (scene.name == "Game")
        {
            if (menuOnScreen)
            {
                CloseMainMenu();
            }
            else
            {
                OpenMainMenu();
            }
        }
        else
        {
            SceneManager.LoadScene(GameSceneName); // Load the "Game" scene.
        }
    }

    public void OptionsButton()
    {
        // Set the visibility of the options menu to true.
        optionsMenu.ViewOptionsMenu(true);
        ViewPanel(true);

        // Set the visibility of the main menu to false.
        ViewMainMenu(false);
    }

    public void ViewMainMenu(bool visible) // Set the visility of the main menu to the value of the parameter.
    {
        mainMenu.SetActive(visible);
        menuOnScreen = visible;
    }

    public void CloseMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        ViewPanel(false);
        gameUI.SetActive(true);
        mainMenu.SetActive(false);
        Cursor.visible = false;
        menuOnScreen = false;
    }

    public void OpenMainMenu()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        ViewPanel(true);
        gameUI.SetActive(false);
        mainMenu.SetActive(true);
        Cursor.visible = true;
        menuOnScreen = true;
    }

    public void ViewPanel(bool visible)
    {
        panel.SetActive(visible);
    }

    public void QuitButton()
    {
        // Quit the game (only works once game is built).
        Application.Quit();
    }
}
