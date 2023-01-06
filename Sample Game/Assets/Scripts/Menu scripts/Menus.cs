using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menus : MonoBehaviour
{

    /* 
     * Script is probably obsolete as switching the scenes reloads everything
     * making it unideal for a pause menu... 
     */

    public KeyCode pauseKey = KeyCode.Tab;
    public GameObject menu;

    private void Start()
    {
        menu.SetActive(false);
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            print("Menu key presses");
            Time.timeScale = 0f;
            menu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}