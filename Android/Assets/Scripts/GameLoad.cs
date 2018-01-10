using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoad : MonoBehaviour {

    public void GameModeOne()
    {
        SceneManager.LoadScene("GameMode1", LoadSceneMode.Single);
    }

    public void GameModeTwo()
    {
        SceneManager.LoadScene("GameMode2", LoadSceneMode.Single);
    }

    public void GameModeThree()
    {
        SceneManager.LoadScene("GameMode3", LoadSceneMode.Single);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Menus", LoadSceneMode.Single);
    }
}
