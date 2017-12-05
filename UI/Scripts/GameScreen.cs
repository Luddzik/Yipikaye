using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScreen : MonoBehaviour {

    [SerializeField] private GameObject gameScreen;
    [SerializeField] private GameObject pauseScreen;

    public void PauseScreen()
    {
        Time.timeScale = 0;
        gameScreen.SetActive(false);
        pauseScreen.SetActive(true);
    }

    public void Resume()
    {
        gameScreen.SetActive(true);
        pauseScreen.SetActive(false);
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        pauseScreen.SetActive(false);
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MainScreen", LoadSceneMode.Single);
    }

}
