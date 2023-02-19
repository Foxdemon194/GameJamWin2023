using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuMan : MonoBehaviour
{

    public GameObject pauseMenu;
    public bool gameIsPaused = false;

    private void Start()
    {
        gameIsPaused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(gameIsPaused)
            {
                ClosePause();
            }
            else
            {
                OpenPause();            }
        }
    }

    public void OpenPause()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        gameIsPaused = true;
    }

    public void ClosePause()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        gameIsPaused = false;
    }
}
