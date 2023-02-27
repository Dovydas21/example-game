using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{

    public MainMenu mainMenuScript;
    public GameObject deathMenu;
    public GameObject gameUI;
    public bool menuOnScreen = false;
    Scene scene;
    GameObject panel;

    private void Start()
    {
        scene = SceneManager.GetActiveScene();
    }

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
        // Reload the Game scene to restart the game.
        SceneManager.LoadScene(scene.name);
    }

    public void QuitButton()
    {
        mainMenuScript.QuitButton();
    }
}
