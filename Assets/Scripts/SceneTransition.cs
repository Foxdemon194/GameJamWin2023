using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

<<<<<<< HEAD
    PauseMenuMan pauseMenuMan;


    public void LoadLevel1() //this loads the tutorial, not level 1
=======
    public void LoadTutorial() //this loads the tutorial, not level 1
>>>>>>> 75e52f8f792a87d8b627afd29b21016685594ef5
    {
        SceneManager.LoadScene("Tutorial");
        Time.timeScale = 1;
    }

    public void LoadL1()
    {
        SceneManager.LoadScene("Level1");
        Time.timeScale = 1;
    }
    public void LoadL2()
    {
        SceneManager.LoadScene("Level2");
        Time.timeScale = 1;
    }
    public void LoadL3()
    {
        SceneManager.LoadScene("Level3");
        Time.timeScale = 1;
    }
    public void LoadL4()
    {
        SceneManager.LoadScene("Level4");
        Time.timeScale = 1;
    }

    public void LoadSettingsMenu()
    {
        SceneManager.LoadScene("SettingsScene");
        Time.timeScale = 1;
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    public void Unpause()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
}
