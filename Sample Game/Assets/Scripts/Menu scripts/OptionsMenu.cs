using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public MainMenu mainMenu;

    public Slider volumeSlider, sensitivitySlider;
    public PlayerLook playerLook; // Reference to "PlayerLook.cs" which is where the sensitivity values are set.

    void Start()
    {

        // Volume
        float savedVolume = PlayerPrefs.GetFloat("OPTIONS_Volume"); // Get the volume that we have got saved and set it.
        if (savedVolume > 0) // If we have a value for it then use it.
        {
            volumeSlider.value = savedVolume;
            SetVolume(volumeSlider.value);
        }
        else
        {
            volumeSlider.value = 1f;
            SetVolume(volumeSlider.value);
        }


        // Sensitivity
        float savedSensitivity = PlayerPrefs.GetFloat("OPTIONS_Sensitivity"); // Get the sensitivity that we have got saved and set it.

        sensitivitySlider.value = savedSensitivity;
        SetSensitivity(sensitivitySlider.value);
    }

    public void SetVolume(float volume)
    {
        print(volume);
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("OPTIONS_Volume", volume); // Save the volume the player selected so the next time they open the game they don't have to set it again.
    }

    public void SetSensitivity(float sensitivity)
    {
        playerLook.xSensitivity = sensitivity;
        playerLook.ySensitivity = sensitivity;
        PlayerPrefs.SetFloat("OPTIONS_Sensitivity", sensitivity); // Save the sensitivity the player selected so the next time they open the game they don't have to set it again.
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
