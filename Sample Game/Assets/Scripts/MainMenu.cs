using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string GameSceneName;

    public void PlayButton()
    {
        SceneManager.LoadScene(GameSceneName);
    }
}
