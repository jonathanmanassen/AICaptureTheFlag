using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();   //exits the game
    }

    public void LoadTestScene()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);   //loads the first scene (test the controls)
    }

    public void LoadCTFScene()
    {
        SceneManager.LoadScene(2, LoadSceneMode.Single);   //loads the second scene (capture the flag)
    }

    public void LoadRandomCTFScene()
    {
        SceneManager.LoadScene(3, LoadSceneMode.Single);   //loads the third scene (random capture the flag)
    }
}
