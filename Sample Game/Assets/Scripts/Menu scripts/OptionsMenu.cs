using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public MainMenu mainMenu;

    public void SetVolume(float volume)
    {

    }

    public void BackButton()
    {
        // Set the visibility of the options menu to false.
        ViewOptionsMenu(false);

        // Set the visibility of the main menu to true.
        mainMenu.ViewMainMenu(true);
    }

    public void ViewOptionsMenu(bool visible)
    {
        // Set the visibility of the options menu to the value of the parameter.
        optionsMenu.SetActive(visible);
    }
}
