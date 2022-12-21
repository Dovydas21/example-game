using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string GameSceneName;
    public GameObject mainMenu;
    public OptionsMenu optionsMenu;

    public void PlayButton()
    {
        // Load the "Game" scene.
        SceneManager.LoadScene(GameSceneName);
    }

    public void OptionsButton()
    {
        // Set the visibility of the options menu to true.
        optionsMenu.ViewOptionsMenu(true);

        // Set the visibility of the main menu to false.
        ViewMainMenu(false);
    }

    public void ViewMainMenu(bool visible)
    {
        // Set the visility of the main menu to the value of the parameter.
        mainMenu.SetActive(visible);
    }

    public void QuitButton()
    {
        // Quit the game (only works once game is built).
        Application.Quit();
    }
}
