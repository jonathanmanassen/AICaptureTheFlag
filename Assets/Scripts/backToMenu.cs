using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class backToMenu : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))  //used to go back to the menu scene
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }
}
