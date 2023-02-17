using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LoadSettingsMenu()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Unpause()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }
}
