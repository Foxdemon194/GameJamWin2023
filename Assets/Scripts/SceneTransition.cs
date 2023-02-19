using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    public void LoadTutorial() //this loads the tutorial, not level 1
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void LoadL1()
    {
        SceneManager.LoadScene("Level1");
    }
    public void LoadL2()
    {
        SceneManager.LoadScene("Level2");
    }
    public void LoadL3()
    {
        SceneManager.LoadScene("Level3");
    }
    public void LoadL4()
    {
        SceneManager.LoadScene("Level4");
    }

    public void LoadSettingsMenu()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Unpause()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
}
